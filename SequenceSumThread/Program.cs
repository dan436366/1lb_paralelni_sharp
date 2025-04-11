using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Write("Enter number of threads: ");
        int threadCount = int.Parse(Console.ReadLine());

        Random random = new Random();
        int[] steps = new int[threadCount];
        int[] delays = new int[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            steps[i] = random.Next(1, 6);
            delays[i] = random.Next(1, 6) * 1000;
        }

        SumThread[] workers = new SumThread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            workers[i] = new SumThread(i + 1, steps[i]);
            workers[i].Start();
        }

        ControllerThread controller = new ControllerThread(workers, delays);
        controller.Start();
        controller.Join();

    }
}

class SumThread
{
    private readonly int id;
    private readonly int step;
    private volatile bool running = true;

    private long sum = 0;
    private long count = 0;

    private Thread thread;

    public SumThread(int id, int step)
    {
        this.id = id;
        this.step = step;
    }

    public void Start()
    {
        thread = new Thread(Run);
        thread.Start();
    }

    private void Run()
    {
        int current = 0;

        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        while (running)
        {
            sum += current;
            current += step;
            count++;

            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - startTime >= 100) 
            {
                startTime = currentTime; 
            }
        }

        Console.WriteLine($"Thread #{id} sum = {sum}, addends = {count}");
    }

    public void StopRunning()
    {
        running = false;
    }

    public void Join()
    {
        thread.Join();
    }
}

class ControllerThread
{
    private readonly SumThread[] threads;
    private readonly int[] delays;
    private Thread controller;

    public ControllerThread(SumThread[] threads, int[] delays)
    {
        this.threads = threads;
        this.delays = delays;
    }

    public void Start()
    {
        controller = new Thread(Run);
        controller.Start();
    }

    public void Join()
    {
        controller.Join();
    }

    private void Run()
    {
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        bool[] stopped = new bool[threads.Length];

        while (true)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            bool allStopped = true;

            for (int i = 0; i < threads.Length; i++)
            {
                if (!stopped[i])
                {
                    allStopped = false;

                    if (currentTime - startTime >= delays[i])
                    {
                        threads[i].StopRunning();
                        stopped[i] = true;
                    }
                }
            }

            if (allStopped)
            {
                break;
            }
        }

        foreach (var t in threads)
        {
            t.Join();
        }
    }
}