using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
