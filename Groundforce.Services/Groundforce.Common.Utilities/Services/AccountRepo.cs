using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Groundforce.Common.Utilities.Services
{
    public class AccountRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AccountRepo(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public  string GetToken(string id, string Lastname)
        {
            // Create claims for JWT
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Name, Lastname)
            };

            // Get JWT Secret Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Generate the Signing Credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create Security Token Descriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            // Build Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the token
            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
