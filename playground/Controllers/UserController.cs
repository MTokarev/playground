using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using playground.DTOs;
using playground.Entities;
using playground.Interfaces;
using playground.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace playground.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ICookiesService _cookiesService;
        private readonly ILogger<UserController> _logger;
        private readonly IActionKeyService _keyService;

        public UserController(IUserService userService, ITokenService tokenService,
            ICookiesService cookiesService, ILogger<UserController> logger,
            IActionKeyService keyService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _cookiesService = cookiesService;
            _logger = logger;
            _keyService = keyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EUser>>> GetAllUsers()
        {
            var allUsers = new AllUsersViewModel
            {
                Users = await _userService.GetAllUsersAsync()
            };

            // Setting username
            ViewData["loggedUserName"] = _cookiesService.GetUserName("X-Username", Request);

            // Return User view and pass model view
            return View("User", allUsers);
        }

        [Authorize(Roles = nameof(ERoles.Administrator))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ERole>>> GetAllRoleAssignments()
        {
            var allRoles = new AllRolesViewModel
            {
                Roles = await _userService.GetAllRoleAssignmentsAsync()
            };

            // Setting username
            ViewData["loggedUserName"] = _cookiesService.GetUserName("X-Username", Request);

            // Return User view and pass model view
            return View(allRoles);
        }

        [Authorize(Roles = nameof(ERoles.Administrator))]
        [HttpGet]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Redirect("GetAllUsers");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<UserWithTokenDto>> Register([FromForm] UserToRegisterDto userToRegister)
        {
            var registeredUser = await _userService.RegisterUserAsync(userToRegister);

            if (registeredUser.HasError)
            {
                // Showing modal view message
                ViewData["modalMessage"] = registeredUser.Message;

                return View();
            }

            return View("UserCreatedSuccesfully");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PasswordReset([FromQuery] string key)
        {
            if(key == null || !Guid.TryParse(key, out var validGuid))
            {
                TempData["modalMessage"] = "Invalid key provided. Try reset password again.";

                return Redirect("/error/showError");
            }
            var keyFromDbResult = await _keyService.GetKeyAsync(new Guid(key), removeKey: false);

            if (keyFromDbResult.HasError)
            {
                TempData["modalMessage"] = keyFromDbResult.Message;

                return Redirect("/error/showError");
            }
            var user = await _userService.GetUserByIdAsync(keyFromDbResult.UserId);

            if(user == null)
            {
                TempData["modalMessage"] = $"Unable to find user with id: '{keyFromDbResult.UserId}'";

                return Redirect("/error/showError");
            }

            // Setting email and key, it would be passed to the POST to do a second validation.
            ViewData["email"] = user.Email;
            ViewData["key"] = key;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordReset([FromForm] PasswordResetDto passwordResetDto)
        {
            if (passwordResetDto.Key == null || !Guid.TryParse(passwordResetDto.Key, out var validGuid))
            {
                TempData["modalMessage"] = "Invalid key provided. Try reset password again.";

                return Redirect("/error/showError");
            }
            var keyFromDbResult = await _keyService.GetKeyAsync(new Guid(passwordResetDto.Key), removeKey: false);

            if (keyFromDbResult.HasError)
            {
                TempData["modalMessage"] = keyFromDbResult.Message;

                return Redirect("/error/showError");
            }

            var result = await _userService.PasswordReset(passwordResetDto.Email, passwordResetDto.NewPassword);

            if(result.HasError)
            {
                TempData["modalMessage"] = result.Message;

                return Redirect("/error/showError");
            }

            return Redirect("/user/login");
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePasswordResetLink(string email)
        {
            var newKeyResult = await _keyService.GenerateKeyAsync(email);

            if (newKeyResult.HasError)
            {
                TempData["modalMessage"] = $"ERROR: {newKeyResult.Message}";

                return Redirect("/error/showError");
            }

            ViewData["modalMessage"] = "Please check your email for password reset link.";

            return Redirect("/home/index");
        }



        [HttpGet]
        public IActionResult Login([FromQuery] int statusCode)
        {
            if (statusCode == 401)
            {
                ViewData["modalMessage"] = "ERROR: Unauthorized access";
            }
            else if (statusCode == 403)
            {
                ViewData["modalMessage"] = "ERROR: User role is not enough to perform this operation";
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult<EUser>> Login([FromForm] UserToLoginDto userToLogin)
        {
            var loggedUser = await _userService.Login(userToLogin);

            // Check if user passed auth
            if (loggedUser.HasError)
            {
                // Show error to user
                ViewData["modalMessage"] = loggedUser.Message;

                return View();
            }

            // Baking JWT token
            string token = _tokenService.CreateToken(loggedUser.UserId, loggedUser.UserRoles);

            // Storing token and username in cookies
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            };

            // Constructing cookies to set
            var cookiesData = new Dictionary<string, string>()
            {
                ["X-Access-Token"] = token,
                ["X-Username"] = userToLogin.Email
            };

            // Setting cookies from data
            _cookiesService.SetCookies(cookiesData, cookieOptions, Response);

            return Redirect("/user/getAllUsers");
        }
    }
}
