/**
 * 
 */
package problems;

import java.awt.List;
import java.util.ArrayList;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.Primus;

/**
 * @author nock
 * 
 */
public class Problem058 extends EulerProblem implements SolvableProblem {

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		long side = 5555L;
		long limit = side * side;
		Primus primus = new EulerPrimus(limit);
		primus.seive();
		return primus.getAnswer().toString();
	}

	private class EulerPrimus extends Primus {
		long sqrRoot;
		long sqr;
		int primes;
		int diagonals;

		long lowerLeft;
		long upperLeft;
		long upperRight;

		public EulerPrimus(long limit) {
			super(limit);
			sqrRoot = 3L;
			sqr = sqrRoot * sqrRoot;
			diagonals = 5;
			primes = 3;
		}

		@Override
		public boolean seiveInteger(long i) {
			if (i == sqr) {
				// clean?
				// System.gc();

				// compute
				this.answer = sqrRoot;
				double ratio = (double) primes / diagonals;

				// report
				System.out.println(String.format(
						"f(%d^2) = f(%d) = %d/%d = %f%%", sqrRoot, sqr, primes,
						diagonals, 100d * ratio));

				// increment
				increment();
				return ratio < 0.10;
			}
			return false;
		}

		private void increment() {
			diagonals += 4;
			sqrRoot += 2L;
			sqr = sqrRoot * sqrRoot;
			long delta = sqrRoot - 1L;
			lowerLeft = sqr - delta;
			upperLeft = lowerLeft - delta;
			upperRight = upperLeft - delta;
		}

		public boolean seivePrime(long p) {
			if (p == lowerLeft || p == upperLeft || p == upperRight)
				primes++;

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
		return false;
	}

}
