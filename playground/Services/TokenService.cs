using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using playground.Entities;
using playground.Interfaces;
using playground.OptionsConfig;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace playground.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly CryptoOptions _cryptoOptions;

        public TokenService(IOptions<CryptoOptions> cryptoOptions)
        {
            _cryptoOptions = cryptoOptions.Value;
            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_cryptoOptions.Key)
                );
        }

        public string CreateToken(int id, IEnumerable<ERole> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, _cryptoOptions.ValidAudience)

            };

            // Adding roles
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Role.ToString())));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_cryptoOptions.TokenExpirationInDays),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
