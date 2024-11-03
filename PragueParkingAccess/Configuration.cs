
using PragueParkingAccess;
using System.Text.Json;
namespace PragueParkingAccess

{

    public class Configuration
    {
        public int ParkingSpots { get; set; }
        public List<VehicleType> VehicleTypes { get; set; }
        public int ParkingSpotSize { get; set; }
        public int Columns { get; set; }

    }
   
}