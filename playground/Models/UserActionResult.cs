using playground.Entities;
using System.Collections.Generic;

namespace playground.Models
{
    public class UserActionResult
    {
        public int UserId { get; set; }
        public IEnumerable<ERole> UserRoles { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; }
    }
}
