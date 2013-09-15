/**
 * 
 */
package euler.math;

import java.math.BigInteger;
import java.util.Arrays;
import java.util.Stack;

/**
 * @author nock
 * 
 */
public final class EulerMath {

	public static long factorial(long n) throws Exception {
		if (n < 0L)
			throw new Exception();
		else if (n <= 1L)
			return 1L;
		else
			return n * factorial(n - 1L);
	}

	public static int ackermanFunction(int m, int n)
			throws IllegalArgumentException {
		return A(m, n);
	}

	public static int A(int m, int n) throws IllegalArgumentException {
		if (m == 0) {
			// System.out.println(String.format("A(%d, %d) = %d", m, n, n + 1));
			return n + 1;
		} else {
			// System.out.println(String.format("A(%d, %d)", m, n));
			try {
				if (m > 0 && n == 0) {
					m = m - 1;
					n = 1;
				} else if (m > 0 && n > 0) {
					n = A(m, n - 1);
					m = m - 1;
				} else
					throw new IllegalArgumentException();
			} catch (StackOverflowError so) {
				throw new StackOverflowError(String.format("A(%d,%d)", m, n));
			}
			return A(m, n);
		}
	}

	public static int numDivisors(BigInteger n) {
		int sum = 0;
		BigInteger i = BigInteger.valueOf(2);
		while (i.compareTo(n.divide(BigInteger.valueOf(2))) < 0) {
			if (n.mod(i) == BigInteger.ZERO)
				sum++;
		}
		return sum;
	}

	public static long numDivisors(long n) {
		long i = 0;
		for (long j = 2; j < n / 2L; j++) {
			if (n % j == 0)
				i++;
		}
		return i;
	}

	public static boolean isPandigital(String digits) {
		if (digits.length() > 9)
			return false;
		char[] expected = "123456789".substring(0, digits.length())
				.toCharArray();
		try {
			char[] actual = digits.toCharArray();
			Arrays.sort(actual);
			for (int i = 0; i < expected.length; i++) {
				if (expected[i] != actual[i])
					return false;
			}
		} catch (Exception ex) {
			return false;
		}
		return true;
	}

	public static boolean isFibonacci(int f) {
		int a = 5 * f * f + 4;
		int b = 5 * f + f - 4;
		return a % 4 == 0 && b % 4 == 0;
	}

	public static Stack<String> generatePandigitalSequences(int len)
			throws Exception {
		if (len > 9 || len < 1) {
			throw new Exception();
		}

		Stack<String> sequences = generatePermutations("123456789".substring(0,
				len));

		return sequences;
	}

	public static Stack<String> generatePermutationRecursiveStep(
			String available, String seed) {
		Stack<String> permutations = new Stack<String>();
		if (available.length() == 1) {
			String out = seed + available;
			// System.out.println(out);
			permutations.add(out);
			return permutations;
		}
		for (int i = 0; i < available.length(); i++) {
			String tmp = "";
			for (int j = 0; j < available.length(); j++) {
				if (j == i)
					continue;
				tmp = tmp + available.charAt(j);
			}
			permutations.addAll(generatePermutationRecursiveStep(tmp, seed
					+ available.charAt(i)));
		}
		return permutations;
	}

	public static long getFibonacci(long n) {
		if (n == 0)
			return 0;
		if (n == 1 || n == 2)
			return 1;
		long f1 = 1;
		long f2 = 2;
		long i = 3;
		while (i < n) {
			long tmp = f2 + f1;
			f1 = f2;
			f2 = tmp;
			i++;
		}
		return f2;
	}

	public static long binomialCoefficient(long n, long k) throws Exception {
		if (k == 0L && n >= 0L)
			return 1L;
		else if (n == 0L && k > 0L)
			return 0L;
		else if (0L <= k && k <= n)
			return factorial(n) / (factorial(k) * factorial(n - k));
		else
			throw new Exception();
	}

	public static BigInteger bigBinomialCoefficient(long n, long k)
			throws Exception {
		return new BigInteger(Long.toString(binomialCoefficient(n, k)));
	}

	/**
	 * Returns the next fibonacci number
	 * 
	 * @param kSubN
	 * @param kSubNPlusOne
	 * @return
	 */
	public static long nextFibonacci(long kSubN, long kSubNPlusOne) {
		return kSubN + kSubNPlusOne;
	}

	public static boolean isAllSameCharacter(int permutation) {
		char[] characters = Integer.toString(permutation).toCharArray();
		if (characters.length < 2)
			return true;
		for (int i = 1; i < characters.length; i++) {
			if (characters[i - 1] != characters[i])
				return false;
		}

		return true;
	}

	public static boolean isPalindrome(String str) {
		char[] array = str.toCharArray();
		for (int i = 0; i < array.length / 2; i++) {
			if (array[i] != array[array.length - i - 1]) {
				return false;
			}
		}

		return true;
	}

	public static Stack<String> generatePermutations(String string) {
		return generatePermutationRecursiveStep(string, "");
	}
}
