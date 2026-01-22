using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Shared data
    static Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
    static object locker = new object();

    static double globalMin = double.MaxValue;
    static double globalMax = double.MinValue;

    static bool processing = true;

    static void Main()
    {
        // Task 3: "Please wait" message (Timer)
        Timer timer = new Timer(
            _ => Console.WriteLine("Processing... please wait"),
            null,
            0,
            1000);

        // Task 1: Data generation in separate THREAD
        Thread generatorThread = new Thread(GenerateData);
        generatorThread.Start();
        generatorThread.Join(); // wait until generation ends

        // Task 2: Analysis using TASKS
        AnalyzeData();

        processing = false;
        timer.Dispose();

        Console.WriteLine("\nGlobal Minimum Weight: " + globalMin);
        Console.WriteLine("Global Maximum Weight: " + globalMax);
    }

    // ================= TASK 1 =================
    static void GenerateData()
    {
        Random rand = new Random();
        int fishermenCount = 16;
        int totalFish = 75;

        // Create fishermen
        for (int i = 1; i <= fishermenCount; i++)
        {
            data[$"Fisherman{i}"] = new List<double>();
        }

        // Generate fish weights
        for (int i = 0; i < totalFish; i++)
        {
            string fisherman = $"Fisherman{rand.Next(1, fishermenCount + 1)}";
            double weight = Math.Round(rand.NextDouble() * 20 + 1, 2);

            lock (locker)
            {
                data[fisherman].Add(weight);
            }

            Thread.Sleep(50); // slow down to see "please wait"
        }
    }

    // ================= TASK 2 =================
    static void AnalyzeData()
    {
        List<Task> tasks = new List<Task>();

        foreach (var fisherman in data)
        {
            tasks.Add(Task.Run(() =>
            {
                List<double> weights;

                lock (locker)
                {
                    weights = fisherman.Value.ToList();
                }

                if (weights.Count == 0) return;

                double avg = weights.Average();
                double min = weights.Min();
                double max = weights.Max();

                lock (locker)
                {
                    if (min < globalMin) globalMin = min;
                    if (max > globalMax) globalMax = max;
                }

                Console.WriteLine(
                    $"{fisherman.Key,-12} | Avg: {avg,6:F2} | Min: {min,6:F2} | Max: {max,6:F2}");
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }
}
