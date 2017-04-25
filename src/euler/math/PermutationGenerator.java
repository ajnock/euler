/**
 * 
 */
package euler.math;

import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.Stack;

/**
 * @author nock
 * 
 */
public class PermutationGenerator {

	private Collection<Thread> threads;
	private String seed;
	private final HashSet<String> permutations;
	private final Object mutex;

	/**
	 * 
	 */
	public PermutationGenerator(String semilla) {
		seed = semilla;
		int size;
		try {
			size = (int) EulerMath.factorial(semilla.length());
		} catch (Exception e) {
			e.printStackTrace();
			size = semilla.length();
			size *= size;
		}
		permutations = new HashSet<String>(size);
		mutex = new Object();
		threads = new Stack<Thread>();
	}

	public int getNumThreads() {
		return threads != null ? threads.size() : 0;
	}

	public void generate() throws InterruptedException {
		switch (seed.length()) {
		case 1:
			permutations.add(seed);
			return;
		case 2:
			char[] c = seed.toCharArray();
			String reversed = String.format("%c%c", c[1], c[0]);
			permutations.add(seed);
			permutations.add(reversed);
			return;
		}
		char[] chars = seed.toCharArray();
		Arrays.sort(chars);
		char last = 0;
		for (int i = 0; i < chars.length; i++) {
			if (chars[i] == last)
				continue;
			last = chars[i];

			char[] available = new char[chars.length - 1];
			Runnable hydra;
			int marker = 0;
			if (i > 0) {
				marker = i - 1;
				System.arraycopy(chars, 0, available, 0, marker);
			}
			if (i < chars.length - 1) {
				System.arraycopy(chars, marker + 1, available, marker,
						chars.length - marker - 1);
			}
			hydra = new Hydra(available, Character.toString(last));

			threads.add(new Thread(hydra));
		}

		for (Thread thread : threads) {
			thread.run();
		}
		for (Thread thread : threads) {
			thread.join();
		}
	}

	private class Hydra implements Runnable {

		private char[] available;
		private String permutation;

		/**
		 * Assumption that char array is sorted
		 * 
		 * @param available
		 *            Sorted char[]
		 * @param permutation
		 */
		public Hydra(char[] available, String permutation) {
			this.available = available;
			this.permutation = permutation;
		}

		@Override
		public void run() {
			this.reproduce();
		}

		private void reproduce() {
			if (available.length == 0) {
				synchronized (mutex) {
//					System.out.println(this.permutation);
					permutations.add(this.permutation);
				}
				return;
			}

			for (int i = 0; i < available.length; i++) {

				char[] nextAvailable = new char[available.length - 1];
				int marker = 0;
				if (i > 0) {
					marker = i - 1;
					System.arraycopy(available, 0, nextAvailable, 0, marker);
				}
				if (i < available.length - 1) {
					System.arraycopy(available, marker + 1, nextAvailable,
							marker, available.length - marker - 1);
				}
				Hydra hydra = new Hydra(nextAvailable, String.format("%s%c",
						new Object[] { this.permutation, available[i] }));
				hydra.reproduce();
			}
		}
	}

	@Override
	public void finalize() throws InterruptedException {
		for (Thread t : threads) {
			t.join();
		}
	}

	public Collection<String> getPermutations() {
		synchronized (mutex) {
			return permutations;
		}
	}
}
