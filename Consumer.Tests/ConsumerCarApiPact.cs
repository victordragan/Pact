using System;
using System.IO;
using PactNet;
using PactNet.Mocks.MockHttpService;
using PactNet.Models;

namespace Consumer.Tests
{
    public class ConsumerCarApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; }

        public int MockServerPort => 9222;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public ConsumerCarApiPact()
        {
            PactBuilder = new PactBuilder(new PactConfig
            {
                SpecificationVersion = "2.0",
                LogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}",
                PactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}"
            })
                .ServiceConsumer("Car API Consumer")
                .HasPactWith("Car API");

            MockProviderService = PactBuilder.MockService(MockServerPort, false, IPAddress.Any);
        }

        public void Dispose()
        {
            PactBuilder.Build();

            var pactPublisher = new PactPublisher("http://localhost");
            pactPublisher.PublishToBroker(
                "..\\..\\..\\pacts\\car_api_consumer-car_api.json",
                "2.0", new[] { "master" });
        }
    }
}