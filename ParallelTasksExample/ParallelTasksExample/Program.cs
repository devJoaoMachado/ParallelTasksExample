using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTasksExample
{
    class Program
    {


        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ExecuteNonParallel(stopwatch);

            ExecuteWithParallelFor(stopwatch);

            ExecuteWithThreadPool(stopwatch);

            ExecuteAsyncTasks(stopwatch);

            ExecuteWithThreadClass(stopwatch);

            Console.ReadKey();

        }

        private static void ExecuteNonParallel(Stopwatch stopwatch)
        {
            stopwatch.Restart();

            Console.WriteLine(" Non Parallel Tasks");

            for (int i = 0; i < 3; i++)
            {
                DownloadFile();
            }

            stopwatch.Stop();

            Console.WriteLine($"Total time(s) {stopwatch.ElapsedMilliseconds / 1000}");
        }

        private static void ExecuteWithParallelFor(Stopwatch stopwatch)
        {
            stopwatch.Restart();

            Console.WriteLine("\n \n Parallel Tasks with Parallel");

            Parallel.For(0, 3, i =>
            {
                DownloadFile();
            });

            Console.WriteLine($"Total time(s) {stopwatch.ElapsedMilliseconds / 1000}");

            stopwatch.Stop();
        }

        private static void ExecuteWithThreadPool(Stopwatch stopwatch)
        {
            stopwatch.Restart();

            EventWaitHandle resetEvent1 = new ManualResetEvent(false);
            EventWaitHandle resetEvent2 = new ManualResetEvent(false);

            Console.WriteLine("\n \n Parallel Tasks with ThreadPool");
            
            ThreadPool.QueueUserWorkItem(t1 => { DownloadFile(); resetEvent1.Set(); });
            ThreadPool.QueueUserWorkItem(t2 => { WritingOnDisk(); resetEvent2.Set(); });
            
            resetEvent1.WaitOne();
            resetEvent2.WaitOne();

            stopwatch.Stop();

            Console.WriteLine($"Total time(s) {stopwatch.ElapsedMilliseconds / 1000}");
        }

        private static void ExecuteAsyncTasks(Stopwatch stopwatch)
        {
            stopwatch.Restart();

            Console.WriteLine("\n \n Parallel Tasks with async await");

            var taskDownload = Task.Run(() => DownloadFile());
            var taskWriting = Task.Run(() => WritingOnDisk());

            Task.WhenAll(taskDownload, taskWriting);

            Console.WriteLine($"Total time(s) {stopwatch.ElapsedMilliseconds / 1000}");

            stopwatch.Stop();
        }

        private static void ExecuteWithThreadClass(Stopwatch stopwatch)
        {
            stopwatch.Restart();

            Console.WriteLine("\n \n Parallel Tasks with Thread class");

            ThreadStart startDownload = new ThreadStart(DownloadFile);
            ThreadStart startWriting = new ThreadStart(WritingOnDisk);

            Thread threadDownload = new Thread(startDownload);
            threadDownload.Start();
            Thread threadWriting = new Thread(startWriting);
            threadWriting.Start();

            threadDownload.Join();
            threadWriting.Join();

            Console.WriteLine($"Total time(s) {stopwatch.ElapsedMilliseconds / 1000}");

            stopwatch.Stop();
        }

        static void DownloadFile()
        {
            string file = "Doc_" + new Random().Next(1, 1000);
            Console.WriteLine($"Thread: { Thread.CurrentThread.ManagedThreadId } Downloading file {file}... ");
            Thread.Sleep(2000);
            Console.WriteLine("Done.");
        }

        static void WritingOnDisk()
        {
            string file = "File_" + new Random().Next(1, 1000);
            Console.WriteLine($"Thread: { Thread.CurrentThread.ManagedThreadId } Writing file {file}... ");
            Thread.Sleep(1500);
            Console.WriteLine("Done.");
        }



    }
}
