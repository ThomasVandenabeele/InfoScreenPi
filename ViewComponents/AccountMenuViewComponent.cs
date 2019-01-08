using InfoScreenPi.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using InfoScreenPi.Infrastructure;
using Microsoft.AspNetCore.Http;
using InfoScreenPi.Entities;
using InfoScreenPi.Infrastructure.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.DataProtection;

namespace InfoScreenPi.ViewComponents
{
    public class AccountMenuViewComponent : ViewComponent
    {
        private readonly IDataService _data;
        private readonly IDataProtector _protector;

        public AccountMenuViewComponent(IDataService dataService, IDataProtectionProvider dataProtectionProvider)
        {
            _data = dataService;
            _protector = dataProtectionProvider.CreateProtector("cookies");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string idString = "";
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                idString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            }
            //string idString = _protector.Unprotect(HttpContext.Request.Cookies["YU2ert-gert24-59HEHF-thtyyE-87R23!"]); // id
            int? id = Convert.ToInt32(idString);

            if (id != null && id != 0)
            {
                User gebruiker = await _data.GetSingleAsync<User>((int)id);
                return View(new AccountViewModel(gebruiker));
            }
            else return View();
        }
    }
}
