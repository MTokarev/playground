using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace imakler.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult ShowError([FromQuery] int errorCode)
        {
            // If request came with status code then add error
            if (errorCode == 401)
            {
                string messageFailed = "ERROR: Unauthorized access";
                ViewData["modalMessage"] = messageFailed;
                _logger.LogError(messageFailed);

                return View();
            }
            else if (errorCode == 403)
            {
                string messageFailed = "ERROR: User role is not enough to perform this operation";
                ViewData["modalMessage"] = messageFailed;
                _logger.LogError(messageFailed);

                return View();
            }

            // If request was redirected with temp data then read and show
            if (TempData.TryGetValue("modalMessage", out var modalMessage))
            {
                ViewData["modalMessage"] = modalMessage;
                _logger.LogError(modalMessage.ToString());
            }

            return View();
        }
    }
}
