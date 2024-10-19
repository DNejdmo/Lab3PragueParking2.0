public class ParkingGarage
{
    private List<string>[] parkingLot;
    private List<VehicleType> vehicleTypes;

    public ParkingGarage(int spots, List<VehicleType> vehicleTypes)
    {
        this.vehicleTypes = vehicleTypes;
        parkingLot = new List<string>[spots]; // Initierar parkeringsplatserna
        for (int i = 0; i < parkingLot.Length; i++)
        {
            parkingLot[i] = new List<string>(); // Initierar listor på varje plats
        }
    }

    // Metod för att parkera fordon
    public void ParkVehicle(string vehicleType, string registrationNumber)
    {
        int spot = FindAvailableSpot(vehicleType);
        if (spot == -1)
        {
            Console.WriteLine("Ingen ledig plats för fordonet.");
            return;
        }

        string vehicle = $"{vehicleType}#{registrationNumber}";

        if (vehicleType == "CAR")
        {
            if (parkingLot[spot].Count == 0)
            {
                parkingLot[spot].Add(vehicle);
                Console.WriteLine($"Bil {registrationNumber} har parkerats på plats {spot + 1}.");
            }
            else
            {
                Console.WriteLine("Platsen är redan upptagen.");
            }
        }
        else if (vehicleType == "MC")
        {
            if (parkingLot[spot].Count < 2)
            {
                parkingLot[spot].Add(vehicle);
                Console.WriteLine($"MC {registrationNumber} har parkerats på plats {spot + 1}.");
            }
            else
            {
                Console.WriteLine("Platsen är redan full för motorcyklar.");
            }
        }
        // Lägg till logik för BUS, BICYCLE om du vill hantera dem också
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
                Console.WriteLine($"Plats {i + 1}: {string.Join(", ", parkingLot[i])}");
            }
        }
    }

    // Metod för att flytta fordon
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
                if (parkingLot[i][j].Contains(registrationNumber))
                {
                    // Flytta fordonet om den nya platsen är ledig
                    if (parkingLot[newSpot].Count == 0 || (parkingLot[newSpot][0].StartsWith("MC") && parkingLot[newSpot].Count < 2))
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

        // Metod för att ta bort ett specifikt fordon
    public void RemoveVehicle(string registrationNumber)
    {
            for (int i = 0; i < parkingLot.Length; i++)
            {
                for (int j = 0; j < parkingLot[i].Count; j++)
                {
                    if (parkingLot[i][j].Contains(registrationNumber))
                    {
                        parkingLot[i].RemoveAt(j);
                        Console.WriteLine($"Fordon {registrationNumber} har tagits bort från plats {i + 1}.");
                        return;
                    }
                }
            }

            Console.WriteLine("Fordonet kunde inte hittas.");
    }

        // Metod för att leta efter ett fordon
        public void FindVehicle(string registrationNumber)
        {
            for (int i = 0; i < parkingLot.Length; i++)
            {
                foreach (var vehicle in parkingLot[i])
                {
                    if (vehicle.Contains(registrationNumber))
                    {
                        Console.WriteLine($"Fordon {registrationNumber} hittades på plats {i + 1}.");
                        return;
                    }
                }
            }

            Console.WriteLine("Fordonet kunde inte hittas.");
        }

        // Metod för att hitta en ledig plats
        private int FindAvailableSpot(string vehicleType)
        {
            // Om MC, först leta efter en plats som redan har en motorcykel men inte är full
            if (vehicleType == "MC")
            {
                for (int i = 0; i < parkingLot.Length; i++)
                {
                    if (parkingLot[i].Count > 0 && parkingLot[i][0].StartsWith("MC") && parkingLot[i].Count < 2)
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
}

