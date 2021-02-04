using System.ComponentModel.DataAnnotations;

namespace playground.DTOs
{
    public class PasswordResetDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        public string Key { get; set; }
    }
}
