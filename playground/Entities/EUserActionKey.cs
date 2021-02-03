using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace playground.Entities
{
    public class EUserActionKey
    {
        [Key]
        public int Id { get; set; }

        public Guid ActionKey { get; set; }
        public int UserId { get; set; }
        public EUser EUser { get; set; }
    }
}
