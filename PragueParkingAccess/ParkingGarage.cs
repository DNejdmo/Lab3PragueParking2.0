using System.Text.Json;
using Spectre.Console;
namespace PragueParkingAccess


{
    public class ParkingGarage
    {
        private List<Vehicle>[] parkingLot;
        private string saveFilePath = "../../../parkingData.json"; // Fil där data sparas när programmet stängs
        private int parkingSpotSize;

        public ParkingGarage(int spots, int spotSize, List<VehicleType> vehicleTypes)
        {
            parkingSpotSize = spotSize;
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
            int startSpot = FindAvailableSpots(vehicle);

            if (startSpot == -1)
            {
                Console.WriteLine("No available parking spot for the vehicle.");
                return;
            }

            int requiredSpots = (int)Math.Ceiling(vehicle.Size / (double)parkingSpotSize);

            // Parkera fordonet över de nödvändiga platserna
            for (int i = startSpot; i < startSpot + requiredSpots; i++)
            {
                parkingLot[i].Add(vehicle);
            }

            // Välj meddelande beroende på antal platser som fordonet upptar
            if (requiredSpots == 1)
            {
                Console.WriteLine($"{vehicle.VehicleType} {vehicle.RegistrationNumber} has been parked on parking spot {startSpot + 1}.");
            }
            else
            {
                Console.WriteLine($"{vehicle.VehicleType} {vehicle.RegistrationNumber} has been parked from spot {startSpot + 1} to {startSpot + requiredSpots}.");
            }

            SaveVehicles();
        }

        // Metod för att hitta ledig plats (Del av menyval 1)
        private int FindAvailableSpots(Vehicle vehicle)
        {
            int vehicleSize = vehicle.Size;
            int requiredSpots = (int)Math.Ceiling(vehicleSize / (double)parkingSpotSize);

            // Om fordonet är mindre än en plats, försök hitta en befintlig delad plats
            if (vehicleSize <= parkingSpotSize / 2)
            {
                for (int i = 0; i < parkingLot.Length; i++)
                {
                    int currentSizeUsed = parkingLot[i].Sum(v => v.Size);
                    if (currentSizeUsed + vehicleSize <= parkingSpotSize)
                    {
                        return i; // Returnera en befintlig plats som kan dela utrymme
                    }
                }
            }

            // Om delad plats inte hittades, sök efter en sekvens av helt tomma platser för större fordon
            int consecutiveEmpty = 0;
            for (int i = 0; i < parkingLot.Length; i++)
            {
                if (parkingLot[i].Count == 0)
                {
                    consecutiveEmpty++;
                    if (consecutiveEmpty == requiredSpots)
                    {
                        return i - requiredSpots + 1; // Returnera startpositionen för tomma platser
                    }
                }
                else
                {
                    consecutiveEmpty = 0;
                }
            }

            return -1; // Ingen lämplig plats hittades
        }

        private Configuration LoadConfig()
        {
            string configPath = "../../../config.json"; 
            if (File.Exists(configPath))
            {
                string jsonData = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<Configuration>(jsonData);
            }
            return new Configuration { ParkingSpots = 100, Columns = 10 }; // standardvärden om configuration saknas
        }

        // Metod för att visa parkeringsplatser (Menyval 2)
        public void ShowParkingLot()
        {
            Configuration config = LoadConfig();
            int columns = config.Columns;
            int totalSpots = config.ParkingSpots;
            int maxSpotSize = config.ParkingSpotSize;
            var table = new Table()
                .NoBorder();

            // Instruktioner för färgerna
            AnsiConsole.MarkupLine("[red]Red = Full[/], [yellow]Yellow = Partially Occupied[/], [green]Green = Empty[/]");

            for (int i = 0; i < columns; i++)
            {
                table.AddColumn("");
            }

            var rowContent = new List<string>();

            // Gå igenom alla parkeringsplatser
            for (int spotIndex = 0; spotIndex < totalSpots; spotIndex++)
            {
                string cellContent = "";

                // Kontrollera om platsen är upptagen
                if (spotIndex < parkingLot.Length && parkingLot[spotIndex].Count > 0)
                {
                    var vehiclesOnSpot = parkingLot[spotIndex];
                    int totalSizeUsed = vehiclesOnSpot.Sum(v => v.Size);

                    
                    if (totalSizeUsed >= maxSpotSize)
                    {
                        // Full plats (röd)
                        cellContent = $"[red]#{spotIndex + 1}\n{string.Join(",\n", vehiclesOnSpot.Select(v => Markup.Escape(v.RegistrationNumber)))}[/]";
                    }
                    else
                    {
                        // Delvis upptagen plats (gul)
                        cellContent = $"[yellow]#{spotIndex + 1}\n{string.Join(",\n", vehiclesOnSpot.Select(v => Markup.Escape(v.RegistrationNumber)))}[/]";
                    }
                }
                else
                {
                    // Tom plats (grön)
                    cellContent = $"[green]#{spotIndex + 1}\nEmpty[/]";
                }

                rowContent.Add(cellContent);

                // Skapa rad i tabellen när antalet kolumner har fyllts
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
                Console.WriteLine("Invalid number.");
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
                        if (parkingLot[newSpot].Count == 0 ||
                    (parkingLot[i][j] is MC && parkingLot[newSpot][0] is MC && parkingLot[newSpot].Count < 2))
                        {
                            parkingLot[newSpot].Add(parkingLot[i][j]);
                            parkingLot[i].RemoveAt(j);
                            Console.WriteLine($"Vehicle {registrationNumber} has been moved to spot {newSpot + 1}.");
                        }
                        else
                        {
                            Console.WriteLine("The new spot is not available.");
                        }
                        return;
                    }
                }
            }
            Console.WriteLine("The vehicle could not be found.");
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
                        Console.WriteLine($"Vehicle {registrationNumber} was found in spot {i + 1}.");
                        return;
                    }
                }
            }
            Console.WriteLine("The vehicle could not be found.");
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
                        int requiredSpots = (int)Math.Ceiling(vehicleToRemove.Size / (double)parkingSpotSize);

                        // Ta bort fordonet från alla platser det upptar
                        for (int k = 0; k < requiredSpots; k++)
                        {
                            if (i + k < parkingLot.Length)
                            {
                                parkingLot[i + k].RemoveAll(v => v.RegistrationNumber == registrationNumber);
                            }
                        }

                        Console.WriteLine($"Fordon {registrationNumber} har tagits bort från plats {i + 1} till {i + requiredSpots}.");
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
            Console.WriteLine("Parkinglot have been saved to file.");
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
                        else if (vehicleData.VehicleType == "BUS")
                        {
                            vehicle = new Bus(vehicleData.RegistrationNumber);
                        }
                        else if (vehicleData.VehicleType == "BICYCLE")
                        {
                            vehicle = new Bicycle(vehicleData.RegistrationNumber);
                        }
                        else
                        {
                            // Hantering för andra fordonstyper om det finns
                            continue; // Om vi inte kan hantera fordonstypen, hoppa över det
                        }

                        vehicle.ParkingTime = vehicleData.ParkingTime;
                        parkingLot[vehicleData.ParkingSpot].Add(vehicle);
                    }

                    Console.WriteLine("Parked vehicles have been loaded from file.");
                }
            }
        }
    }
}
