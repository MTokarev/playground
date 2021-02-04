using System.Collections.Generic;
using System.Threading.Tasks;

namespace playground.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="sendTo"></param>
        /// <param name="subject"></param>
        /// <param name="messageBody"></param>
        /// <param name="bccTo"></param>
        /// <param name="ccTo"></param>
        /// <returns>Return Task</returns>
        Task SendEmailAsync(ICollection<string> sendTo, string subject, string messageBody,
            ICollection<string>? bccTo = null, ICollection<string>? ccTo = null);
    }
}
