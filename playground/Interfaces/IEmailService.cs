using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace playground.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(ICollection<string> sendTo, string subject, string messageBody,
            ICollection<string>? bccTo = null, ICollection<string>? ccTo = null);
    }
}
