using Microsoft.AspNetCore.Mvc;

namespace playground.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult ShowError([FromQuery] int errorCode)
        {
            if (errorCode == 401)
            {
                ViewData["modalMessage"] = "ERROR: Unauthorized access";
            }
            else if (errorCode == 403)
            {
                ViewData["modalMessage"] = "ERROR: User role is not enough to perform this operation";
            }

            TempData.TryGetValue("modalMessage", out var modalMessage);
            ViewData["modalMessage"] = modalMessage;

            return View();
        }
    }
}
