using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace Consumer.Tests
{
    public class CarsApiConsumerTests : IClassFixture<ConsumerCarApiPact>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;

        public CarsApiConsumerTests(ConsumerCarApiPact data)
        {
            _mockProviderService = data.MockProviderService;
            _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public void GetAllCars_WhenCalled_ReturnsAllEvents()
        {
            var test = new[]
            {
                new
                {
                    carId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    mileage1 =  0,
                    fuel = "Gasoline"
                },
                new
                {
                    carId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    mileage1 = 0,
                    fuel = "Gasoline"
                },
                new
                {
                    carId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    mileage1 = 15000,
                    fuel = "Diesel"
                }
            };

            var res = new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, object>
                {
                    {"Content-Type", "application/json; charset=utf-8"}
                },
                Body = test
            };

            //Arrange
            _mockProviderService.Given("there are cars with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                .UponReceiving("a request to retrieve all cars")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/cars",
                    Headers = new Dictionary<string, object>
                    {
                        { "Accept", "application/json" },
                    }
                })
                .WillRespondWith(res);

            var consumer = new CarsApiClient(_mockProviderServiceBaseUri);

            //Act
            var events = consumer.GetAllCars();

            //Assert
            Assert.NotEmpty(events);
            Assert.Equal(3, events.Count());


            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void CreateEvent_WhenCalledWithEvent_Succeeds()
        {
            //Arrange
            var carId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");

            _mockProviderService.UponReceiving("a request to create a new car")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/cars",
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        carId,
                        mileage = 0,
                        fuel = "Gasoline"
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 201
                });

            var consumer = new CarsApiClient(_mockProviderServiceBaseUri);

            //Act / Assert
            consumer.CreateCar(carId);

            _mockProviderService.VerifyInteractions();
        }
    }
}
