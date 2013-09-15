/**
 * 
 */
package problems.solved;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.Primus;
import euler.math.PrimusBase;

/**
 * @author nock The prime factors of 13195 are 5, 7, 13 and 29.
 * 
 *         What is the largest prime factor of the number 600851475143 ?
 */
public class Problem003 extends EulerProblem implements SolvableProblem {

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		System.out.println(max);
		final long maxFactor = (long) Math.floor((double) max / 3d);
		System.out.println(maxFactor);
		Thread.sleep(1000 * 3);
		PrimusBase primus = new MyPrimus(maxFactor);
		primus.seive();
		return primus.getAnswer();
	}

	static long max = 600851475143L;

	private class MyPrimus extends PrimusBase {
		private long total;

		public MyPrimus(long limit) {
			super(limit);
			total = 1L;
		}

		@Override
		public boolean seiveInteger(long i) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean seivePrime(long p) {
			if (max % p == 0) {
				answer = p;
				total *= p;

				System.out.println();
				System.out.print(p);
				System.out.print(" => ");
				System.out.println(total);
				if (total == max)
					return true;
			}
			System.out.print('.');
			return false;
		}
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#hasBeenSolved()
	 */
	@Override
	public boolean hasBeenSolved() {
		// TODO Auto-generated method stub
		return true;
	}

}
