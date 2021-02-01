using Microsoft.EntityFrameworkCore;
using playground.Data;
using playground.DTOs;
using playground.Entities;
using playground.Interfaces;
using playground.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace playground.Services
{
    public class UserService : IUserService
    {
        private DatabaseContext _dbcontext;

        public UserService(DatabaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<UserActionResult> RegisterUserAsync(UserToRegisterDto userToRegister, ERoles role = ERoles.User)
        {
            // Checking if user exist
            if (await CheckUserExistAync(userToRegister.Email))
            {
                return new UserActionResult()
                {
                    UserId = 0,
                    HasError = true,
                    Message = $"User with email '{userToRegister.Email}' already exist."
                };
            }

            // Adding crypto service to generate salt and use hashing
            using var hmac = new HMACSHA512();

            var user = new EUser
            {
                FirstName = userToRegister.FirstName,
                LastName = userToRegister.LastName,
                Email = userToRegister.Email.ToLower(),
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userToRegister.Password))
            };

            // Create user
            await _dbcontext.Users.AddAsync(user);
            await _dbcontext.SaveChangesAsync();

            var userRole = new ERole
            {
                EUserId = user.id,
                Role = userToRegister.Email == "admin@localhost" ? ERoles.Administrator : ERoles.User,
                EUser = user
            };

            // Assign user role
            await _dbcontext.Roles.AddAsync(userRole);
            await _dbcontext.SaveChangesAsync();

            return new UserActionResult()
            {
                UserId = user.id,
                HasError = false,
                Message = $"User '{userToRegister.Email}' has been registered."
            };
        }

        public async Task<IEnumerable<EUser>> GetAllUsersAsync()
        {
            return await _dbcontext.Users.ToListAsync<EUser>();
        }

        public async Task<IEnumerable<ERole>> GetAllRoleAssignmentsAsync()
        {
            return await _dbcontext.Roles.ToListAsync<ERole>();
        }

        public async Task<bool> CheckUserExistAync(string email)
        {
            return await _dbcontext.Users.AnyAsync<EUser>(x => x.Email == email.ToLower());
        }

        public async Task<UserActionResult> DeleteUserAsync(int id)
        {
            EUser userToRemove = await GetUserByIdAsync(id);
            _dbcontext.Users.Remove(userToRemove);

            // This action will break on Failure and return task result with Status: Faulted
            var result = await _dbcontext.SaveChangesAsync();

            return new UserActionResult()
            {
                UserId = id,
                HasError = false,
                Message = $"User '{userToRemove.Email}' has been removed. '{result}' row affected."
            };
        }

        public async Task<EUser> GetUserByIdAsync(int id)
        {
            return await _dbcontext.Users.SingleOrDefaultAsync<EUser>(x => x.id == id);
        }

        public async Task<EUser> GetUserByEmailAsync(string email)
        {
            return await _dbcontext.Users.SingleOrDefaultAsync<EUser>(x => x.Email == email);
        }

        public async Task<UserActionResult> Login(UserToLoginDto userToLogin)
        {
            var user = await GetUserByEmailAsync(userToLogin.Email);
            UserActionResult result;

            // if username not found
            if (user == null)
            {
                result = InitResult(0, null, true, $"Login failed for '{userToLogin.Email}'. Username or password incorrect.");

                return result;
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userToLogin.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedHash[i])
                {
                    result = InitResult(0, null, true, $"Login failed for '{userToLogin.Email}'. Username or password incorrect.");

                    return result;
                }
            }

            // Get user roles
            var userRoles = await GetUserRolesAsync(user.id);

            result = InitResult(user.id, userRoles, false, $"User '{userToLogin.Email}' has been succesfully logged in.");

            return result;
        }

        private async Task<IEnumerable<ERole>> GetUserRolesAsync(int userId)
        {
            return await _dbcontext.Roles.Where(x => x.EUserId == userId).ToListAsync<ERole>();
        }

        private UserActionResult InitResult(int id, IEnumerable<ERole> roles, bool hasError, string message)
        {
            return new UserActionResult()
            {
                UserId = id,
                UserRoles = roles,
                HasError = hasError,
                Message = message
            };
        }
    }
}
