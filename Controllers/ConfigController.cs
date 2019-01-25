using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoScreenPi.Entities;
using InfoScreenPi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using InfoScreenPi.ViewModels;
using InfoScreenPi.Infrastructure.Services;
using InfoScreenPi.Infrastructure.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoScreenPi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ConfigController : BaseController
    {
        private InfoScreenContext _context;
        private readonly IMembershipService _membershipService;
        private readonly IEncryptionService _encryptionService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ConfigController(InfoScreenContext context,
                                IMembershipService membershipService,
                                IDataService dataService,
                                IHostingEnvironment hostEnvironment,
                                IDataProtectionProvider dataProtectionProvider,
                                IEncryptionService encryptionService)
        : base(dataService, hostEnvironment)
        {
            _context = context;
            _membershipService = membershipService;
            _encryptionService = encryptionService;
            _hostingEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") != null) ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.ActiveItems = _data.GetItemsNoRss(false).ToList();
            ViewBag.TickerItems = new List<string>(System.IO.File.ReadAllLines(_hostEnvironment.WebRootPath + "/data/ticker.txt"));
            ViewBag.Backgrounds = _data.GetBackgroundsNoRSS(false).ToList();
            ViewBag.RssAbo = _data.GetAll<RssFeed>(r => r.StandardBackground).ToList();
            ViewBag.Logo = _data.GetSettingByName("LogoUrl");
            ViewBag.TitleProg = _data.GetSettingByName("Title");

            string idString = "";
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                idString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            }
            //string idString = _protector.Unprotect(HttpContext.Request.Cookies["YU2ert-gert24-59HEHF-thtyyE-87R23!"]); // id
            int? id = Convert.ToInt32(idString);

            if (id != null && id != 0)
            {
                int roleId = _data.GetAll<UserRole>().First(ur => ur.UserId == id).RoleId;
                Role role = _data.GetSingle<Role>(roleId);

                ViewBag.CurrentRole = role;
            }

            return View(_context.Users.ToList());
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.Logo = _data.GetSettingByName("LogoUrl");
            return View();
            //i => !(i is RSSItem) && ((IExpiring)i).Archieved == false, a => ((IStatic)a).Background
            /*var test = _data.GetAll<RSSItem>().ToList();
            var a = 0;
            return Success(""+test[0].Title);*/
            /*bool archieved = true;
            List<Background> exclAchtergronden = _data.GetAll<Item>(i => i is IStatic && (i is RSSItem  || (archieved? false : ((IExpiring)i).Archieved)))
                                                 .Select(i => ((IStatic) i).Background).ToList();
            archieved = false;
            List<Background> exclAchtergronden2 = _data.GetAll<Item>(i => i is IStatic && (i is RSSItem  || (archieved? false : ((IExpiring)i).Archieved)))
                                                 .Select(i => ((IStatic) i).Background).ToList();
            return Success("Archived = true: "+exclAchtergronden.Count()+" Archived = false: "+exclAchtergronden2.Count());*/
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

                    User loggedUser = _data.GetSingleByUsername(user.Username);
                    //HttpContext.Response.Cookies.Append("YU2ert-gert24-59HEHF-thtyyE-87R23!", _protector.Protect(loggedUser.Id.ToString()), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });

                    loggedUser.LastLogin = DateTime.Now;
                    _data.Edit(loggedUser);
                    _data.Commit();

                    return RedirectToAction("Index", "Config");
                }
                else
                {
                    _authenticationResult = new GenericResult()
                    {
                        Succeeded = false,
                        Message = "Authentication failed"
                    };
                    ViewBag.Logo = _data.GetSettingByName("LogoUrl");
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

                _data.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                _data.Commit();
            }

            _result = new ObjectResult(_authenticationResult);
            ViewBag.Logo = _data.GetSettingByName("LogoUrl");
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
                _data.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                _data.Commit();

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
                model = await _data.GetSingleAsync<User>((int)id);
            }

            return PartialView("~/Views/Config/Account/Details.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOperatingTableView(int devideId)
        {
            //String JSONString = "{\"screenOn\":[{\"day\":2,\"hour\":9,\"minutes\":0},{\"day\":3,\"hour\":8,\"minutes\":45},{\"day\":4,\"hour\":9,\"minutes\":45},{\"day\":5,\"hour\":7,\"minutes\":30},{\"day\":6,\"hour\":10,\"minutes\":45}],\"screenOff\":[{\"day\":2,\"hour\":11,\"minutes\":15},{\"day\":3,\"hour\":10,\"minutes\":30},{\"day\":4,\"hour\":11,\"minutes\":0},{\"day\":5,\"hour\":9,\"minutes\":30},{\"day\":6,\"hour\":12,\"minutes\":15}]}";
            //String JSONString =
            //    "{\"screenOn\":[{\"day\":1,\"hour\":8,\"minutes\":15},{\"day\":1,\"hour\":17,\"minutes\":0},{\"day\":2,\"hour\":17,\"minutes\":0},{\"day\":2,\"hour\":8,\"minutes\":15},{\"day\":3,\"hour\":17,\"minutes\":0},{\"day\":3,\"hour\":13,\"minutes\":30},{\"day\":3,\"hour\":8,\"minutes\":15},{\"day\":4,\"hour\":17,\"minutes\":0},{\"day\":4,\"hour\":8,\"minutes\":15},{\"day\":5,\"hour\":17,\"minutes\":0},{\"day\":5,\"hour\":8,\"minutes\":15},{\"day\":6,\"hour\":8,\"minutes\":15}],\"screenOff\":[{\"day\":1,\"hour\":12,\"minutes\":0},{\"day\":1,\"hour\":20,\"minutes\":30},{\"day\":2,\"hour\":20,\"minutes\":30},{\"day\":2,\"hour\":12,\"minutes\":0},{\"day\":3,\"hour\":20,\"minutes\":30},{\"day\":3,\"hour\":15,\"minutes\":30},{\"day\":3,\"hour\":12,\"minutes\":0},{\"day\":4,\"hour\":20,\"minutes\":30},{\"day\":4,\"hour\":12,\"minutes\":0},{\"day\":5,\"hour\":20,\"minutes\":30},{\"day\":5,\"hour\":12,\"minutes\":0},{\"day\":6,\"hour\":12,\"minutes\":0}]}";

            String JSONString = _data.GetSingle<Device>(i => i.Id == devideId).OperateString;
            return PartialView("~/Views/Config/Settings/OperatingTable.cshtml", JSONString);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOperatingTimes(string operatingString, int deviceId)
        {

            Device edit = _data.GetSingle<Device>(d => d.Id == deviceId);

            string cronstring = GenerateCronJobList(operatingString, deviceId);

            string path = _hostingEnvironment.ContentRootPath;
            string fn = "crons-dev" + deviceId + ".cr";
            string fileName = Path.Combine(path, "crons", fn);
            using (StreamWriter outFile = new StreamWriter(fileName))
            {
                outFile.Write(cronstring);
            }

            // copy cronlist to remote rpi screen
            string scpCommand = "scp " + fileName + " pi@" + edit.IP + ":~/cron/" + fn;
            //var outputScp = scpCommand.Bash();

            // renew crons on remote rpi with crontab
            string sshCommand = "ssh pi@" + edit.IP + " '" + edit.CronCommand + " ~/cron/" + fn + "'";
            //var outputSsh = sshCommand.Bash();



            edit.OperateString = operatingString;
            edit.CronString = cronstring;
            _data.Edit(edit);
            _data.Commit();


            return Success("Werkingstijden opgeslagen");
        }

        public string GenerateCronJobList(string operatingString, int deviceId)
        {
            string scrOnCmd = _data.GetSingle<Device>(d => d.Id == deviceId).ScreenOnCommand;
            string scrOffCmd = _data.GetSingle<Device>(d => d.Id == deviceId).ScreenOffCommand;

            //var json = JsonConvert.DeserializeObject(operatingString);
            var json = JObject.Parse(operatingString);
            var cronString = "";
            foreach (var scOn in json["screenOn"])
            {
                cronString = cronString + scOn["minutes"] + " " + scOn["hour"] + " * * " + scOn["day"] + " " + scrOnCmd + "\n";
            }
            foreach (var scOff in json["screenOff"])
            {
                cronString = cronString + scOff["minutes"] + " " + scOff["hour"] + " * * " + scOff["day"] + " " + scrOffCmd + "\n";
            }

            return cronString;
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult CreateUserView()
        {
            return PartialView("~/Views/Config/Account/CreateUser.cshtml");
        }

        [HttpPost]
        public IActionResult RegisterUser (String Username, String Email, String FirstName, String LastName, String Password, String VerifyPassword, bool Admin){
            if(Username != null | Username != ""){
                if(VerifyPassword.Equals(Password)){
                   return CreateUser(Username, Email, FirstName, LastName, Password, Admin);
                }
                else return Fail("Wachtwoorden komen niet overeen");
            }
            else return Fail("Geef gebruikersnaam op");
        }


        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            User s = _data.GetSingle<User>(userId);
            //User s = _userRepository.GetAll().First(u => u.Username.Equals(username));
            _data.Delete(s);
            _data.Commit();

            return Success("Gebruiker verwijderd");

        }

        [HttpGet]
        public ActionResult CreateUser(string username, string email, string firstname, string lastname, string wachtwoord, bool admin)
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

            _data.Add(user);
            _data.Commit();

            string roleName = admin? "Admin" : "Redacteur";
            Role role = _data.GetSingle<Role>(r => r.Name.Equals(roleName));

            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _data.Add(userRole);
            _data.Commit();

            return Json(new {success = true, gebruiker= user});
        }

        [HttpPost]
        public ActionResult SaveTicker(List<string> listkey)
        {
            var n = listkey;
            System.IO.File.WriteAllLines(_hostEnvironment.WebRootPath + "/data/ticker.txt", listkey.Where(str => str != null));
            return Success("Ingevoerde data voor de infoticker opgeslagen!");
        }

        [HttpGet]
        public ActionResult GetSettings()
        {
            List<Setting> model = _data.GetAll<Setting>().ToList();
            List<User> gebruikers = _context.Set<User>().Include(x => x.UserRoles).ThenInclude(x => x.Role).ToList();

            ViewBag.Users = gebruikers;
            ViewBag.Devices = _data.GetAll<Device>().ToList();
            return PartialView("~/Views/Config/Settings/Modify.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveSettings(Dictionary<string, string> parameters)
        {
            foreach(var setting in parameters)
            {
                _data.SetSettingByName(setting.Key, setting.Value);
            }
            return Success();
        }

        [HttpPost]
        [RequestSizeLimit(52428800*2)] // 100MB
        public async Task<IActionResult> SaveProgSettings(string title, IFormFile logo)
        {


            var logoRoot = Path.Combine(_hostEnvironment.WebRootPath, "images/logo");
            //string n = string.Format("vid-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            //var fileName = n + "-" + video.FileName.Replace(" ", "-");

            if (logo != null)
            {
                if(logo.Length > 0){
                    var fileName = logo.FileName.Replace(" ", "-");
                    var fullFileName = Path.Combine(logoRoot, fileName);
                    using (var stream = new FileStream(fullFileName, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }

                    _data.SetSettingByName("LogoUrl", fileName);
                }

            }

            _data.SetSettingByName("Title", title);
            _data.Commit();

            return Success();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetRefresh(Boolean status)
        {
            _data.SetSettingByName("Refresh", status.ToString());
            return Success("Het scherm zal refreshen binnen 15 seconden");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetRefresh()
        {
            Boolean RefreshStatus = Convert.ToBoolean(_data.GetSettingByName("Refresh"));
            return Json(new {success = true, status = RefreshStatus, message="Gelukt"});
        }
    }

    public static class ExtensionMethods
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }

    public class TimeAction
    {
        public int Day { get; set; }
        public int Hour  { get; set; }
        public int Minutes { get; set; }
    }

    public class OperatingTimes
    {
        public int DeviceId { get; set; }
        public List<TimeAction> ScreenOnList { get; set; }
        public List<TimeAction> ScreenOffList { get; set; }
    }

}
