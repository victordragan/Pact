using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Consumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer
{
    public class CarsApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public CarsApiClient(string baseUri = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri ?? "http://my.api/v2/app") };
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public IEnumerable<Car> GetAllCars()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/cars");
            request.Headers.Add("Accept", "application/json");

            var response = _httpClient.SendAsync(request);

            try
            {
                var result = response.Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var content = result.Content.ReadAsStringAsync().Result;
                    return !String.IsNullOrEmpty(content)
                                ? JsonConvert.DeserializeObject<IEnumerable<Car>>(content, _jsonSettings)
                                : new List<Car>();
                }

                RaiseResponseError(request, result);
            }
            finally
            {
                Dispose(request, response);
            }

            return null;
        }

        public void CreateCar(Guid eventId)
        {
            var car = new
            {
                CarId = eventId,
                Mileage = 0,
                Fuel = "Gasoline"
            };

            var eventJson = JsonConvert.SerializeObject(car, _jsonSettings);
            var requestContent = new StringContent(eventJson, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/cars") { Content = requestContent };

            var response = _httpClient.SendAsync(request);

            try
            {
                var result = response.Result;
                var statusCode = result.StatusCode;
                if (statusCode == HttpStatusCode.Created)
                {
                    return;
                }

                RaiseResponseError(request, result);
            }
            finally
            {
                Dispose(request, response);
            }
        }

        private static void RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            throw new HttpRequestException(
                String.Format("The Events API request for {0} {1} failed. Response Status: {2}, Response Body: {3}",
                failedRequest.Method.ToString().ToUpperInvariant(),
                failedRequest.RequestUri,
                (int)failedResponse.StatusCode, 
                failedResponse.Content.ReadAsStringAsync().Result));
        }

        public void Dispose()
        {
            Dispose(_httpClient);
        }

        public void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables.Where(d => d != null))
            {
                disposable.Dispose();
            }
        }
    }
}
