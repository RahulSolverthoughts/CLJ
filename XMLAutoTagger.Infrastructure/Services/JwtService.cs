using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using XMLAutoTagger.Application.Common.Interfaces;
using XMLAutoTagger.Domain.Entities;

namespace XMLAutoTagger.Infrastructure.Services
{

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.FullName) ? $"{user.FirstName} {user.LastName}".Trim() : user.FullName),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("role_code", user.Role?.RoleCode ?? string.Empty),
                new Claim("department_id", user.DepartmentId?.ToString() ?? string.Empty),
                new Claim("department_name", user.Department?.DepartmentName ?? string.Empty),
                new Claim("avatar_initials", user.AvatarInitials ?? string.Empty),
                new Claim("avatar_colour_hex", user.AvatarColourHex ?? "#185FA5")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
