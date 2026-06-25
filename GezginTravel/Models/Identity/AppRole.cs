using Microsoft.AspNetCore.Identity;

namespace GezginTravel.Models.Identity;

public class AppRole : IdentityRole<int>
{
    public string FullName { get; set; } = null!;
}