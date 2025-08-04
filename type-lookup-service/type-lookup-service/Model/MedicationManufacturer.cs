using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace type_lookup_service.Models
{
	[BsonIgnoreExtraElements]
	public class MedicationManufacturer : MedCompassBase
	{
		[MaxLength(500)]
		public string ManufacturerName { get; set; }
	}
}
