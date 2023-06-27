using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingApplicationAPINET.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ShoppingApplicationAPINET.Controllers
{
    public class LoginRequestBody
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RegisterRequestBody
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class AuthController : Controller
    {
        private IConfiguration _configuration;

        private readonly ShoppingContext _context;

        private SHA256 _hashAlgorithm;

        private string _GenerateHash(string passwordInput)
        {
            byte[] data = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(passwordInput));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public AuthController(ShoppingContext shoppingContext, IConfiguration configuration)
        {
            _context = shoppingContext;
            _hashAlgorithm = SHA256.Create();
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/auth/register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterRequestBody body)
        {
            try
            {
                User newUser = new User();
                newUser.User_Name = body.username;
                newUser.Password_Hash = _GenerateHash(body.password);
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/auth/login")]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(
            [FromBody] LoginRequestBody body
        )
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync((user) => String.Compare(user.User_Name, body.username) == 0);
                if (user == null)
                {
                    throw new Exception("Failed to find user with that username");
                }
                string hash = _GenerateHash(body.password);
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                if (stringComparer.Compare(user.Password_Hash, hash) == 0)
                {
                    List<Claim> claims = new List<Claim>
                        {
                            new Claim("User_ID", user.User_ID.ToString()),
                        };
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    return Ok();
                }
                throw new Exception("Invalid password");
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/auth/logout")]
        public async Task<ActionResult> Logout ()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}

