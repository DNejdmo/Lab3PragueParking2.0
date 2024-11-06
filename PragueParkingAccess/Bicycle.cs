using PragueParkingAccess;
namespace PragueParkingAccess
{
    public class Bicycle : Vehicle
    {
        public Bicycle(string registrationNumber) : base(registrationNumber, "BICYCLE", 1)
        {
        }

        public override string GetInfo()
        {
            return $"Bike {RegistrationNumber}";
        }
    }

}