using PragueParkingAccess;
namespace PragueParkingAccess
{
    public class Car : Vehicle
    {
        public Car(string registrationNumber) : base(registrationNumber, "CAR", 4)
        {
        }

        public override string GetInfo()
        {
            return $"Bil {RegistrationNumber}";
        }
    }

}