using System.ComponentModel.DataAnnotations;

namespace WebApiRoleBasedAuthorization.Model
{
    public class Register
    {
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword {  get; set; } = string.Empty;

    }
}
