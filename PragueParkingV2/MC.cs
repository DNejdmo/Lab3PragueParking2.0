using PragueParkingV2;

public class MC : Vehicle
{
    public MC(string registrationNumber) : base(registrationNumber, "MC", 2)
    {
    }

    public override string GetInfo()
    {
        return $"MC {RegistrationNumber}";
    }
}

