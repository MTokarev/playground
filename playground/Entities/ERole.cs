using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
