using System;

namespace GezginTravel.Models.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        public Country() 
        { 
            Cities = new HashSet<City>();
        }
    }
}
