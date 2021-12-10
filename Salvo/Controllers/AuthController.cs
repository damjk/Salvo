using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using salvo.Models;
using salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IPlayerRepository _repository;

        public AuthController(IPlayerRepository repository)
        {
            _repository = repository;

        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] PlayerDTO player)
        {
            try
            {
                Player user = _repository.FindByEmail(player.Email);

                byte[] data = Encoding.ASCII.GetBytes(player.Password);
                data = new SHA256Managed().ComputeHash(data);
                string passwordHash = Encoding.ASCII.GetString(data);

                if (user == null || !String.Equals(user.Password, passwordHash))
                    return Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim("Player", user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                );

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
