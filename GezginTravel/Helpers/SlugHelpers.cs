using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace GezginTravel.Helpers
{
    public class SlugHelpers
    {
        public static string GenerateSlug(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = text.Trim().ToLowerInvariant();

            text = text
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ç", "c");

            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized) 
            { 
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            text = builder.ToString().Normalize(NormalizationForm.FormC);

            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-");
            text = Regex.Replace(text, @"-+", "-");

            return text.Trim('-');
        }
    }
}
