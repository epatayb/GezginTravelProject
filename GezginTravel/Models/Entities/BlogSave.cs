using System;
using GezginTravel.Models.Identity;

namespace GezginTravel.Models.Entities
{
    public class BlogSave : BaseEntity
    {
        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public int UserId { get; set; }
        public AppUser User { get; set; }
    }
}