﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Euler
{
    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<Tuple<object, bool>> queue = new BlockingCollection<Tuple<object, bool>>();
        private static readonly object flushMutex = new object();
        private static readonly ManualResetEvent flushingSignal = new ManualResetEvent(true);
        private static Worker worker;

        static NonBlockingConsole()
        {
            worker = new Worker();
        }

        /// <summary>
        /// Writes to the console in a non-blocking manner
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(object value = null)
        {
            flushingSignal.WaitOne();
            queue.Add(new Tuple<object, bool>(value, true));
        }

        /// <summary>
        /// Writes to the console in a non-blocking manner
        /// </summary>
        /// <param name="value"></param>
        public static void Write(object value = null)
        {
            flushingSignal.WaitOne();
            queue.Add(new Tuple<object, bool>(value, false));
        }

        /// <summary>
        /// Blocks until the queue has been flushed to the console
        /// </summary>
        public static void Flush()
        {
            flushingSignal.Reset();
            lock (flushMutex)
            {
                worker.Flush();
                worker.Dispose();
                flushingSignal.Set();
                worker = new Worker();
            }
        }

        private class Worker : IDisposable
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
                        Tuple<object, bool> item;
                        while (queue.TryTake(out item))
                        {
                            var obj = item.Item1;
                            if (item.Item2)
                            {
                                Console.WriteLine(obj);
                            }
                            else
                            {
                                Console.Write(obj);
                            }
                        }
                    }

                    token.ThrowIfCancellationRequested();
                }, token);
            }

            public void Flush()
            {
                source.Cancel();

                try
                {
                    disposedValue = true;
                    task.Wait();
                }
                catch (AggregateException)
                {
                    //Console.WriteLine("Flushed");
                }
                finally
                {
                    source.Dispose();
                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        source.Dispose();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~Worker() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
