using System;

namespace GezginTravel.Models.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public ICollection<Blog> Blogs { get; set; }

        public City()
        {
            Blogs = new HashSet<Blog>();
        }
    }
}