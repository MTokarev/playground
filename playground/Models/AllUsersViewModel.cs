using playground.Entities;
using System.Collections.Generic;

namespace playground.Models
{
    public class AllUsersViewModel
    {
        public IEnumerable<EUser> Users { get; set; }
    }
}
