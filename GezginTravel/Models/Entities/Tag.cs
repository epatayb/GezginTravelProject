using System;

namespace GezginTravel.Models.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        // Navigation Properties
        public ICollection<BlogTag> BlogTags { get; set; }
    }
}