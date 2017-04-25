/**
 * 
 */
package problems;

import euler.EulerProblem;

/**
 * For any N, let f(N) be the last five digits before the trailing zeroes in N!.
 * For example,
 * 
 * 9! = 362880 so f(9)=36288
 * 
 * 10! = 3628800 so f(10)=36288
 * 
 * 20! = 2432902008176640000 so f(20)=17664
 * 
 * Find f(1,000,000,000,000)
 * 
 * @author nock
 * 
 */
public class Problem160 extends EulerProblem {

	final static long max = 10 ^ 6;

	@Override
	public Object solve() throws Exception {
		long x = 1;
		for (int i = 0; i < 10; i++) {
			for (int j = 1; j < 10; j++) {
				x += j * (10 ^ i);
			}
		}
		return Long.toString(x);
	}
	
	public long f(long n) {
		long x = 1;
		for (int i = 0; i < 10; i++) {
			for (int j = 1; j < 10; j++) {
				x += j * (10 ^ i);
			}
		}
		return x;
	}

	@Override
	public boolean hasBeenSolved() {
		return false;
	}

}
