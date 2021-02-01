using System.ComponentModel.DataAnnotations;

namespace playground.DTOs
{
    public class UserWithTokenDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
