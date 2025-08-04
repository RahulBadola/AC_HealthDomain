using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using type_lookup_service.Data;
using type_lookup_service.Services;
using type_lookup_service.Utils;
using AutoFixture;
using type_lookup_service.Models;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Linq;
using type_lookup_service.Model;

namespace type_lookup_service.Tests.Services
{
    public class MedicationServiceTests
    {
        private MockRepository mockRepositoryBase;

        private Mock<IContextLogger<MedicationService>> mockContextLogger;
        private Mock<IRepository> mockRepository;
        private readonly Fixture fixture = new Fixture { RepeatCount = 10 };
        public MedicationServiceTests()
        {
            this.mockRepositoryBase = new MockRepository(MockBehavior.Default);
            this.mockContextLogger = this.mockRepositoryBase.Create<IContextLogger<MedicationService>>();
            this.mockRepository = this.mockRepositoryBase.Create<IRepository>();
        }

        private MedicationService CreateService()
        {
            mockContextLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            return new MedicationService(
                this.mockContextLogger.Object,
                this.mockRepository.Object);
        }

        [Fact]
        public async Task GetMedicationByIdShouldReturnFoundIfRecordExists()
        {
            // Arrange
            var service = this.CreateService();
           
            var data = fixture.Create<Medication>();
            Guid medicationId = data.Id;

            this.mockRepository.Setup(
                repo => repo.GetMedication(It.IsAny<Guid>())
                ).ReturnsAsync((DbResponse.Found, data));

            // Act
            var result = await service.GetMedicationById(
                medicationId);

            // Assert
            Assert.Equal(DbResponse.Found, result.response);
        }

        [Fact]
        public async Task GetMedicationByIdShouldReturnNotFoundIfRecordExists()
        {
            // Arrange
            var service = this.CreateService();

            var data = fixture.Create<Medication>();
            Guid medicationId = data.Id;

            this.mockRepository.Setup(
                repo => repo.GetMedication(It.IsAny<Guid>())
                ).ReturnsAsync((DbResponse.NotFound, null));

            // Act
            var result = await service.GetMedicationById(
                medicationId);

            // Assert
            Assert.Equal(DbResponse.NotFound, result.response);
        }
        [Fact]
        public async Task GetMedicationsdShouldReturnFoundIfRecordsExists()
        {
            // Arrange
            MedicationSearchRequest searchRequest = new MedicationSearchRequest();
            var service = this.CreateService();

            var medications = fixture.CreateMany<Medication>().ToList();
            searchRequest.MedicationIds = medications.Select(x => x.Id).ToList();
            this.mockRepository.Setup(
                repo => repo.GetMedications(It.IsAny<List<Guid>>())
                ).ReturnsAsync((DbResponse.Found, medications));

            // Act
            var result = await service.GetMedications(searchRequest);


            // Assert
            Assert.Equal(DbResponse.Found, result.response);
        }

        [Fact]
        public async Task GetMedicationShouldReturnNotFoundIfRecordDoesntExists()
        {
            // Arrange
            var service = this.CreateService();
            MedicationSearchRequest searchRequest = new MedicationSearchRequest();
            var medications = fixture.CreateMany<Medication>().ToList();
            searchRequest.MedicationIds = medications.Select(x => x.Id).ToList();
            this.mockRepository.Setup(
                repo => repo.GetMedications(It.IsAny<List<Guid>>())
                ).ReturnsAsync((DbResponse.NotFound, null));

            // Act
            var result = await service.GetMedications(
                searchRequest);
            
            // Assert
            Assert.Equal(DbResponse.NotFound, result.response);
        }
        [Fact]
        public async Task GetMedicationIfrequiredProductPackageManufactureDescriptionTrue()
        {
            // Arrange
            var service = this.CreateService();
            MedicationSearchRequest searchRequest = new MedicationSearchRequest() { 
            MedicationIds= It.IsAny<List<Guid>>(),
            IncludeManfacturer=true,
            IncludePackDesc=true,
            IncludeProdDesc=true
            };

            var medications = fixture.CreateMany<Medication>().ToList();
                this.mockRepository.Setup(
                repo => repo.GetMedications(It.IsAny<List<Guid>>())
                ).ReturnsAsync((DbResponse.Found, medications));
            searchRequest.MedicationIds = medications.Select(x => x.Id).ToList();

            var productDescriptions = fixture.CreateMany<MedicationProductDescription>().ToList();
            this.mockRepository.Setup(
                repo => repo.GetProductDescriptionsByIdsAsync(It.IsAny<List<Guid>>(),false)
                ).ReturnsAsync((DbResponse.Found, productDescriptions));
            var manufacturer = fixture.CreateMany<MedicationManufacturer>().ToList();
            this.mockRepository.Setup(
               repo => repo.GetManufacturerByIdsAsync(It.IsAny<List<Guid>>(), false)
               ).ReturnsAsync((DbResponse.Found, manufacturer));
            var packageDescription = fixture.CreateMany<MedicationPackageDescription>().ToList();
            this.mockRepository.Setup(
               repo => repo.GetPackageDescriptionIdsAsync(It.IsAny<List<Guid>>(), false)
               ).ReturnsAsync((DbResponse.Found, packageDescription));

            // Act
            var result = await service.GetMedications(
                searchRequest);

            // Assert
            Assert.Equal(DbResponse.Found, result.response);
            Assert.True(result.data.Count>0);
            Assert.True(result.data[0].MedicationPackageDescriptions.Count>10);
            Assert.True(result.data[0].MedicationManufacturers.Count > 10);
            Assert.True(result.data[0].MedicationPackageDescriptions.Count > 10);
        }
        [Fact]
        public async Task MedicationsIfAnyError()
        {
            // Arrange
            var service = this.CreateService();
            MedicationSearchRequest searchRequest = new MedicationSearchRequest();

            var medications = fixture.CreateMany<Medication>().ToList();
            searchRequest.MedicationIds = medications.Select(x => x.Id).ToList();
            // Act
            var result = await service.GetMedications(searchRequest);

            // Assert
            Assert.Equal(DbResponse.Error, result.response);
        }

    }
}
