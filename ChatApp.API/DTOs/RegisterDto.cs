using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength:8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
