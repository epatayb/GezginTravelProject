namespace GezginTravel.Services.Email
{
    public interface IEmailService
    {
        Task SendMailAsync(
            string to,
            string subject,
            string htmlBody,
            string? plainTextBody = null);
    }
}
