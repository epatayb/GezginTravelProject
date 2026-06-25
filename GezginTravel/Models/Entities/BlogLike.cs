using System;
using GezginTravel.Models.Identity;

namespace GezginTravel.Models.Entities
{
    public class BlogLike : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}