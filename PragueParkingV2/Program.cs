using PragueParkingV2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Spectre.Console;


class Program
{
    static void Main(string[] args)
    {
        // Läs in konfigurationsdata från JSON-fil
        string configFilePath = "../../../config.json";
        Configuration config = LoadConfiguration(configFilePath);

        string priceListFilePath = "../../../pricelist.txt";
        Dictionary<string, int> priceList = LoadPriceList(priceListFilePath);

        if (config != null && priceList != null)
        {
           

            ParkingGarage garage = new ParkingGarage(config.ParkingSpots, config.VehicleTypes);
            bool exit = false;
            while (!exit)
            {
                string choice = AnsiConsole.Prompt(
                   new SelectionPrompt<string>()
                       .Title("[bold blue]Välj ett alternativ:[/]")
                       .AddChoices(new[]
                       {
                            "1. Parkera ett fordon",
                            "2. Visa parkeringsplatser",
                            "3. Flytta ett fordon",
                            "4. Leta efter ett fordon",
                            "5. Ta bort ett fordon",
                            "6. Redigera prislista",
                            "7. Avsluta"
                       }));

            switch (choice[0]) // Kollar första tecknet för valet
            {
                case '1':
                    Console.WriteLine("Ange fordonstyp (CAR/MC):");
                    string vehicleType = Console.ReadLine().ToUpper();

                    string registrationNumber;
                    do
                    {
                        Console.WriteLine("Ange registreringsnummer (max 10 tecken):");
                        registrationNumber = Console.ReadLine().ToUpper();

                        if (registrationNumber.Length > 10)
                        {
                            Console.WriteLine("Registreringsnumret får vara max 10 tecken. Försök igen.");
                        }

                    } while (registrationNumber.Length > 10);

                    Vehicle vehicle;
                    if (vehicleType == "CAR")
                    {
                        vehicle = new Car(registrationNumber);
                    }
                    else if (vehicleType == "MC")
                    {
                        vehicle = new MC(registrationNumber);
                    }
                    else
                    {
                        Console.WriteLine("Ogiltig fordonstyp.");
                        break;
                    }

                    garage.ParkVehicle(vehicle);
                    break;

                case '2':
                    garage.ShowParkingLot();
                    break;

                case '3':
                    Console.WriteLine("Ange registreringsnummer för fordonet som ska flyttas:");
                    string regToMove = Console.ReadLine().ToUpper();
                    Console.WriteLine("Ange ny plats att flytta fordonet till:");
                    int newSpot = int.Parse(Console.ReadLine());
                    garage.MoveVehicle(regToMove, newSpot);
                    break;

                case '4':
                    Console.WriteLine("Ange registreringsnummer för fordonet du söker:");
                    string regToFind = Console.ReadLine().ToUpper();
                    garage.FindVehicle(regToFind);
                    break;

                case '5':
                    Console.WriteLine("Ange registreringsnummer på fordonet som ska lämna:");
                    string regNumber = Console.ReadLine().ToUpper();

                    Vehicle vehicleToRemove = garage.RemoveVehicle(regNumber);
                    if (vehicleToRemove != null)
                    {
                        int price = CalculateParkingPrice(vehicleToRemove, priceList);
                        Console.WriteLine($"Fordonet {vehicleToRemove.RegistrationNumber} ska betala {price} CZK.");
                    }
                    break;

                case '6':
                    EditPriceList(priceListFilePath, priceList);
                    break;

                case '7':
                    exit = true;
                    AnsiConsole.MarkupLine("[bold red]Avslutar programmet...[/]");
                    break;

                default:
                    Console.WriteLine("Felaktigt val, försök igen.");
                    break;
            }
        }
    }
        else
        {
            AnsiConsole.MarkupLine("[red]Misslyckades med att läsa in konfigurationen.[/]");
        }
    }


    // Metod för att läsa in JSON-konfiguration
    public static Configuration LoadConfiguration(string filePath)
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Configuration>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid inläsning av konfigurationsfil: {ex.Message}");
            return null;
        }
    }
     

    //Metod för att läsa in prislistan
    public static Dictionary<string, int> LoadPriceList(string filePath)
    {
        var priceList = new Dictionary<string, int>();

        try
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int price))
                    {
                        priceList[parts[0]] = price;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid inläsning av prislistan: {ex.Message}");
        }

        return priceList;
    }

    //Metod för att beräkna priset.
    public static int CalculateParkingPrice(Vehicle vehicle, Dictionary<string, int> priceList)
    {
        TimeSpan parkingDuration = DateTime.Now - vehicle.ParkingTime;

        // Om parkeringstiden är mindre än eller lika med 10 minuter, är det gratis
        if (parkingDuration.TotalMinutes <= 10)
        {
            return 0;
        }

        // Hämta pris per timme baserat på fordonstyp
        if (priceList.TryGetValue(vehicle.VehicleType, out int hourlyRate))
        {
            // Beräkna antalet timmar och runda upp till närmaste timme
            int hoursParked = (int)Math.Ceiling(parkingDuration.TotalHours);
            return hoursParked * hourlyRate;
        }

        // Om fordonstypen inte finns i prislistan
        Console.WriteLine($"Ingen prisinformation tillgänglig för fordonstypen {vehicle.VehicleType}.");
        return 0;
    }

    //Metod för att editera prislistan (Menyval 7)
    static void EditPriceList(string priceListFilePath, Dictionary<string, int> priceList)
    {
        Console.WriteLine("\n--- REDIGERA PRISLISTA ---");

        foreach (var vehicleType in priceList.Keys)
        {
            Console.WriteLine($"Nuvarande pris för {vehicleType}: {priceList[vehicleType]} CZK per timme.");
            Console.Write($"Ange nytt pris för {vehicleType}: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int newPrice))
            {
                priceList[vehicleType] = newPrice;
                Console.WriteLine($"Priset för {vehicleType} har uppdaterats till {newPrice} CZK.");
            }
            else
            {
                Console.WriteLine("Ogiltigt pris, försök igen.");
            }
        }

        // Spara den uppdaterade prislistan till fil
        SavePriceList(priceListFilePath, priceList);
    }

    //Metod för att spara en ny prislista (Del av menyval 7)
    static void SavePriceList(string filePath, Dictionary<string, int> priceList)
    {
        var lines = new List<string>();

        foreach (var item in priceList)
        {
            lines.Add($"{item.Key}={item.Value}");  // Skriver fordonstyp=pris
        }

        File.WriteAllLines(filePath, lines);
        Console.WriteLine("Prislistan har sparats.");
    }


}

