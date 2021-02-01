using playground.Entities;
using System.Collections.Generic;

namespace playground.Interfaces
{
    /// <summary>
    /// Interface for jwt token service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Returns token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>String token</returns>
        public string CreateToken(int id, IEnumerable<ERole> roles);
    }
}
