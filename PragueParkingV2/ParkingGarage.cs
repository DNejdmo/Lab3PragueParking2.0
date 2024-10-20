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

    // Metod för att parkera fordon (Menyval 1)
    public void ParkVehicle(Vehicle vehicle)
    {
        int spot = FindAvailableSpot(vehicle.VehicleType);
        if (spot == -1)
        {
            Console.WriteLine("Ingen ledig plats för fordonet.");
            return;
        }

        if (vehicle is Car)
        {
            if (parkingLot[spot].Count == 0)
            {
                parkingLot[spot].Add(vehicle);
                Console.WriteLine($"Bil {vehicle.RegistrationNumber} har parkerats på plats {spot + 1}.");
            }
            else
            {
                Console.WriteLine("Platsen är redan upptagen.");
            }
        }
        else if (vehicle is MC)
        {
            if (parkingLot[spot].Count < 2)
            {
                parkingLot[spot].Add(vehicle);
                Console.WriteLine($"MC {vehicle.RegistrationNumber} har parkerats på plats {spot + 1}.");
            }
            else
            {
                Console.WriteLine("Platsen är redan full för motorcyklar.");
            }
        }
        // Hantering för andra fordonstyper kan läggas till här
    }

    //Metod för att hitta ledig plats (Del av menyval 1)
    private int FindAvailableSpot(string vehicleType)
    {
        // Om MC, först leta efter en plats som redan har en motorcykel men inte är full
        if (vehicleType == "MC")
        {
            for (int i = 0; i < parkingLot.Length; i++)
            {
                if (parkingLot[i].Count > 0 && parkingLot[i][0] is MC && parkingLot[i].Count < 2)
                {
                    return i; // Returnera den platsen
                }
            }
        }

        // Leta efter en helt tom plats
        for (int i = 0; i < parkingLot.Length; i++)
        {
            if (parkingLot[i].Count == 0)
            {
                return i; // Ledig plats
            }
        }
        return -1; // Ingen ledig plats
    }



    // Metod för att visa parkeringsplatser (Menyval 2)
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

    // Metod för att flytta fordon (Menyval 3)
    public void MoveVehicle(string registrationNumber, int newSpot)
    {
        if (newSpot < 1 || newSpot > parkingLot.Length)
        {
            Console.WriteLine("Ogiltigt platsnummer.");
            return;
        }
        newSpot--; // Justera för 0-indexerad array

        // Leta efter fordonet
        for (int i = 0; i < parkingLot.Length; i++)
        {
            for (int j = 0; j < parkingLot[i].Count; j++)
            {
                if (parkingLot[i][j].RegistrationNumber == registrationNumber)
                {
                    // Flytta fordonet om den nya platsen är ledig
                    if (parkingLot[newSpot].Count == 0 || (parkingLot[newSpot][0] is MC && parkingLot[newSpot].Count < 2))
                    {
                        parkingLot[newSpot].Add(parkingLot[i][j]);
                        parkingLot[i].RemoveAt(j);
                        Console.WriteLine($"Fordon {registrationNumber} har flyttats till plats {newSpot + 1}.");
                    }
                    else
                    {
                        Console.WriteLine("Den nya platsen är inte tillgänglig.");
                    }
                    return;
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
    }



    // Metod för att leta efter ett fordon (Menyval 4)
    public void FindVehicle(string registrationNumber)
    {
        for (int i = 0; i < parkingLot.Length; i++)
        {
            foreach (var vehicle in parkingLot[i])
            {
                if (vehicle.RegistrationNumber == registrationNumber)
                {
                    Console.WriteLine($"Fordon {registrationNumber} hittades på plats {i + 1}.");
                    return;
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
    }


   
    // Metod för att ta bort ett fordon (Menyval 5)
    public Vehicle RemoveVehicle(string registrationNumber)
    {
        for (int i = 0; i < parkingLot.Length; i++)
        {
            for (int j = 0; j < parkingLot[i].Count; j++)
            {
                if (parkingLot[i][j].RegistrationNumber == registrationNumber)
                {
                    Vehicle removedVehicle = parkingLot[i][j]; // Spara fordonet innan det tas bort
                    parkingLot[i].RemoveAt(j);
                    Console.WriteLine($"Fordon {registrationNumber} har tagits bort från plats {i + 1}.");
                    return removedVehicle; // Returnera det borttagna fordonet
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
        return null; // Returnera null om fordonet inte hittades
    }



   
}
