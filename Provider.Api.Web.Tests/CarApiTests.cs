using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Api.Web.Tests
{
    public class CarApiTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public CarApiTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EnsureCarApiHonoursPactWithConsumer()
        {
            //Arrange
            const string serviceUri = "http://localhost:9222";
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_output)
                }
            };
            
            using (WebApp.Start<TestStartup>(serviceUri))
            {
                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ProviderState($"{serviceUri}/provider-states")
                    .ServiceProvider("Car API", serviceUri)
                    .HonoursPactWith("Car API Consumer")
                    //.PactUri($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Consumer.Tests{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}car_api_consumer-car_api.json")
                    .PactUri("http://localhost/pacts/provider/Car%20API/consumer/Car%20API%20Consumer/latest")
                    .Verify();
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
