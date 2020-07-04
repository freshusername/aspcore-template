using System.ComponentModel.DataAnnotations;

namespace web.Models.auth
{
    public class LoginModel
    {
        [Required]
        public string UnameOrEmail { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
