using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace type_lookup_service.Models
{
	[BsonIgnoreExtraElements]
	public class MedicationProductDescription : MedCompassBase
	{
		[MaxLength(4000)]
		public string ProductDescription { get; set; }

		[MaxLength(1)]
		public string ProductDescriptionFirstLetter { get; set; }

        public int? GCNSEQNumber { get; set; }

        public Guid? MedicationProductGroupId { get; set; }

        public string MedicationSource { get; set; }
    }
}
