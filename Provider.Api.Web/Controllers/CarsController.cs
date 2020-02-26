using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Provider.Api.Web.Models;

namespace Provider.Api.Web.Controllers
{
    public class CarsController : ApiController
    {
        [Route("cars")]
        public IEnumerable<Car> Get()
        {
            return GetAllCarsFromRepo();
        }

        [Route("cars")]
        public HttpResponseMessage Post(Car car)
        {
            if (car == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private IEnumerable<Car> GetAllCarsFromRepo()
        {
            return new List<Car>
            {
                new Car
                {
                    CarId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Mileage1 = 0,
                    Fuel = "Gasoline"
                },
                new Car
                {
                    CarId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Mileage1 = 0,
                    Fuel = "Gasoline"
                },
                new Car
                {
                    CarId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    Mileage1 = 15000,
                    Fuel = "Diesel"
                }
            };
        }
    }
}