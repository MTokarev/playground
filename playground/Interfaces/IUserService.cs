using playground.DTOs;
using playground.Entities;
using playground.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace playground.Interfaces
{
    /// <summary>
    /// Interface for user service
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Register user and return UserActionResult object with operation result and id
        /// By default user will be created with "User" role
        /// </summary>
        /// <param name="userToRegister"></param>
        /// <returns>Operation result DTO</returns>
        Task<UserActionResult> RegisterUserAsync(UserToRegisterDto userToRegister, ERoles role = ERoles.User);

        /// <summary>
        /// Check if user exist by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns bool</returns>
        Task<bool> CheckUserExistAync(string email);

        /// <summary>
        /// Return list of all users
        /// </summary>
        /// <returns>Returns list of users</returns>
        Task<IEnumerable<EUser>> GetAllUsersAsync();

        /// <summary>
        /// Return list of all role assignemts
        /// </summary>
        /// <returns>Returns list of users</returns>
        Task<IEnumerable<ERole>> GetAllRoleAssignmentsAsync();

        /// <summary>
        /// Delete user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Operation result DTO</returns>
        Task<UserActionResult> DeleteUserAsync(int id);

        /// <summary>
        /// Log user in by username and password
        /// </summary>
        /// <param name="userToLogin"></param>
        /// <returns>Operation result DTO</returns>
        Task<UserActionResult> Login(UserToLoginDto userToLogin);

        /// <summary>
        /// Return user, query by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns user</returns>
        Task<EUser> GetUserByIdAsync(int id);

        /// <summary>
        /// Return user, query by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns user</returns>
        Task<EUser> GetUserByEmailAsync(string email);
    }
}