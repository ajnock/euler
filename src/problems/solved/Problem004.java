package problems.solved;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.EulerMath;

/**
 * 
 A palindromic number reads the same both ways. The largest palindrome made
 * from the product of two 2-digit numbers is 9009 = 91 × 99.
 * 
 * Find the largest palindrome made from the product of two 3-digit numbers.
 * 
 * @author nock
 * 
 */
public class Problem004 extends EulerProblem implements SolvableProblem {

	public Problem004() {
		// TODO Auto-generated constructor stub
	}

	@Override
	public Object solve() throws Exception {
		int palindrome = 0;
		for (int i = 1; i <= 999; i++) {
			for (int j = 1; j <= 999; j++) {
				int tmp = i * j;
				if (EulerMath.isPalindrome(Integer.toString(tmp))
						&& palindrome < tmp) {
					System.out.println(i + " x " + j + " = " + tmp);
					palindrome = tmp;
				}
			}
		}
		return solution = Integer.toString(palindrome);
	}

	@Override
	public boolean hasBeenSolved() {
		// TODO Auto-generated method stub
		return true;
	}

}
