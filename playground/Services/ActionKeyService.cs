using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using playground.Data;
using playground.Entities;
using playground.Interfaces;
using playground.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace playground.Services
{
    public class ActionKeyService : IActionKeyService
    {
        private readonly DatabaseContext _dbcontext;
        private readonly IUserService _userService;
        private readonly ILogger<ActionKeyService> _logger;
        private readonly IEmailService _emailService;

        public ActionKeyService(DatabaseContext dbcontext, IUserService userService,
            ILogger<ActionKeyService> logger, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _userService = userService;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<KeyActionResult> GenerateKeyAsync(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                string messageFailed = $"Unable to find user with email: '{email}'.";
                _logger.LogWarning(messageFailed);

                return InitResult(0, true, messageFailed, Guid.Empty);
            }

            var newActionKey = new EUserActionKey
            {
                ActionKey = Guid.NewGuid(),
                EUser = user,
                UserId = user.id,
                CreatedUTC = DateTime.UtcNow
            };

            await _dbcontext.UserActionKeys.AddAsync(newActionKey);
            int rowAffected = await _dbcontext.SaveChangesAsync();

            if (rowAffected == 0)
            {
                string messageFailed = $"Unable to save key for user with email: '{email}'.";
                _logger.LogError(messageFailed);

                return InitResult(0, true, messageFailed, Guid.Empty);
            }

            string messageSuccess = $"New key for user '{user.Email}' has been generated.";
            _logger.LogInformation(messageSuccess);

            var emailBody = new StringBuilder();
            emailBody.Append($"<h3>Dear {user.Email},</h3>");
            emailBody.Append("Password reset for your account was requested. ");
            emailBody.Append($"Click <a href = 'https://localhost:5001/user/passwordReset?key={newActionKey.ActionKey.ToString()}'>here </a>");
            emailBody.Append("to change your password.");

            await _emailService.SendEmailAsync(new List<string> { user.Email }, "Password reset requested", emailBody.ToString());

            return InitResult(user.id, false, messageSuccess, newActionKey.ActionKey);

        }

        public async Task<KeyActionResult> GetKeyAsync(Guid key, bool removeKey = false)
        {
            var keyFromDb = await _dbcontext.UserActionKeys.SingleOrDefaultAsync(k => k.ActionKey == key);
            if (keyFromDb == null)
            {
                string messageFailed = $"Unable to find key in the databse.";
                _logger.LogWarning(messageFailed);

                return InitResult(0, true, messageFailed, Guid.Empty);
            }

            // Remove key if requested.
            if (removeKey)
            {
                _dbcontext.UserActionKeys.Remove(keyFromDb);
                await _dbcontext.SaveChangesAsync();
            }

            string messageSuccess = $"Password reset key was accessed: '{key.ToString()}'";
            _logger.LogInformation(messageSuccess);

            return InitResult(keyFromDb.UserId, false, messageSuccess, key);
        }

        private KeyActionResult InitResult(int id, bool hasError, string message, Guid key)
        {
            return new KeyActionResult
            {
                UserId = id,
                HasError = hasError,
                Message = message,
                Key = key
            };
        }
    }
}
