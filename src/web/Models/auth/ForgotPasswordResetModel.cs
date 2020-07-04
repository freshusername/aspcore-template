using System.ComponentModel.DataAnnotations;

namespace web.Models.auth
{
    public class ForgotPasswordResetModel
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
