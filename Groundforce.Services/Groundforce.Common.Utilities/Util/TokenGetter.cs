using Groundforce.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Groundforce.Common.Utilities
{
    public class TokenGetter
    {
        public string GetToken(ApplicationUser user, IConfiguration _config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //Create claims for JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name, user.LastName)
                };

            //Get JWT secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("TokenAppSettings:Token").Value));

            //Generate Signing Credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create Security Token Descriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,
            };

            //Build Token Handler


            //Create Token
            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            //return Token
            return tokenHandler.WriteToken(token);

        }
    }
}
