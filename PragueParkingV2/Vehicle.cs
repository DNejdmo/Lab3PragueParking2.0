using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParkingV2
{
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; set; }
        public string VehicleType { get; set; }
        public DateTime ParkingTime { get; set; }  // Tid när fordonet parkerades

        protected Vehicle(string registrationNumber, string vehicleType)
        {
            RegistrationNumber = registrationNumber;
            VehicleType = vehicleType;
            ParkingTime = DateTime.Now;
        }

        public virtual string GetInfo()
        {
            return $"{VehicleType} {RegistrationNumber}";
        }
    }

}
