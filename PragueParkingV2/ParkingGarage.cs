using PragueParkingV2;
using Spectre.Console;
using System.Text.Json;

public class ParkingGarage
{
    private List<Vehicle>[] parkingLot;
    private string saveFilePath = "../../../parkingData.json"; // Fil där data sparas när programmet stängs

    public ParkingGarage(int spots, List<VehicleType> vehicleTypes)
    {
        parkingLot = new List<Vehicle>[spots];
        for (int i = 0; i < parkingLot.Length; i++)
        {
            parkingLot[i] = new List<Vehicle>();
        }

        // Ladda sparade fordon om filen existerar
        LoadVehicles();
    }

    // Metod för att parkera fordon (Menyval 1)
    public void ParkVehicle(Vehicle vehicle)
    {
        int startSpot = FindAvailableSpots(vehicle.Size);

        if (startSpot == -1)
        {
            Console.WriteLine("Ingen ledig plats för fordonet.");
            return;
        }

        // Om fordonet får plats på en befintlig plats, parkera det där
        if (parkingLot[startSpot].Sum(v => v.Size) + vehicle.Size <= 4)
        {
            parkingLot[startSpot].Add(vehicle);
            Console.WriteLine($"{vehicle.VehicleType} {vehicle.RegistrationNumber} har parkerats på plats {startSpot + 1}.");
        }
        else
        {
            // Annars, parkera fordonet över flera tomma platser
            int requiredSpots = (int)Math.Ceiling(vehicle.Size / 4.0);
            for (int i = startSpot; i < startSpot + requiredSpots; i++)
            {
                parkingLot[i].Add(vehicle);
            }
            Console.WriteLine($"{vehicle.VehicleType} {vehicle.RegistrationNumber} har parkerats från plats {startSpot + 1} till {startSpot + requiredSpots}.");
        }

        SaveVehicles(); // Spara parkerat fordon
    }






    //Metod för att hitta ledig plats (Del av menyval 1)
    private int FindAvailableSpots(int vehicleSize)
    {
        // Försök först att hitta en plats med tillräckligt med ledigt utrymme för mindre fordon
        for (int i = 0; i < parkingLot.Length; i++)
        {
            int currentSizeUsed = parkingLot[i].Sum(v => v.Size);
            int availableSize = 4 - currentSizeUsed; // Anta 4 som standardstorlek för en parkeringsplats

            if (availableSize >= vehicleSize)
            {
                return i; // Returnera en befintlig plats med tillräckligt med utrymme
            }
        }

        // Om inget delat utrymme hittades, leta efter en sekvens av helt tomma platser för större fordon
        int requiredSpots = (int)Math.Ceiling(vehicleSize / 4.0);
        int consecutiveEmpty = 0;

        for (int i = 0; i < parkingLot.Length; i++)
        {
            if (parkingLot[i].Count == 0)
            {
                consecutiveEmpty++;
                if (consecutiveEmpty == requiredSpots)
                {
                    return i - requiredSpots + 1; // Returnera startpositionen
                }
            }
            else
            {
                consecutiveEmpty = 0; // Återställ om sekvensen bryts
            }
        }

        return -1; // Ingen ledig sekvens hittades
    }

    // Metod för att visa parkeringsplatser (Menyval 2)
    public void ShowParkingLot()
    {
        int columns = 10;  
        int totalSpots = 100;  
        var table = new Table()
            .NoBorder()  
            .Collapse()  
            .HideHeaders();  

        
        for (int i = 0; i < columns; i++)
        {
            table.AddColumn(""); 
        }

        var rowContent = new List<string>();

        // Gå igenom alla parkeringsplatser
        for (int spotIndex = 0; spotIndex < totalSpots; spotIndex++)
        {
            string cellContent;

            // Om platsen är upptagen
            if (spotIndex < parkingLot.Length && parkingLot[spotIndex].Count > 0)
            {
                var vehiclesOnSpot = parkingLot[spotIndex];

                // Om det finns en bil eller två MC
                if (vehiclesOnSpot.Count == 1)
                {
                    cellContent = $"Plats {spotIndex + 1}\n{Markup.Escape(vehiclesOnSpot[0].RegistrationNumber)}";
                }
                else
                {
                    cellContent = $"Plats {spotIndex + 1}\n{Markup.Escape(vehiclesOnSpot[0].RegistrationNumber)}, {Markup.Escape(vehiclesOnSpot[1].RegistrationNumber)}";
                }
            }
            else
            {
                // Om platsen är tom
                cellContent = $"Plats {spotIndex + 1}\nTOM";
            }

            
            rowContent.Add(cellContent);

            
            if (rowContent.Count == columns)
            {
                table.AddRow(rowContent.ToArray());
                rowContent.Clear();
            }
        }

        // Lägg till sista raden om det finns platser kvar
        if (rowContent.Count > 0)
        {
            while (rowContent.Count < columns)
            {
                rowContent.Add("EMPTY");  // Fyll tomma celler om nödvändigt
            }
            table.AddRow(rowContent.ToArray());
        }

        // Skriv ut tabellen med Spectre.Console
        AnsiConsole.Write(table);
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
                    Vehicle vehicleToRemove = parkingLot[i][j];
                    parkingLot[i].RemoveAt(j);
                    Console.WriteLine($"Fordon {registrationNumber} har tagits bort från plats {i + 1}.");

                    SaveVehicles(); // Spara direkt efter borttagning
                    return vehicleToRemove;  // Returnera det borttagna fordonet
                }
            }
        }
        Console.WriteLine("Fordonet kunde inte hittas.");
        return null; // Returnera null om inget fordon hittades
    }

    public void SaveVehicles()
    {
        var vehiclesToSave = new List<VehicleSaveData>();

        // Gå igenom alla parkeringsplatser och spara fordonsdata
        for (int i = 0; i < parkingLot.Length; i++)
        {
            foreach (var vehicle in parkingLot[i])
            {
                vehiclesToSave.Add(new VehicleSaveData
                {
                    RegistrationNumber = vehicle.RegistrationNumber,
                    VehicleType = vehicle.VehicleType,
                    ParkingTime = vehicle.ParkingTime,
                    ParkingSpot = i
                });
            }
        }

        // Serialisera till JSON och spara i filen
        string jsonData = JsonSerializer.Serialize(vehiclesToSave, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(saveFilePath, jsonData);
        Console.WriteLine("Fordon har sparats.");
    }


    // Metod för att läsa in fordon från en JSON-fil
    public void LoadVehicles()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            var loadedVehicles = JsonSerializer.Deserialize<List<VehicleSaveData>>(jsonData);

            if (loadedVehicles != null)
            {
                foreach (var vehicleData in loadedVehicles)
                {
                    Vehicle vehicle;
                    if (vehicleData.VehicleType == "CAR")
                    {
                        vehicle = new Car(vehicleData.RegistrationNumber);
                    }
                    else if (vehicleData.VehicleType == "MC")
                    {
                        vehicle = new MC(vehicleData.RegistrationNumber);
                    }
                    else
                    {
                        // Hantering för andra fordonstyper om det finns
                        continue; // Om vi inte kan hantera fordonstypen, hoppa över det
                    }

                    vehicle.ParkingTime = vehicleData.ParkingTime;
                    parkingLot[vehicleData.ParkingSpot].Add(vehicle);
                }

                Console.WriteLine("Fordon har laddats från fil.");
            }
        }
    }


}
