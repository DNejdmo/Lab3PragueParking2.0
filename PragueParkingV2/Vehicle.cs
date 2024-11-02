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
        public int Size { get; set; }
        public DateTime ParkingTime { get; set; }

        protected Vehicle(string registrationNumber, string vehicleType, int size)
        {
            RegistrationNumber = registrationNumber;
            VehicleType = vehicleType;
            Size = size; 
            ParkingTime = DateTime.Now;
        }

        public virtual string GetInfo()
        {
            return $"{VehicleType} {RegistrationNumber}";
        }
    }


}
