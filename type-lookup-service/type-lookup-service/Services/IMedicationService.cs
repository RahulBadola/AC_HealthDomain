using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using type_lookup_service.Model;
using type_lookup_service.Models;

namespace type_lookup_service.Services
{
    public interface IMedicationService
    {
        Task<(DbResponse response, Medication data)> GetMedicationById(Guid medicationId);
        Task<(DbResponse response, List<Medication> data)> GetMedications(MedicationSearchRequest medications);
    }
}
