namespace GezginTravel.Services.Email
{
    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly IWebHostEnvironment _environment;

        public EmailTemplateRenderer(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> values)
        {
            var templatePath = Path.Combine(
                _environment.ContentRootPath,
                "EmailTemplates",
                $"{templateName}.html"
                );

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template bulunamadı: {templateName}");
            }

            var html = await File.ReadAllTextAsync(templatePath);

            foreach (var value in values)
            {
                html = html.Replace($"{{{{{value.Key}}}}}", value.Value);
            }

            return html;
        }
    }
}
