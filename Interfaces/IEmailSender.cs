namespace FleetManage.Api.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }

    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body)
        {
            _logger.LogInformation(
                "DEV EMAIL\nTo: {To}\nSubject: {Subject}\nBody:\n{Body}",
                to, subject, body
            );

            return Task.CompletedTask;
        }
    }
}
