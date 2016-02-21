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
        private static BlockingCollection<object> m_Queue = new BlockingCollection<object>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
              () =>
              {
                  using (var writer = new StreamWriter(File.Open(@"D:\Primes.txt", FileMode.Append, FileAccess.Write, FileShare.Read)))
                  {
                      while (true)
                      {
                          var p = m_Queue.Take();

                          writer.WriteLine(p);
                          Console.WriteLine(p);
                      }
                  }
              });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void WriteLine(object value)
        {
            m_Queue.Add(value);
        }
    }
}
