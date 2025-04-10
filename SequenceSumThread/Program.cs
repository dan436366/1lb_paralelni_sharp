using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SequenceSumThread
{
    class SequenceSumThread
    {
        private int id;     
        private int step;         
        private bool running = true;
        private long sum = 0;  
        private int count = 0;    

        public SequenceSumThread(int id, int step)
        {
            this.id = id;
            this.step = step;
        }

        public void Stop() => running = false;

        public void Run()
        {
            int current = 0;
            while (running)
            {
                sum += current;  
                current += step; 
                count++;
                Thread.Sleep(50); 
            }

            Console.WriteLine($"Thread {id}: sum = {sum}, addends = {count}");
        }

        public static void Main(string[] args)
        {
            Console.Write("Enter number of threads: ");
            int numThreads = int.Parse(Console.ReadLine());

            Console.Write("Enter step of threads: ");
            int step = int.Parse(Console.ReadLine());

            SequenceSumThread[] workers = new SequenceSumThread[numThreads];
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                workers[i] = new SequenceSumThread(i + 1, step);
                threads[i] = new Thread(new ThreadStart(workers[i].Run));
                threads[i].Start();
            }

            for (int i = 0; i < numThreads; i++)
            {
                Thread.Sleep((i + 1) * 1000);
                workers[i].Stop();
            }
        }
    }
}
