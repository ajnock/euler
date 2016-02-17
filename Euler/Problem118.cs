using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euler
{
    public class Problem118 : Problem
    {
        public override object Solve()
        {
            var digits = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var queue = new ConcurrentQueue<Tuple<int[], int[]>>();
            var nodes = Expand(new int[0], digits, queue);

            foreach (var node in nodes)
            {

            }

            return null;
        }

        private static IEnumerable<Tuple<int[], int[]>> Expand(int[] sets, int[] digits, ConcurrentQueue<Tuple<int[], int[]>> queue)
        {
            var task = Task.Run(() =>
            {
                Produce(sets, digits, queue);
            });

            Tuple<int[], int[]> value;
            while (task.Status == TaskStatus.Running)
            {
                while (queue.TryDequeue(out value))
                {
                    yield return value;
                }
            }

            while (queue.TryDequeue(out value))
            {
                yield return value;
            }
        }

        private static void Produce(int[] s, int[] o, ConcurrentQueue<Tuple<int[], int[]>> queue)
        {
            Parallel.ForEach(o, (d) =>
            {
                if (IsPrime(d))
                {
                    var sets = s.ToList();
                    var digits = o.ToList();
                    sets.Add(d);
                    digits.Remove(d);
                    Parallel.ForEach(Expand(sets.ToArray(), digits.ToArray(), queue), (node) =>
                    {
                        queue.Enqueue(node);
                    });
                }

                var next = o.Where(i => i != d);
                Parallel.ForEach(next, (i) =>
                {
                    int combo1 = int.Parse(i.ToString() + d);
                    int combo2 = int.Parse(d + i.ToString());
                    var sets = s.ToList();
                    var digits = o.ToList();

                    var tasks = new List<Task>() {
                        Task.Run(()=> {
                    if (IsPrime(combo1))
                    {
                        sets.Add(combo1);
                        digits.Remove(combo1);
                        Parallel.ForEach(Expand(sets.ToArray(), digits.ToArray(), queue), (node) =>
                        {
                            queue.Enqueue(node);
                        });
                    } }),
                        Task.Run(()=> {
                    if (IsPrime(combo2))
                    {
                        sets.Add(combo2);
                        digits.Remove(combo2);
                        Parallel.ForEach(Expand(sets.ToArray(), digits.ToArray(), queue), (node) =>
                        {
                            queue.Enqueue(node);
                        });
                    } }) };
                });
            });
        }
    }
}
