/**
 * 
 */
package problems.solved;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.Queue;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.sequence.CollatzSequence;

/**
 * @author nock
 * 
 * 
 *         The following iterative sequence is defined for the set of positive
 *         integers:
 * 
 *         n → n/2 (n is even) n → 3n + 1 (n is odd)
 * 
 *         Using the rule above and starting with 13, we generate the following
 *         sequence: 13 → 40 → 20 → 10 → 5 → 16 → 8 → 4 → 2 → 1
 * 
 *         It can be seen that this sequence (starting at 13 and finishing at 1)
 *         contains 10 terms. Although it has not been proved yet (Collatz
 *         Problem), it is thought that all starting numbers finish at 1.
 * 
 *         Which starting number, under one million, produces the longest chain?
 * 
 *         NOTE: Once the chain starts the terms are allowed to go above one
 *         million.
 * 
 *         837799
 * 
 */
public class Problem014 extends EulerProblem implements SolvableProblem {

	final HashMap<Long, Long> vectors;

	/**
	 * 
	 */
	public Problem014() {
		vectors = new HashMap<Long, Long>();
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		for (long i = 2; i < 1000000; i++) {
			// System.out.println(i);
			MyCollatz mc = new MyCollatz(i);
			mc.run();
		}

		long max = 0L;
		long index = 0L;
		for (long i : vectors.keySet()) {
			long tmp = vectors.get(i);
			if (tmp > max) {
				max = tmp;
				index = i;
				System.out.println(tmp);
			}
		}

		return String.format("C(%d) = %d", index, max);
	}

	private class MyCollatz extends CollatzSequence {
		Queue<Long> sequence;

		public MyCollatz(long i) {
			super(i);
			sequence = new LinkedList<Long>();
			sequence.add(i);
		}

		@Override
		public boolean seive(long i) {
			sequence.add(i);
			if (i == 1) {
				vectors.put(seed, (long) sequence.size());
			} else if (vectors.containsKey(i)) {
				vectors.put(seed, sequence.size() + vectors.get(i));
			} else {
				// System.out.print(" => ");
				// System.out.print(i);
				return false;
			}
			System.out.print(seed);
			System.out.print(" = ");
			System.out.print(vectors.get(i));
			System.out.println();
			return true;
		}
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#hasBeenSolved()
	 */
	@Override
	public boolean hasBeenSolved() {
		return true;
	}

}
