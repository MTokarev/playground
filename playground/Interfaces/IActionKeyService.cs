using playground.Models;
using System;
using System.Threading.Tasks;

namespace playground.Interfaces
{
    public interface IActionKeyService
    {
        /// <summary>
        /// Generate action keys that required for some task like password reset
        /// </summary>
        /// <param name="email"></param>
        /// <returns>KeyActionResult</returns>
        Task<KeyActionResult> GenerateKeyAsync(string email);

        /// <summary>
        /// Gets ket from database and optionally removes it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removeKey"></param>
        /// <returns>KeyActionResult</returns>
        Task<KeyActionResult> GetKeyAsync(Guid key, bool removeKey = false);
    }
}
