using System.ComponentModel.DataAnnotations;

namespace playground.Entities
{
    public class ERole
    {
        [Key]
        public int Id { get; set; }
        public int EUserId { get; set; }
        public ERoles Role { get; set; }
        public EUser EUser { get; set; }
    }
}
