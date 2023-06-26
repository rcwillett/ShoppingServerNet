using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingApplicationAPINET.Models;
using Microsoft.EntityFrameworkCore;
using ShoppingApplicationAPINET.Types;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingApplicationAPINET.Controllers
{
    public class AuthController : Controller
    {
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

        public AuthController (ShoppingContext shoppingContext)
        {
            _context = shoppingContext;
            _hashAlgorithm = SHA256.Create();
        }

        [HttpPost]
        [Route("/auth/register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                return Ok();
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/auth/login")]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(ILoginRequestBody login)
        {
            try
            {
                User? user = await _context.Users.FindAsync(login.username);
                if (user == null)
                {
                    throw new Exception("Failed to find user with that username");
                }
                string hash = _GenerateHash(login.password);
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                if (stringComparer.Compare(user.Password_Hash, hash) == 0)
                {
                    return Ok();
                }
                throw new Exception("Invalid password");
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}

