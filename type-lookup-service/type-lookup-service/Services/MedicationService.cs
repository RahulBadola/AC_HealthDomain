using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using type_lookup_service.Data;
using type_lookup_service.Models;
using type_lookup_service.Utils;
using System.Linq;
using type_lookup_service.Model;

namespace type_lookup_service.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public MedicationService(
            IContextLogger<MedicationService> logger,
            IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<(DbResponse response, Medication data)> GetMedicationById(Guid medicationId)
        {
            _logger.LogInformation($"{nameof(MedicationService)} - {nameof(GetMedicationById)} Begin");
            return await _repository.GetMedication(medicationId);
        }

        public async Task<(DbResponse response, List<Medication> data)> GetMedications(MedicationSearchRequest medications)
        {
            _logger.LogInformation($"{nameof(MedicationService)} - {nameof(GetMedications)} Begin");
            var (response, medicationDetails) = await _repository.GetMedications(medications?.MedicationIds);
            if (medicationDetails?.Count > 0)
            {
                await IncludeMedicationDescriptions(medications, medicationDetails);
            }
            return (response, medicationDetails);
        }
        private async Task IncludeMedicationDescriptions(MedicationSearchRequest medications, List<Medication> medicationDetails)
        {
            if (medications.IncludeProdDesc)
            {
                var (prodresponse, proddescriptions) = await _repository.GetProductDescriptionsByIdsAsync(medicationDetails?.Select(x => x.MedicationProductDescriptionId.GetValueOrDefault()).ToList());
                if (prodresponse == DbResponse.Found && proddescriptions?.Count > 0)
                {
                    medicationDetails.ForEach(med =>
                    {
                        med?.MedicationProductDescriptions?.Add(proddescriptions.FirstOrDefault(x => x.Id.Equals(med.MedicationProductDescriptionId)));
                    });
                }
            }
            if (medications.IncludePackDesc)
            {
                var (packresponse, packDescriptions) = await _repository.GetPackageDescriptionIdsAsync(medicationDetails?.Select(x => x.MedicationPackageDescriptionID.GetValueOrDefault()).ToList());
                if (packresponse == DbResponse.Found && packDescriptions?.Count > 0)
                {
                    medicationDetails.ForEach(med =>
                    {
                        med?.MedicationPackageDescriptions?.Add(packDescriptions.FirstOrDefault(x => x.Id.Equals(med.MedicationPackageDescriptionID)));
                    });
                }
            }
            if (medications.IncludeManfacturer)
            {
                var (manuresponse, manufacturers) = await _repository.GetManufacturerByIdsAsync(medicationDetails?.Select(x => x.MedicationManufacturerId.GetValueOrDefault()).ToList());
                if (manuresponse == DbResponse.Found && manufacturers?.Count > 0)
                {
                    medicationDetails.ForEach(med =>
                    {
                        med?.MedicationManufacturers?.Add(manufacturers.FirstOrDefault(x => x.Id.Equals(med.MedicationManufacturerId)));
                    });
                }
            }
        }
    }
}
