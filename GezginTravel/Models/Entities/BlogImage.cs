using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GezginTravel.Models.Entities
{
    public class BlogImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public int OrderNo { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}