using Microsoft.AspNetCore.Mvc;

namespace playground.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult ShowError([FromQuery] int errorCode)
        {
            // If request came with status code then add error
            if (errorCode == 401)
            {
                ViewData["modalMessage"] = "ERROR: Unauthorized access";

                return View();
            }
            else if (errorCode == 403)
            {
                ViewData["modalMessage"] = "ERROR: User role is not enough to perform this operation";

                return View();
            }

            // If request was redirected with temp data then read and show
            if (TempData.TryGetValue("modalMessage", out var modalMessage))
            {
                ViewData["modalMessage"] = modalMessage;
            }

            return View();
        }
    }
}
