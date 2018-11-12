using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoScreenPi.Entities;
using InfoScreenPi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using InfoScreenPi.ViewModels;
using InfoScreenPi.Infrastructure.Services;
using InfoScreenPi.Infrastructure.Repositories;
using InfoScreenPi.Infrastructure.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;

namespace InfoScreenPi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ConfigController : Controller
    {
        private InfoScreenContext _context;
        private readonly IMembershipService _membershipService;
        private readonly IUserRepository _userRepository;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IBackgroundRepository _backgroundRepository;
        private readonly IRssFeedRepository _rssFeedRepository;
        private readonly IDataProtector _protector;
        private readonly ISettingRepository _settingRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public ConfigController(InfoScreenContext context, 
                                IMembershipService membershipService,
                                IUserRepository userRepository,
                                ILoggingRepository _errorRepository,
                                IItemRepository itemRepository,
                                IHostingEnvironment hostEnvironment,
                                IBackgroundRepository backgroundRepository,
                                IRssFeedRepository rssFeedRepository,
                                IDataProtectionProvider dataProtectionProvider,
                                ISettingRepository settingRepository,
                                IEncryptionService encryptionService,
                                IRoleRepository roleRepository,
                                IUserRoleRepository userRoleRepository)
        {
            _context = context;
            _membershipService = membershipService;
            _userRepository = userRepository;
            _loggingRepository = _errorRepository;
            _itemRepository = itemRepository;
            _hostEnvironment = hostEnvironment;
            _backgroundRepository = backgroundRepository;
            _rssFeedRepository = rssFeedRepository;
            _protector = dataProtectionProvider.CreateProtector("cookies");
            _settingRepository = settingRepository;
            _encryptionService = encryptionService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") != null) ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.ActiveItems = (List<Item>) _itemRepository.AllIncluding(a => a.Background, a => a.Soort).Where(i => i.Soort.Description != "RSS" && i.Archieved == false).ToList();
            ViewBag.TickerItems = new List<string>(System.IO.File.ReadAllLines(_hostEnvironment.WebRootPath + "/data/ticker.txt")); 
            ViewBag.Backgrounds = (List<Background>) _backgroundRepository.GetAllWithoutRSS(false).ToList();
            ViewBag.RssAbo = (List<RssFeed>) _rssFeedRepository.AllIncluding(r => r.StandardBackground).ToList();
            return View(_context.Users.ToList());
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            //User _user = _membershipService.CreateUser("TVDA", "thomas.vandenabeele@kuleuven.be", "i1n2f3o4", new int[] { 1 });

            IActionResult _result = new ObjectResult(false);
            GenericResult _authenticationResult = null;

            try
            {
                MembershipContext _userContext = _membershipService.ValidateUser(user.Username, user.Password);

                if (_userContext.User != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, _userContext.User.Email),
                        new Claim("FullName", _userContext.User.LastName + " " + _userContext.User.FirstName),
                        new Claim("Id", _userContext.User.Id.ToString()),
                        //new Claim(ClaimTypes.Role, _claims.First().ToString()),
                        new Claim(ClaimTypes.Role, "Admin", ClaimValueTypes.String, user.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        ///ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = user.RememberMe,
                        // Whether the authentication session is persisted across 
                        // multiple requests. Required when setting the 
                        // ExpireTimeSpan option of CookieAuthenticationOptions 
                        // set with AddCookie. Also required when setting 
                        // ExpiresUtc.

                        ///IssuedUtc = DateTimeOffset.UtcNow,
                        // The time at which the authentication ticket was issued.

                        RedirectUri = "config"
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), 
                        authProperties);
                    

                    // await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance",
                        // new ClaimsPrincipal(new ClaimsIdentity(_claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                        // new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties {IsPersistent = user.RememberMe });


                    _authenticationResult = new GenericResult()
                    {
                        Succeeded = true,
                        Message = "Authentication succeeded"
                    };

                    User loggedUser = _userRepository.GetSingleByUsername(user.Username);
                    //HttpContext.Response.Cookies.Append("YU2ert-gert24-59HEHF-thtyyE-87R23!", _protector.Protect(loggedUser.Id.ToString()), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });

                    loggedUser.LastLogin = DateTime.Now;
                    _userRepository.Edit(loggedUser);
                    _userRepository.Commit();

                    return RedirectToAction("Index", "Config");
                }
                else
                {
                    _authenticationResult = new GenericResult()
                    {
                        Succeeded = false,
                        Message = "Authentication failed"
                    };
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                _authenticationResult = new GenericResult()
                {
                    Succeeded = false,
                    Message = ex.Message
                };

                _loggingRepository.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                _loggingRepository.Commit();
            }

            _result = new ObjectResult(_authenticationResult);
            return View(user);
        }

        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return RedirectToAction("login","config");
            }
            catch (Exception ex)
            {
                _loggingRepository.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                _loggingRepository.Commit();

                return BadRequest();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails()
        {
            //string idString = _protector.Unprotect(HttpContext.Request.Cookies["YU2ert-gert24-59HEHF-thtyyE-87R23!"]); // id
            string idString = "";
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                idString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            }
            int? id = Convert.ToInt32(idString);
            User model = null;
            if (id != null && id != 0)
            {
                model = await _userRepository.GetSingleAsync((int)id);
            }
            
            return PartialView("~/Views/Config/Account/Details.cshtml", model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateUser(string username, string email, string firstname, string lastname, string wachtwoord)
        {

            var passwordSalt = _encryptionService.CreateSalt();

            var user = new User()
            {
                Username = username,
                FirstName = firstname,
                LastName = lastname,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(wachtwoord, passwordSalt),
                DateCreated = DateTime.Now
            };

            _userRepository.Add(user);
            _userRepository.Commit();

            var role = _roleRepository.GetSingle(1);
            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _userRoleRepository.Add(userRole);

            _userRoleRepository.Commit();

            _userRepository.Commit();

            return Json(new {success = true, gebruiker= user});
        }

        [HttpPost]
        public ActionResult SaveTicker(List<string> listkey)
        {
            var n = listkey;
            System.IO.File.WriteAllLines(_hostEnvironment.WebRootPath + "/data/ticker.txt", listkey.Where(str => str != null));
            return Json(new {success= true, message="Ingevoerde data voor de infoticker opgeslagen!"});
        }

        [HttpGet]
        public ActionResult GetSettings()
        {
            List<Setting> model = _settingRepository.GetAll().ToList();
            return PartialView("~/Views/Config/Settings/Modify.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveSettings(Dictionary<string, string> parameters)
        {
            foreach(var setting in parameters)
            {
                _settingRepository.SetSettingByName(setting.Key, setting.Value);
            }
            return Json(new {success= true, message="Instellingen opgeslagen!"});
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetRefresh(Boolean status)
        {
            _settingRepository.SetSettingByName("Refresh", status.ToString());
            return Json(new {success= true, message="Het scherm zal refreshen binnen 15 seconden"});
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetRefresh()
        {
            Boolean RefreshStatus = Convert.ToBoolean(_settingRepository.GetSettingByName("Refresh"));
            return Json(new {success = true, status = RefreshStatus, message="Gelukt"});
        }
    }
}
