using playground.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace playground.Models
{
    public class AllUsersViewModel
    {
        public IEnumerable<EUser> Users { get; set; }
    }
}
