using System;
using System.ComponentModel.DataAnnotations;

namespace playground.Entities
{
    public class EUserActionKey
    {
        [Key]
        public int Id { get; set; }

        public Guid ActionKey { get; set; }
        public int UserId { get; set; }
        public EUser EUser { get; set; }
        public DateTime CreatedUTC { get; set; }
    }
}
