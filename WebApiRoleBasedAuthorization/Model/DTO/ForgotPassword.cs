using System.ComponentModel.DataAnnotations;

namespace WebApiRoleBasedAuthorization.Model.DTO
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string? ClientUri { get; set; }
    }



}
