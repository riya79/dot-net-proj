using EmailServiceLibrary.Model;
using EmailServiceLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebApiRoleBasedAuthorization.Model;
using WebApiRoleBasedAuthorization.Model.DTO;
using static System.Net.WebRequestMethods;


namespace WebApiRoleBasedAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }
        /*  [HttpPost("register")]
          public async Task<IActionResult> Register([FromBody] Register model)
          {
              if (!ModelState.IsValid)
              {
                  return BadRequest(ModelState);
              }

              var user = new IdentityUser { UserName = model.Username, Email = model.Email };
              var result = await _userManager.CreateAsync(user, model.Password);

              if (result.Succeeded)
              {
                  try
                  {
                      // Generate the email confirmation token
                      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                      var param = new Dictionary<string, string>
                      {
                          { "token", token },
                          { "email", model.Email }
                      };

                      // Construct the email confirmation URL
                      var callbackUrl = QueryHelpers.AddQueryString("https://localhost:7046/api/Account/ConfirmEmail", param);
                      // Create a message with the token included
                      var message = new Message(new string[] { user.Email }, "Confirm your email", $"Please confirm your account by using the following token: {token}");
                      await _emailSender.SendEmailAsync(message);
                  }
                  catch (Exception ex)
                  {
                      // Log the exception and return a generic error message
                      // Logger.LogError(ex, "Error sending email confirmation.");
                      return StatusCode(500, "Internal server error. Please try again later.");
                  }

                  return Ok("User registered successfully. Please check your email to confirm your account.");
              }

              return BadRequest(result.Errors);
          }
  */

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Error confirming your email.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
            {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                    new Claim("id", user.Id)
                };

                    authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                            SecurityAlgorithms.HmacSha256)
                    );
                   

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), username = user.UserName, roles = userRoles , userId = user.Id});
                }

                return Unauthorized("Invalid username or password.");
            
            }
         catch (Exception ex)
          {
              return StatusCode(StatusCodes.Status500InternalServerError, $"Error logging in: {ex.Message}");
          }
}

    [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("Invalid request");
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var param = new Dictionary<string, string>
            {
                { "token", token },
                { "email", forgotPassword.Email }
            };
            // Construct callback URL for reset password
            var callbackUrl = QueryHelpers.AddQueryString(forgotPassword.ClientUri!, param);
            // Create email message
            var message = new Message(new string[] { user.Email }, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            // Send email asynchronously
            await _emailSender.SendEmailAsync(message);

            return Ok(new
            {
                token = token,
                email = user.Email
            });
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return BadRequest("Invalid request");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token!, resetPassword.Password!);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            return Ok("Password reset successful");
        }

/*
        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> logout()
        {
            string rawuserId = HttpContext.User.FindFirstValue("id");
            if (!Guid.TryParse(rawuserId, out Guid id))
            {
                return Unauthorized();
            }
            return NoContent();

        }
*/
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully." });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Role already exists");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role Assigned successfully." });
            }

            return BadRequest(result.Errors);
        }
       
    }
}
