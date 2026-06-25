using System;

namespace GezginTravel.Models.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public ICollection<BlogCategory> BlogCategories { get; set; }
    }
}
