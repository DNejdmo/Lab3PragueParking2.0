using PragueParkingV2;

public class ParkingGarage
{
    private List<Vehicle>[] parkingLot;

    public ParkingGarage(int spots, List<VehicleType> vehicleTypes)
    {
        parkingLot = new List<Vehicle>[spots];
        for (int i = 0; i < parkingLot.Length; i++)
        {
            parkingLot[i] = new List<Vehicle>();
        }
    }

    // Metod för att parkera fordon
    public void ParkVehicle(Vehicle vehicle)
    {
        int spot = FindAvailableSpot(vehicle.VehicleType);
        if (spot == -1)
        {
            Console.WriteLine("Ingen ledig plats för fordonet.");
            return;
        }

        parkingLot[spot].Add(vehicle);
        Console.WriteLine($"{vehicle.GetInfo()} har parkerats på plats {spot + 1}.");
    }

    // Metod för att visa parkeringsplatser
    public void ShowParkingLot()
    {
        Console.WriteLine("\n--- Parkeringsöversikt ---");
        for (int i = 0; i < parkingLot.Length; i++)
        {
            if (parkingLot[i].Count == 0)
            {
                Console.WriteLine($"Plats {i + 1}: [TOM]");
            }
            else
            {
                Console.WriteLine($"Plats {i + 1}: {string.Join(", ", parkingLot[i].Select(v => v.GetInfo()))}");
            }
        }
    }

    // Metod för att hitta en ledig plats
    private int FindAvailableSpot(string vehicleType)
    {
        for (int i = 0; i < parkingLot.Length; i++)
        {
            if (parkingLot[i].Count == 0 || (vehicleType == "MC" && parkingLot[i].Count < 2))
            {
                return i; // Returnera ledig plats
            }
        }
        return -1; // Ingen ledig plats
    }
}
