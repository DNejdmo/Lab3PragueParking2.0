using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // Läs in konfigurationsdata från JSON-fil
        string configFilePath = "../../../config.json";  // Sökvägen till JSON-filen
        Configuration config = LoadConfiguration(configFilePath);

        if (config != null)
        {
            Console.WriteLine($"Antal parkeringsplatser: {config.ParkingSpots}");
            Console.WriteLine("Fordonstyper och deras storlekar:");
            foreach (var vehicleType in config.VehicleTypes)
            {
                Console.WriteLine($"- {vehicleType.Type}: {vehicleType.Size}");
            }

            ParkingGarage garage = new ParkingGarage(config.ParkingSpots, config.VehicleTypes);
            bool exit = false;

            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
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

                        garage.ParkVehicle(vehicleType, registrationNumber);
                        break;

                    case "2":
                        garage.ShowParkingLot();
                        break;

                    case "3":
                        Console.WriteLine("Ange registreringsnummer för fordonet som ska flyttas:");
                        string regToMove = Console.ReadLine().ToUpper();

                        Console.WriteLine("Ange ny plats att flytta fordonet till:");
                        int newSpot = int.Parse(Console.ReadLine());

                        garage.MoveVehicle(regToMove, newSpot);
                        break;

                    case "4":
                        Console.WriteLine("Ange registreringsnummer för fordonet du söker:");
                        string regToFind = Console.ReadLine().ToUpper();

                        garage.FindVehicle(regToFind);
                        break;

                    case "5":
                        Console.WriteLine("Ange registreringsnummer för fordonet som ska tas bort:");
                        string regToRemove = Console.ReadLine().ToUpper();

                        garage.RemoveVehicle(regToRemove);
                        break;

                    case "6":
                        exit = true;
                        Console.WriteLine("Avslutar programmet...");
                        break;

                    default:
                        Console.WriteLine("Felaktigt val, försök igen.");
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine("Misslyckades med att läsa in konfigurationen.");
        }
    }

    // Metod för att visa menyn
    static void ShowMenu()
    {
        Console.WriteLine("\n--- PARKERINGSGARAGE MENY ---");
        Console.WriteLine("1. Parkera ett fordon");
        Console.WriteLine("2. Visa parkeringsplatser");
        Console.WriteLine("3. Flytta ett fordon");
        Console.WriteLine("4. Leta efter ett fordon");
        Console.WriteLine("5. Ta bort ett fordon");
        Console.WriteLine("6. Avsluta");
        Console.WriteLine("Välj ett alternativ:");
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
}

