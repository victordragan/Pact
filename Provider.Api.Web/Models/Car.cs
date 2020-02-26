using System;

namespace Provider.Api.Web.Models
{
    public class Car
    {
        public Guid CarId { get; set; }
        public int Mileage1 { get; set; }
        public string Fuel { get; set; }
    }
}
