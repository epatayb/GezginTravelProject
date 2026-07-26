namespace GezginTravel.Services.Email
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(
            string templateName,
            Dictionary<string, string> values);
    }
}
