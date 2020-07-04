using System.ComponentModel.DataAnnotations;

namespace web.Models.auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string ReturnUrl { get; set; } = null!;
    }
}
