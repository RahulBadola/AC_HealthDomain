using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace type_lookup_service.Models
{
	[BsonIgnoreExtraElements]
	public class MedicationPackageDescription : MedCompassBase
	{
		[MaxLength(4000)]
		public string PackageDescription { get; set; }

		[MaxLength(1)]
		public string PackageDescriptionFirstLetter { get; set; }
	}
}
