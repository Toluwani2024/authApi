using authapi.DTOs;
using authapi.IServices;
using authapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authapi.Servicess
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<(bool success, string message)> RegisterUserAsync(UserSignupDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
            {
                return (false, "user with this email already exists");
            }
            var newuser = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var iscreated = await _userManager.CreateAsync(newuser, request.Password);

            if (iscreated.Succeeded)
            {
                return (true, "user created successfully");
            }
            return (false, "creation failed");
        }
        public async Task<(bool success, string message, string token)> LoginUserAsync(UserLoginDto request)
        {

            var existinguser = await _userManager.FindByEmailAsync(request.Email);
            if (existinguser == null)
            {
                return (false, "user not found", null);
            }
            var isCorrect = await _userManager.CheckPasswordAsync(existinguser, request.Password);
            if (isCorrect)
            {
                return (false, "wrong credentials", null);
            }

            var token = GeneratejwtToken(existinguser);
            return (true, "login successful", token);
        }

        private string GeneratejwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + "" +user.LastName)
            };
            var token = new JwtSecurityToken
          (
                _configuration["Jwt:Issues"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
              );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    } }
