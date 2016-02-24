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
        private static BlockingCollection<object> queue = new BlockingCollection<object>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
              () =>
              {
                  while (true)
                  {
                      var obj = queue.Take();
                      Console.WriteLine(obj);
                  }
              });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void WriteLine(object value)
        {
            queue.Add(value);
        }
    }
}
