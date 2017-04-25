/**
 * 
 */
package problems.solved;

import java.lang.reflect.Array;
import java.util.Arrays;
import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.Stack;
import java.util.Vector;

import euler.EulerProblem;
import euler.math.EulerMath;
import euler.math.PermutationGenerator;
import euler.math.PrimesFactory;

/**
 * @author nock
 * 
 * 
 * 
 *         The number, 197, is called a circular prime because all rotations of
 *         the digits: 197, 971, and 719, are themselves prime.
 * 
 *         There are thirteen such primes below 100: 2, 3, 5, 7, 11, 13, 17, 31,
 *         37, 71, 73, 79, and 97.
 * 
 *         How many circular primes are there below one million?
 */
public class Problem035 extends EulerProblem {

	private static int limit = (int) 1e6;

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#solve()
	 */
	@Override
	public Object solve() throws Exception {
		PrimesFactory factory = new PrimesFactory(limit);
		System.out.println("Seiving...");
		factory.seive();
		System.out.println("seive done");
		primes = factory.getPrimes();

		long circularPrimes = 0;
		// LinkedList<Integer> reversedPrimes = new LinkedList<Integer>();
		// for (Integer prime : primes) {
		// reversedPrimes.add(prime);
		// }

		for (Integer prime : factory.getPrimes()) {
			update(prime);
			if (!wouldWork())
				continue;

			circularPrimes++;
			System.out.println(String.format("#%d !!!", circularPrimes));
			// for (int i = 0; i < chars.length; i++) {
			// switch (chars[i]) {
			// case '0':
			// case '2':
			// case '4':
			// case '5':
			// case '6':
			// case '8':
			// continue;
			// }
			// }
			//
			// Stack<String> family = EulerMath.generatePermutations(Integer
			// .toString(p));
			// System.out.println(family.size());
			// Stack<Integer> familyInts = new Stack<Integer>();
			// boolean isPrimeSet = true;
			// while (!family.empty() && isPrimeSet) {
			// String permutation = family.pop();
			// System.out.print(" " + permutation + " ");
			// int perm = Integer.parseInt(permutation);
			// familyInts.push(perm);
			// isPrimeSet &= factory.isPrime(perm);
			// // System.out.print(' ');
			// // System.out.print(permutation);
			// // if (!isPrimeSet)
			// // System.out.print('!');
			// // System.out.print(' ');
			// }
			//
			// System.out.println();
			//
			// boolean unique = true;
			// if (isPrimeSet) {
			// System.out.println(p);
			// // System.out.print(p + " = > {");
			// for (int perm : familyInts) {
			// // System.out.print(" ");
			// // System.out.print(perm);
			// if (!permutations.add(perm)
			// && !EulerMath.isAllSameCharacter(perm)) {
			// // System.out.print('!');
			// unique = false;
			// }
			// // System.out.print(" ");
			// }
			// // System.out.print("}");
			// // System.out.println();
			//
			// circularPrimes++;
			// // System.out.println(Long.toString(circularPrimes++));
			// }
		}

		return solution = Long.toString(circularPrimes);
	}

	private void update(int prime) {
		p = prime;
		str = Integer.toString(p);
	}

	private boolean wouldWork() throws InterruptedException {
		// degenerate cases
		switch (p) {
		case 2:
		case 3:
		case 5:
			System.out.print(String.format("Generating %d and it is circular",
					p));
			return true;
		}

		if (!digitsDivisible())
			return false;

		if (!permutationsArePrime())
			return false;

		return true;
	}

	private int p;
	private String str;
	private Collection<Integer> primes;

	private boolean digitsDivisible() {
		// int sum = 0;
		for (char s : str.toCharArray()) {
			switch (s) {
			case '0':
			case '2':
			case '4':
			case '5':
			case '6':
			case '8':
				// System.out.println(String.format(
				// "%d is not circular because it contains %c!", p, s));
				return false;
			}
		}
		return true;
	}

	private boolean permutationsArePrime() throws InterruptedException {
		// PermutationGenerator generator = new PermutationGenerator(str);
		// System.out.print(String.format("Generating %s and it is ", str));
		// generator.generate();
		// Stack<String> set = EulerMath.generatePermutations(str);
		// System.out.print("{ ");
		// for (String permutation : set) {
		// System.out.print(String.format("%s ", permutation));

		// int q = Integer.parseInt(permutation);
		// if (!primes.contains(q)) {
		// System.out.print(String.format("} not circular by %d\n", q));
		// return false;
		// }
		// }

		// System.out.print("} circular ");

		char[] chars = str.toCharArray();
		for (int i = 0; i < chars.length; i++) {
			StringBuilder permutation = new StringBuilder(chars.length);
			for (int j = 0; j < chars.length; j++) {
				permutation.append(chars[(i + j) % chars.length]);
			}
			int q = Integer.parseInt(permutation.toString());
			if (!primes.contains(q))
				return false;
		}

		System.out.println(String.format("%d is circular!", p));

		return true;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see euler.SolvableProblem#hasBeenSolved()
	 */
	@Override
	public boolean hasBeenSolved() {
		// 55
		return true;
	}

}
