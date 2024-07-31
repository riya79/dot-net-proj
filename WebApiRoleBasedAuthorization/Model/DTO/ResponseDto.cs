using System.ComponentModel.DataAnnotations;

namespace WebApiRoleBasedAuthorization.Model.DTO
{
    public class ResponseDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username can't be longer than 50 characters.")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
