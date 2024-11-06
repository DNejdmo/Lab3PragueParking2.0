using PragueParkingAccess;
namespace PragueParkingAccess
{
    public class Bus : Vehicle
    {
        public Bus(string registrationNumber) : base(registrationNumber, "BUS", 16)
        {
        }

        public override string GetInfo()
        {
            return $"Bus {RegistrationNumber}";
        }
    }

}