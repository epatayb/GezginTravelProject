using System;
using GezginTravel.Models.Identity;

namespace GezginTravel.Models.Entities
{
    public class BlogComment : BaseEntity
    {
        public string Content { get; set; }

        public int UserId { get; set; }
        public AppUser User { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public int? ParentCommentId { get; set; }

        public BlogComment ParentComment { get; set; }
        public ICollection<BlogComment> Replies { get; set; }
    }
}