using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Euler
{
    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<object> queue = new BlockingCollection<object>();
        private static readonly ManualResetEvent signal = new ManualResetEvent(false);
        private static Worker worker;

        static NonBlockingConsole()
        {
            worker = new Worker();
        }

        public static void WriteLine(object value)
        {
            queue.Add(value);
        }

        public static void Flush()
        {
            worker.Flush();
        }

        private class Worker
        {
            private readonly CancellationTokenSource source;
            public readonly CancellationToken token;
            private readonly Task task;

            public Worker()
            {
                source = new CancellationTokenSource();
                token = source.Token;
                task = Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        object obj;
                        while (queue.TryTake(out obj))
                        {
                            Console.WriteLine(obj);
                        }
                    }

                    token.ThrowIfCancellationRequested();
                });
            }

            public void Flush()
            {
                source.Cancel();

                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var v in ex.InnerExceptions)
                        Console.WriteLine(ex.Message + " " + v.Message);
                }
                finally
                {
                    source.Dispose();
                }
            }
        }
    }
}
