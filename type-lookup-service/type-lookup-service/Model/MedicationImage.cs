using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace type_lookup_service.Models
{
    [BsonIgnoreExtraElements]
    public class MedicationImage : MedCompassBase
    {
        [Required]
		public Guid MedicationId { get; set; }

        public string Image { get; set; }

        public Guid? RowFilterId { get; set; }
    }
}
