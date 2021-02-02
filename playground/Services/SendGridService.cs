using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using playground.Interfaces;
using playground.OptionsConfig;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace playground.Services
{
    public class SendGridService : IEmailService
    {
        private readonly IOptions<SendgridOptions> _options;
        private readonly ILogger<SendGridService> _logger;

        public SendGridService(IOptions<SendgridOptions> options, ILogger<SendGridService> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task SendEmailAsync(ICollection<string> sendTo, string subject,
            string messageBody, ICollection<string>? bccTo = null, ICollection<string>? ccTo = null)
        {
            await Execute(sendTo, subject, messageBody, bccTo, ccTo);
        }

        private async Task<Response> Execute(ICollection<string> sendTo, string subject,
            string messageBody, ICollection<string> bccTo, ICollection<string> ccTo)
        {
            if (_options.Value.ApiKey == null)
            {
                _logger.LogError($"Attempt to send email while 'appsettings.json' doesn't have 'SendGrid' directive.");
                return null;
            }

            var mailFrom = new EmailAddress(_options.Value.SenderEmail, _options.Value.SenderName);

            var client = new SendGridClient(_options.Value.ApiKey);
            
            var mail = new SendGridMessage
            {
                From = mailFrom,
                Subject = subject,
                PlainTextContent = messageBody,
                HtmlContent = messageBody
            };

            mail.AddTos(sendTo.Select(x => new EmailAddress { Email = x }).ToList());
            
            if(ccTo != null)
            {
                mail.AddCcs(ccTo.Select(x => new EmailAddress { Email = x }).ToList());
            }

            if(bccTo != null)
            {
                mail.AddBccs(bccTo.Select(x => new EmailAddress { Email = x }).ToList());
            }

            var result = await client.SendEmailAsync(mail);
            
            string logString = $@"
                    Message Subject '{subject}'.
                    {(sendTo != null ? "To: " + string.Join(",", sendTo) : String.Empty)}
                    {(ccTo != null ? " CC: " + ccTo : String.Empty)}
                    {(bccTo != null ? " BCC: " + bccTo : String.Empty)}";

            if (result.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogError($"Unable to send email. Status code '{result.StatusCode}'. {logString}");
            } else if(result.StatusCode == HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Mail has been submitted. {logString}");
            }

            return result;
        }

    }
}
