using System.ComponentModel.DataAnnotations;

namespace web.Models.auth
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        public string? Name { get; set; }
    }
}
