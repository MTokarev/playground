using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace playground.OptionsConfig
{
    public class SendgridOptions
    {
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}
