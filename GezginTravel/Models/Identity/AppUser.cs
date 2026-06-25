using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using GezginTravel.Models.Entities;

namespace GezginTravel.Models.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Title { get; set; }
        public string About { get; set; }

        public string PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public bool IsEmailVerified { get; set; }

        public decimal PopularityScore { get; set; }

        public bool IsDeleted { get; set; } // Soft delete flag
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<UserFollow> Followers { get; set; }
        public ICollection<UserFollow> Following { get; set; }

    }
}