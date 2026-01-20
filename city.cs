using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static Dictionary<int, List<double>> weatherData = new Dictionary<int, List<double>>();
    static bool working = true;

    static void Main()
    {
        Console.WriteLine("Weather analysis started...\n");

        // ===== TASK 3: waiting message =====
        Thread waitThread = new Thread(ShowWaitingMessage);
        waitThread.Start();

        // ===== TASK 1: generate data =====
        Thread generatorThread = new Thread(GenerateWeatherData);
        generatorThread.Start();
        generatorThread.Join();

        // ===== TASK 2: analyze data =====
        AnalyzeWeatherData();

        // stop waiting message
        working = false;
        waitThread.Join();

        Console.WriteLine("\nWeather analysis finished.");
    }

    // ================= TASK 1 =================
    static void GenerateWeatherData()
    {
        Random rand = new Random();

        for (int city = 1; city <= 5; city++)
        {
            weatherData[city] = new List<double>();

            for (int i = 0; i < 20; i++)
            {
                double temp = rand.NextDouble() * 40; // 0–40°C
                weatherData[city].Add(temp);
                Thread.Sleep(50); // slow down generation
            }
        }
    }

    // ================= TASK 2 =================
    static void AnalyzeWeatherData()
    {
        List<Task> tasks = new List<Task>();

        foreach (var city in weatherData)
        {
            tasks.Add(Task.Run(() =>
            {
                double avg = city.Value.Average();
                double min = city.Value.Min();
                double max = city.Value.Max();

                Console.WriteLine(
                    $"City {city.Key} | Avg: {avg:F1}°C | Min: {min:F1}°C | Max: {max:F1}°C");
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    // ================= TASK 3 =================
    static void ShowWaitingMessage()
    {
        while (working)
        {
            Console.WriteLine("Processing weather data... please wait");
            Thread.Sleep(1000);
        }
    }
}
