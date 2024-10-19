using PragueParkingV2;

public class Car : Vehicle
{
    public Car(string registrationNumber)
        : base(registrationNumber, "CAR")
    {
    }

    public override string GetInfo()
    {
        return $"Bil {RegistrationNumber}";
    }
}

