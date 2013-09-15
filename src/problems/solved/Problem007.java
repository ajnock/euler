package problems.solved;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.PrimesFactory;

/**
 * 
 * By listing the first six prime numbers: 2, 3, 5, 7, 11, and 13, we can see
 * that the 6th prime is 13.
 * 
 * What is the 10 001st prime number?
 * 
 * @author nock
 * 
 */
public class Problem007 extends EulerProblem implements SolvableProblem {

	@Override
	public Object solve() throws Exception {
		// System.out.println(Integer.MAX_VALUE);
		// System.out.println(Integer.MIN_VALUE);
		// System.out.println(Integer.SIZE);
		PrimesFactory factory = new PrimesFactory(104743 + 1);
		return Integer.toString(factory.getPrime(10001));
	}

	@Override
	public boolean hasBeenSolved() {
		return true;
	}

}
