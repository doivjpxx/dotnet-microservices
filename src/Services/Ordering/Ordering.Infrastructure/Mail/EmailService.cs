using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ordering.Infrastructure.Mail;

public class EmailService : IEmailService
{
    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    private EmailSettings _emailSettings { get; }
    private ILogger<EmailService> _logger { get; }

    public async Task<bool> SendEmail(Email email)
    {
        var client = new SendGridClient(_emailSettings.ApiKey);

        var from = new EmailAddress
        {
            Email = _emailSettings.FromAddress,
            Name = _emailSettings.FromName
        };

        var subject = email.Subject;
        var to = new EmailAddress(email.To);
        var body = email.Body;

        var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        var response = await client.SendEmailAsync(sendGridMessage);
        
        _logger.LogInformation("Email sent");

        if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }
        
        _logger.LogError("Something went wrong");

        return false;

    }
}