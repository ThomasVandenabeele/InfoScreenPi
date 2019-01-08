using Microsoft.AspNetCore.Mvc;
using InfoScreenPi.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;

namespace InfoScreenPi.Controllers
{
    public abstract class BaseController : Controller
    {
      protected readonly IDataService _data;
      protected readonly IHostingEnvironment _hostEnvironment;

      public BaseController(IDataService dataService,
                            IHostingEnvironment hostEnvironment)
      {
          _data = dataService;
          _hostEnvironment = hostEnvironment;
      }

      private JsonResult SimpleJsonResponse(bool succ, string msg)
      {
          return Json(new { success = succ, message = (msg) });
      }
      protected JsonResult Success(string msg)
      {
          return SimpleJsonResponse(true, msg);
      }
      protected JsonResult Success()
      {
        return Success("Success!");
      }
      protected JsonResult Fail(string msg)
      {
          return SimpleJsonResponse(false,msg);
      }
      protected JsonResult Fail()
      {
          return Fail("Something went wrong.");
      }
    }
}
