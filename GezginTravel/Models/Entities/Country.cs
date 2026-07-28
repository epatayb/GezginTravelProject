using System;

namespace GezginTravel.Models.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public ICollection<City> Cities { get; set; }
        public ICollection<Blog> Blogs { get; set; }

        public Country() 
        { 
            Cities = new HashSet<City>();
            Blogs = new HashSet<Blog>();
        }
    }
}
