using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Shared data: Plant -> Gas readings
    static Dictionary<int, List<double>> gasData = new Dictionary<int, List<double>>();
    static object locker = new object();

    static double globalMin = double.MaxValue;
    static double globalMax = double.MinValue;

    static bool working = true;

    static void Main()
    {
        Console.WriteLine("Gas consumption analysis started...\n");

        // Task 3: waiting message (Thread)
        Thread waitThread = new Thread(ShowWaitingMessage);
        waitThread.Start();

        // Task 1: data generation (Thread)
        Thread generatorThread = new Thread(GenerateGasData);
        generatorThread.Start();
        generatorThread.Join();

        // Task 2: data analysis (Tasks)
        AnalyzeGasData();

        working = false;
        waitThread.Join();

        Console.WriteLine("\nGlobal Minimum Consumption: " + globalMin.ToString("F2"));
        Console.WriteLine("Global Maximum Consumption: " + globalMax.ToString("F2"));
        Console.WriteLine("\nAnalysis finished.");
    }

    // ================= TASK 1 =================
    static void GenerateGasData()
    {
        Random rand = new Random();

        for (int plant = 1; plant <= 5; plant++)
        {
            gasData[plant] = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                double value = rand.NextDouble() * 200 + 200; // 200–400
                gasData[plant].Add(value);
                Thread.Sleep(50); // slow down
            }
        }
    }

    // ================= TASK 2 =================
    static void AnalyzeGasData()
    {
        List<Task> tasks = new List<Task>();

        foreach (var plant in gasData)
        {
            tasks.Add(Task.Run(() =>
            {
                double avg = plant.Value.Average();
                double min = plant.Value.Min();
                double max = plant.Value.Max();

                lock (locker)
                {
                    if (min < globalMin) globalMin = min;
                    if (max > globalMax) globalMax = max;
                }

                Console.WriteLine(
                    $"Plant {plant.Key} | Avg: {avg:F2} | Min: {min:F2} | Max: {max:F2}");
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    // ================= TASK 3 =================
    static void ShowWaitingMessage()
    {
        while (working)
        {
            Console.WriteLine("Processing data... please wait");
            Thread.Sleep(1000);
        }
    }
}
