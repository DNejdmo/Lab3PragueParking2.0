using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using PragueParkingAccess;
namespace PragueParkingAccess

{
    public class VehicleSaveData
    {
        public string RegistrationNumber { get; set; }
        public string VehicleType { get; set; }
        public DateTime ParkingTime { get; set; }
        public int ParkingSpot { get; set; }
    }

}
