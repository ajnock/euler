/**
 * 
 */
package problems;

import euler.EulerProblem;
import euler.SolvableProblem;

/**
 * @author nock
 * 
 * 
 * 
 *         Let p(n) represent the number of different ways in which n coins can
 *         be separated into piles. For example, five coins can separated into
 *         piles in exactly seven different ways, so p(5)=7.
 * 
 *         OOOOO | OOOO O | OOO OO | OOO O O | OO OO O | OO O O O | O O O O O
 * 
 *         Find the least value of n for which p(n) is divisible by one million.
 */
public class Problem078 extends EulerProblem implements SolvableProblem {

	/**
	 * 
	 */
	public Problem078() {
		// TODO Auto-generated constructor stub
	}

	public static Partition[] generatePartitions(int n) {
		Partition[] partitions = new Partition[n];
		Thread[] threads = new Thread[n - 2];
		for (int i = 0; i < threads.length; i++) {
			for (int j = 0; j < n / i; j++) {

			}
		}
		return partitions;
	}

	public class Partition {

		private int n;
		private int[] bits;

		public int getN() {
			return n;
		}

		public int[] getBits() {
			return bits;
		}

		public Partition(int size) {
			n = size;
			bits = new int[n];
		}

		public boolean isLegal() {
			int sum = 0;
			for (int i = 0; i < n; i++) {
				sum += bits[i];
				if (sum > n) {
					return false;
				}
			}
			return sum == n;
		}

		@Override
		public boolean equals(Object obj) {
			try {
				Partition partition = (Partition) obj;
				if (partition.n == this.n) {
					for (int i = 0; i < n; i++) {
						if (partition.bits[i] != bits[i]) {
							return false;
						}
					}
					return true;
				}
				return false;
			} catch (Exception ex) {
				return false;
			}
		}

		@Override
		public int hashCode() {
			int sum = 17;
			for (int i = 0; i < n; i++) {
				sum += 71 * bits[i];
			}

			for (int i = n; i > 0; i--) {
				sum += 73 * bits[i - 1];
			}
			return sum;
		}
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see problems.SolvableProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		int i = 0;
		boolean solved = false;
		while (!solved && i <= Integer.MAX_VALUE) {
			int partitions = p(i++);
			System.out.println("p(" + Integer.toString(i) + ") = "
					+ Integer.toString(partitions));
			solved = partitions % 1000000 == 0;
		}
		return Integer.toString(i);
	}

	private int p(int n) {
		return n;
	}

	@Override
	public boolean hasBeenSolved() {
		return false;
	}

}
