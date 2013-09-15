package problems;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.EulerMath;

/**
 * 
 For non-negative integers m, n, the Ackermann function A(m, n) is defined as
 * follows:
 * 
 * For example A(1, 0) = 2, A(2, 2) = 7 and A(3, 4) = 125.
 * 
 * Find Sum(A(n, n),n=0..6) and give your answer mod 14^8 = 1475789056.
 * 
 * @author nock
 * 
 */
public class Problem282 extends EulerProblem implements SolvableProblem {
	private static final int modulus = 1475789056;

	public class AckermanThread extends Thread implements Runnable {
		private final int index;
		private int m;
		private int n;
		public int A;
		private volatile int age;

		@Override
		public String toString() {
			if (done)
				synchronized (lock) {
					return String.format("A(%d,%d) = %d", m, n, age);
				}
			else
				return Integer.toString(A);
		}

		public AckermanThread(int i) {
			index = i;
			age = 0;
			lock = new Object();
			done = false;
		}

		private final static int OUT = 0;
		private final static int Neq0 = 1;
		private final static int gr0 = 2;
		private int state;
		private Object lock;
		private volatile boolean done;

		@Override
		public void run() {

		}

	}

	@Override
	public String solve() throws Exception {
		// AckermanThread[] threads = new Thread[7];
		// for (int i = 0; i < 7; i++) {
		// threads[i] = new AckermanThread(i);
		// }
		// for (int i = 0; i < 7; i++) {
		// threads[i].start();
		// }
		// boolean theShowMustGoOn = true;
		// while (theShowMustGoOn) {
		// StringBuilder builder = new StringBuilder();
		// for (int i = 0; i < 7; i++) {
		// theShowMustGoOn |= threads[i].isAlive();
		// builder.append(String.format("\t%s", threads[i]));
		// }
		// System.out.println(builder);
		// }
		for (int i = 0; i < 10; i++) {
			System.out.println(String.format("A(%d,%d) = %d", i, 0, EulerMath.A(i, 0)));
		}
		long sum = 1L;
		int tmp = 1;
		System.out.print(tmp);
		for (int i = 1; i < 7; i++) {
			int n = tmp;
			for (int j = 0; j < i + 1; j++) {
				try {
					n = EulerMath.A(i - 1, n);
				} catch (StackOverflowError ex) {
					return String.format("\nA(%d,%d)", i - 1, n);
				}
			}
			sum += n;
			System.out.print(String.format(" + %d", n));
		}
		System.out.println(String.format(" = %d %% %d = %d", sum, modulus, sum
				% modulus));
		return Long.toString(sum);
	}

	@Override
	public boolean hasBeenSolved() {
		// TODO Auto-generated method stub
		return false;
	}

}
