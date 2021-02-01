using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using playground.DTOs;
using playground.Entities;
using playground.Interfaces;
using playground.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace playground.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userService;
        private ITokenService _tokenService;
        private ICookiesService _cookiesService;

        public UserController(IUserService userService, ITokenService tokenService
            , ICookiesService cookiesService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _cookiesService = cookiesService;
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

        [Authorize(Roles= nameof(ERoles.Administrator))]
        [HttpGet]
        public async Task<IActionResult> DeleteUser([FromQuery]int userId)
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
        public IActionResult Login([FromQuery]int statusCode)
        {
            if(statusCode == 401)
            {
                ViewData["modalMessage"] = "ERROR: Unauthorized access";
            }else if(statusCode == 403)
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
