using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace playground.Entities
{
    public class EUser
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "First name required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email name required")]
        public string Email { get; set; }
        public int LoginsCount { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public List<ERole> Roles { get; set; }
        public List<EUserActionKey> UserActionKeys { get; set; }
        public DateTime LastLoginUTC { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime LastUpdatedUTC { get; set; }
    }
}
