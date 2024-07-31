using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using WebApiRoleBasedAuthorization.Model.DTO;

namespace WebApiRoleBasedAuthorization.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(
            UserManager<IdentityUser> userManager)

        {
            _userManager = userManager;

        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accesed the user controller");
        }




        [HttpGet("userdetails/{username}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return NotFound();
                }

                var email = user.Email;

                var responseDto = new ResponseDto
                {
                    Username = username,
                    Email = email
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving user details: {ex.Message}");
            }
        }

        [HttpGet("userinfo/{userid}")]
        public async Task<IActionResult> GetUserById([FromRoute] string userid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid);
                if (user == null)
                {
                    return NotFound();
                }

                var responseDto = new ResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving user details: {ex.Message}");
            }
        }

        [HttpPost("editUserDetails/{userId}")]
        public async Task<IActionResult> UpdateUserDetails([FromRoute] string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var responseDto = new ResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving user details: {ex.Message}");
            }
        }

        [HttpPost("updateUserById")]
        public async Task<IActionResult> UpdateUserById([FromBody] ResponseDto model)
        {
            try
            {

                // Find the user by ID
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                user.UserName = model.Username;
                user.Email = model.Email;

                // Save the changes
                var result = await _userManager.UpdateAsync(user);
                var responseDto = new ResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                };

                if (result.Succeeded)
                {
                    return Ok(responseDto.Id);
                }
                else
                {
                    return BadRequest("Failed to update user details.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user details: {ex.Message}");
            }
        }

        // New endpoint to get all users
        [HttpGet("allusers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userManager.Users.Select(u => new ResponseDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email
                }).ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving users: {ex.Message}");
            }
        }
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting user");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting user: {ex.Message}");
            }
        }





    }
}


