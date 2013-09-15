/**
 * 
 */
package problems;

import java.util.ArrayList;

import euler.EulerProblem;
import euler.math.EulerMath;
import euler.math.PrimesFactory;

/**
 * The Fibonacci sequence is defined by the recurrence relation:
 * 
 * Fn = Fn-1 + Fn-2, where F1 = 1 and F2 = 1.
 * 
 * It turns out that F541, which contains 113 digits, is the first Fibonacci
 * number for which the last nine digits are 1-9 pandigital (contain all the
 * digits 1 to 9, but not necessarily in order). And F2749, which contains 575
 * digits, is the first Fibonacci number for which the first nine digits are 1-9
 * pandigital.
 * 
 * Given that Fk is the first Fibonacci number for which the first nine digits
 * AND the last nine digits are 1-9 pandigital, find k.
 * 
 * @author nock
 * 
 */
public class Problem104 extends EulerProblem {

	/*
	 * (non-Javadoc)
	 * 
	 * @see problems.EulerProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		long n = 1;
		long Fn = 1;

		System.out.println("9! = " + Long.toString(EulerMath.factorial(9)));
		PrimesFactory factory = new PrimesFactory(1000000);
		// generate 362880 pandigital combinations
		for (int i = 1; i < 262880; i++) {
			char[] characters = (new String("123456789")).toCharArray();
			int len = characters.length;
			int delta = factory.getPrime(i);
			ArrayList<Character> used = new ArrayList<Character>();
			StringBuilder sequence = new StringBuilder();
			for (int a = 0; sequence.length() < len; a++) {
				char tmp = characters[a];
				if (used.contains(tmp))
					a = (a + delta) % len;
				if (a < len)
					tmp = characters[a];
				sequence.append(tmp);
				used.add(tmp);
			}
			System.out.println(sequence.toString());
		}

		return "F[" + Long.toString(n) + "] = " + Long.toString(Fn);
	}

	public boolean isPalindrome(int f, long n) {
		if (n < 2749)
			return false;
		String number = Integer.toString(f);
		int len = number.length();
		if (len < 10)
			return false;
		String ms10 = number.substring(0, 8);
		String ls10 = number.substring(len - 9, len - 1);
		for (int i = 1; i < 10; i++) {
			String str = Integer.toString(i);
			if (!ms10.contains(str) || !ls10.contains(str))
				return false;
		}
		return true;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see problems.EulerProblem#hasBeenSolved()
	 */
	@Override
	public boolean hasBeenSolved() {
		return false;
	}

}
