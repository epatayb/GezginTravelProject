using GezginTravel.Constants;

namespace GezginTravel.ViewModels.Dashboard.Admin.AdminUser
{
    public class AdminUserListItemViewModel
    {
        public int Id { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? Title {  get; set; }
        public string? About { get; set; }

        public string PhotoUrl { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();
        public string PrimaryRole { get; set; } = RoleConstants.User;

        public string RoleText => Roles.Any()
            ? string.Join(", ", Roles) 
            : "Rol Bulunamadı";

        public int TotalBlogs { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public decimal PopularityScore { get; set; }

        public bool IsSuspended { get; set; }
    }
}
