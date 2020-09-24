using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskStopTesting
{
    class Program
    {
        private static CancellationTokenSource Canceller { get; set; }

        static void DoWork()
        {
            for (int i = 0; i < 10000; i++)
                Console.WriteLine("value" + i);

            Console.WriteLine("Task finished!");
        }

        static void Main(string[] args)
        {
            Canceller = new CancellationTokenSource();

            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                    {
                        DoWork();
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }, Canceller.Token);

            Canceller.CancelAfter(1000);

            try
            {
                task.Wait(Canceller.Token);
            }
            catch (OperationCanceledException)
            {

            }

            Console.WriteLine();
            Console.WriteLine("is Completed: {0}", task.IsCompleted);
            Console.WriteLine("is Canceled: {0}", task.IsCanceled);
            Console.WriteLine("is Faulted: {0}", task.IsFaulted);
            Console.WriteLine();
            Console.WriteLine("is CanBeCanceled: {0}", Canceller.Token.CanBeCanceled);
            Console.WriteLine("is CancellationRequested: {0}", Canceller.Token.IsCancellationRequested);

            Console.ReadLine();
        }
    }
}
