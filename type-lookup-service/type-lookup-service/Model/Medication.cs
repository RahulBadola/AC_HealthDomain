using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace type_lookup_service.Models
{
    [BsonIgnoreExtraElements]
    public class Medication : MedCompassBase
    {
        public Medication()
        {
            MedicationImages = new List<MedicationImage>();
            MedicationManufacturers = new List<MedicationManufacturer>();
            MedicationPackageDescriptions = new List<MedicationPackageDescription>();
            MedicationProductDescriptions = new List<MedicationProductDescription>();
        }

		[Required]
		[MaxLength(50)]
		public string MedicationCode { get; set; }

		[MaxLength(100)]
		public string MedicationProductType { get; set; }

		[MaxLength(4000)]
		public string Route { get; set; }

        public bool? Formulary { get; set; }

        public Guid? MedicationManufacturerId { get; set; }

        public Guid? MedicationProductDescriptionId { get; set; }

        public Guid? MedicationPackageDescriptionID { get; set; }

		[MaxLength(2000)]
		public string PatientPMLDisplayName { get; set; }

        [MaxLength(50)]
		public string DEASCHEDULE { get; set; }

        [MaxLength(50)]
		public string NormalizedMedicationCode { get; set; }

        public IList<MedicationImage> MedicationImages { get; set; }

        public IList<MedicationManufacturer> MedicationManufacturers { get; set; }

        public IList<MedicationPackageDescription> MedicationPackageDescriptions { get; set; }

        public IList<MedicationProductDescription> MedicationProductDescriptions { get; set; }
    }
}
