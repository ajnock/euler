package problems.solved;

import java.util.Stack;

import euler.EulerProblem;
import euler.math.EulerMath;

/**
 * 
 We shall say that an n-digit number is pandigital if it makes use of all the
 * digits 1 to n exactly once. For example, 2143 is a 4-digit pandigital and is
 * also prime.
 * 
 * What is the largest n-digit pandigital prime that exists?
 * 
 * @author nock
 * 
 */
public class Problem041 extends EulerProblem {

	public Problem041() {
		// TODO Auto-generated constructor stub
	}

	@Override
	public Object solve() throws Exception {
		for (int i = 9; i > 4; i--) {
			Stack<String> permutations = EulerMath
					.generatePandigitalSequences(i);
			while (!permutations.isEmpty()) {
				String p = permutations.pop();
				if (IsPrime(p))
					return solution = p;
			}
		}
		return null;
	}

	private boolean IsPrime(String p) {
		long l = Long.parseLong(p);
		for (long i = 2; i < Math.floor(l / 2); i++) {
			if (l % i == 0)
				return false;
		}
		return true;
	}

	@Override
	public boolean hasBeenSolved() {
		// TODO Auto-generated method stub
		return true;
	}

}
