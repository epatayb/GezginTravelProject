using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GezginTravel.Models.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } // Soft delete flag
        public DateTime? DeletedDate { get; set; }
    }
}