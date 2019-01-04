using Microsoft.AspNetCore.Mvc;

namespace InfoScreenPi.Controllers
{
    public abstract class BaseController : Controller
    {
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
