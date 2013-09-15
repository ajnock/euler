/**
 * 
 */
package problems.solved;

import euler.EulerProblem;

/**
 * If we list all the natural numbers below 10 that are multiples of 3 or 5, we
 * get 3, 5, 6 and 9. The sum of these multiples is 23.
 * 
 * Find the sum of all the multiples of 3 or 5 below 1000.
 * 
 * @author nock
 */
public class Problem001 extends EulerProblem {
	
	public Object solve() {
		int multipleOf3 = 3;
		int multipleOf5 = 5;
		int i = 1;
		int sum = 0;

		while (multipleOf3 < 1000) {
			sum += multipleOf3;
			multipleOf3 += 3;
		}

		while (multipleOf5 < 1000) {
			sum += multipleOf5;
			multipleOf5 += 5;
			i++;
			// skip multiples of 3
			if (i % 3 == 0) {
				i++;
				multipleOf5 += 5;
			}
		}

		solution = Integer.toString(sum);
		
		return Integer.toString(sum);
	}

	@Override
	public boolean hasBeenSolved() {
		return true;
	}
}
