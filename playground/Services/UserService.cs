﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using playground.Data;
using playground.DTOs;
using playground.Entities;
using playground.Interfaces;
using playground.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace playground.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _dbcontext;
        private readonly ILogger<UserService> _logger;

        public UserService(DatabaseContext dbcontext, ILogger<UserService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<UserActionResult> RegisterUserAsync(UserToRegisterDto userToRegister, ERoles role = ERoles.User)
        {
            // Checking if user exist
            if (await CheckUserExistAync(userToRegister.Email))
            {
                string messageFailed = $"User with email '{userToRegister.Email}'";
                _logger.LogWarning(messageFailed);
                return InitResult(0, null, true, messageFailed);
            }

            // Adding crypto service to generate salt and use hashing
            using var hmac = new HMACSHA512();

            var user = new EUser
            {
                FirstName = userToRegister.FirstName,
                LastName = userToRegister.LastName,
                Email = userToRegister.Email.ToLower(),
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userToRegister.Password)),
                CreatedUTC = DateTime.UtcNow
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

            string messageSeccess = $"User '{userToRegister.Email}' has been registered.";
            _logger.LogInformation(messageSeccess);

            return InitResult(user.id, null, false, messageSeccess);
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

            string messageSuccess = $"User '{userToRemove.Email}' has been removed. '{result}' row affected.";
            _logger.LogInformation(messageSuccess);

            return InitResult(id, null, false, messageSuccess);
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

            if (String.IsNullOrEmpty(userToLogin.Email) || String.IsNullOrEmpty(userToLogin.Password))
            {
                string messageFailed = $"Email and password must not be empty.";
                _logger.LogWarning(messageFailed);
                
                result = InitResult(0, null, true, messageFailed);

                return result;
            }

            // if username not found
            if (user == null)
            {
                string messageFailed = $"Login failed for '{userToLogin.Email}'. Username or password incorrect.";
                _logger.LogWarning(messageFailed);

                result = InitResult(0, null, true, messageFailed);

                return result;
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userToLogin.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedHash[i])
                {
                    string messageFailed = $"Login failed for '{userToLogin.Email}'. Username or password incorrect.";
                    _logger.LogWarning(messageFailed);

                    result = InitResult(0, null, true, $"Login failed for '{userToLogin.Email}'. Username or password incorrect.");

                    return result;
                }
            }

            // Update login stats
            user.LastLoginUTC = DateTime.UtcNow;
            user.LoginsCount++;
            await _dbcontext.SaveChangesAsync();

            // Get user roles
            var userRoles = await GetUserRolesAsync(user.id);

            string messageSuccess = $"User '{userToLogin.Email}' has been succesfully logged in.";
            _logger.LogInformation(messageSuccess);

            result = InitResult(user.id, userRoles, false, messageSuccess);

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

        public async Task<UserActionResult> PasswordReset(string email, string password)
        {
            UserActionResult result;
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                string messageFailed = $"Unable to find user with email '{email}'.";
                _logger.LogWarning(messageFailed);
                result = InitResult(0, null, true, messageFailed);

                return result;
            }

            using var hmac = new HMACSHA512();

            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            user.LastUpdatedUTC = DateTime.UtcNow;

            var dbresult = await _dbcontext.SaveChangesAsync();

            if (dbresult == 0)
            {
                string messageFailed = $"Unable to save new password to database for user: '{email}'.";
                _logger.LogWarning(messageFailed);

                result = InitResult(0, null, true, messageFailed);

                return result;
            }

            string messageSuccess = $"Password for user: '{user.Email}' has been update.";
            _logger.LogInformation(messageSuccess);

            return InitResult(user.id, user.Roles, false, messageSuccess);
        }
    }
}
