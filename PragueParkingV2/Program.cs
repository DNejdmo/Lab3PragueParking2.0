using Spectre.Console;
using System.Text.Json;

using PragueParkingAccess;
class Program
{
    static void Main(string[] args)
    {
        // Läs in konfigurationsdata från JSON-fil
        string configFilePath = "../../../config.json";
        Configuration config = LoadConfiguration(configFilePath);

        //Läser in prislistan
        string priceListFilePath = "../../../pricelist.txt";
        Dictionary<string, int> priceList = LoadPriceList(priceListFilePath);

        if (config != null && priceList != null)
        {


            ParkingGarage garage = new ParkingGarage(config.ParkingSpots, config.ParkingSpotSize, config.VehicleTypes);

            //Show menu
            bool exit = false;
            while (!exit)
            {
                string choice = AnsiConsole.Prompt(
                   new SelectionPrompt<string>()
                       .Title("[bold blue]Choose an alternative:[/]")
                       .AddChoices(new[]
                       {
                            "1. Park a vehicle",
                            "2. Show parkingspots",
                            "3. Move a vehicle",
                            "4. Find a vehicle",
                            "5. Remove a vehicle",
                            "6. Edit the pricelist",
                            "7. EXIT"
                       }));


                //Manage menu selections
                switch (choice[0])
                {
                    case '1':
                        // Låt användaren välja fordonstyp med piltangenter och Enter
                        string vehicleType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Choose vehicletype:")
                                .AddChoices("CAR", "MC")
                                .HighlightStyle(new Style(Color.Blue, decoration: Decoration.Bold)));


                        // Be om registreringsnummer
                        string registrationNumber;
                        do
                        {
                            Console.WriteLine("Enter registration number (max. 10 characters):");
                            registrationNumber = Console.ReadLine().ToUpper();

                            if (registrationNumber.Length > 10)
                            {
                                Console.WriteLine("The registration number may be a maximum of 10 characters. Try again.");
                            }

                        } while (registrationNumber.Length > 10);

                        // Skapa fordonet baserat på det valda valet
                        Vehicle vehicle;
                        if (vehicleType == "CAR")
                        {
                            vehicle = new Car(registrationNumber);
                        }
                        else
                        {
                            vehicle = new MC(registrationNumber);
                        }

                        garage.ParkVehicle(vehicle);
                        break;

                    case '2':
                        garage.ShowParkingLot();
                        break;

                    case '3':
                        Console.WriteLine("Enter the registration number of the vehicle to be moved:");
                        string regToMove = Console.ReadLine().ToUpper();
                        Console.WriteLine("Enter new parking spot to move the vehicle to:");
                        int newSpot = int.Parse(Console.ReadLine());
                        garage.MoveVehicle(regToMove, newSpot);
                        AnsiConsole.MarkupLine("[blue]Press a key to return to the menu...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case '4':
                        Console.WriteLine("Enter the registration number of the vehicle you are looking for:");
                        string regToFind = Console.ReadLine().ToUpper();
                        garage.FindVehicle(regToFind);
                        AnsiConsole.MarkupLine("[blue]Press a key to return to the menu...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case '5':
                        Console.WriteLine("Enter the registration number of the vehicle that is to leave the parking lot:");
                        string regNumber = Console.ReadLine().ToUpper();

                        Vehicle vehicleToRemove = garage.RemoveVehicle(regNumber);
                        if (vehicleToRemove != null)
                        {
                            int price = CalculateParkingPrice(vehicleToRemove, priceList);
                            Console.WriteLine($"Vehicle {vehicleToRemove.RegistrationNumber} must pay {price} CZK.");
                        }
                        AnsiConsole.MarkupLine("[blue]Press a key to return to the menu...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case '6':
                        EditPriceList(priceListFilePath, priceList);
                        AnsiConsole.MarkupLine("[blue]Press a key to return to the menu...[/]");
                        Console.ReadKey();
                        AnsiConsole.Clear();
                        break;

                    case '7':
                        exit = true;
                        AnsiConsole.MarkupLine("[bold red]Exiting the program...[/]");
                        break;

                    default:
                        Console.WriteLine("Incorrectly choice, try again.");
                        break;
                }
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to load configuration.[/]");
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
            Console.WriteLine($"Error loading configuration file: {ex.Message}");
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
            Console.WriteLine($"Error loading the price list: {ex.Message}");
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
        Console.WriteLine($"No price information available for the vehicle type {vehicle.VehicleType}.");
        return 0;
    }

    //Metod för att editera prislistan (Menyval 7)
    static void EditPriceList(string priceListFilePath, Dictionary<string, int> priceList)
    {
        Console.WriteLine("\n--- EDIT PRICE LIST ---");

        foreach (var vehicleType in priceList.Keys)
        {
            Console.WriteLine($"Current price for {vehicleType}: {priceList[vehicleType]} CZK by the hour.");
            Console.Write($"Enter new price for {vehicleType}: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int newPrice))
            {
                priceList[vehicleType] = newPrice;
                Console.WriteLine($"The price for {vehicleType} has been updated to {newPrice} CZK.");
            }
            else
            {
                Console.WriteLine("Invalid price, please try again.");
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
        Console.WriteLine("The price list has been edited.");
    }


}

