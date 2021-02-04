using System;

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
