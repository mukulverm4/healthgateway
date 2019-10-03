//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.Medication.Test
{
    using HealthGateway.MedicationService.Models;
    using HealthGateway.MedicationService.Parsers;
    using HealthGateway.MedicationService.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;


    public class MedicationService_Test
    {
        private readonly IMedicationService service;

        public MedicationService_Test()
        {
            Mock<IHNMessageParser<Prescription>> parserMock = new Mock<IHNMessageParser<Prescription>>();
            Mock<IHttpClientFactory> httpMock = new Mock<IHttpClientFactory>();
            var clientHandlerStub = new DelegatingHandlerStub();
            var client = new HttpClient(clientHandlerStub);
            httpMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            Mock<IConfiguration> configMock = new Mock<IConfiguration>();
            this.service = new RestMedicationService(parserMock.Object, httpMock.Object, configMock.Object);            
        }

        [Fact]
        public async Task ShouldGetPrescriptions()
        {
            List<Prescription> prescriptions = await this.service.GetPrescriptionsAsync("123456789", "test", "10.0.0.1");

            Assert.True(prescriptions.Count == 0);
        }
    }
}
