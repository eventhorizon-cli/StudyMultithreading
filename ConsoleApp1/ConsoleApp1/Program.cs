using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numberOfOperations = 500;
            var sw = new Stopwatch();
            sw.Start();
            UseThreads(numberOfOperations);
            sw.Stop();
            Console.WriteLine($"Excution time using threads: {sw.ElapsedMilliseconds}");

            sw.Reset();
            sw.Start();
            UseThreadPool(numberOfOperations);
            sw.Stop();

            // 线程池在快要结束时创建更多的线程，但是仍然花费了更多的时间。我们为操作系统节省了内存和线程数，但是为此付出了更长的时间
            Console.WriteLine($"Excution time using threads: {sw.ElapsedMilliseconds}");
        }

        static void UseThreads(int numberOfOperations)
        {
            using (var countdown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Scheduling work by creating threads");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    var thread = new Thread(() =>
                    {
                        Console.Write($"{Thread.CurrentThread.ManagedThreadId}, ");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();
                    });
                    thread.Start();
                }

                countdown.Wait();
                Console.WriteLine();
            }
        }

        static void UseThreadPool(int numberOfOperations)
        {
            using (var countdown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Start working on a threadpool");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Console.Write($"{Thread.CurrentThread.ManagedThreadId}, ");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();
                    });
                }

                countdown.Wait();
                Console.WriteLine();
            }
        }
    }
}
