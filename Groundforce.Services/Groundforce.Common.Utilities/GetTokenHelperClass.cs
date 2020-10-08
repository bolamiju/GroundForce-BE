﻿using Groundforce.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Groundforce.Common.Utilities
{
    public class GetTokenHelperClass
    {

        public string GetToken(ApplicationUser _user, IConfiguration _config)
        {
            //get application user model
            var user = _user;
            var config = _config;
            //Create claim for JWT
            var claims = new Claim[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim (ClaimTypes.Name, user.FirstName),
                 new Claim(ClaimTypes.Email, user.Email)
            };

            //Create jwt secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

            //Generate signin creadentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create security token descriptor
            var securityTokenDescribe = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //build token
            var handleToken = new JwtSecurityTokenHandler();

            SecurityToken token = handleToken.CreateToken(securityTokenDescribe);

            return handleToken.WriteToken(token);
        }

    }
}
