using System.ComponentModel.DataAnnotations;

namespace playground.DTOs
{
    public class UserToLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
