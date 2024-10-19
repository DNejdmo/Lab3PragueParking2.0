using PragueParkingV2;

public class MC : Vehicle
{
    public MC(string registrationNumber)
        : base(registrationNumber, "MC")
    {
    }

    public override string GetInfo()
    {
        return $"MC {RegistrationNumber}";
    }
}

