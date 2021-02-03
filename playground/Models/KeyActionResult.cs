using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace playground.Models
{
    public class KeyActionResult
    {
        public int UserId { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; }
        public Guid Key { get; set; }
    }
}
