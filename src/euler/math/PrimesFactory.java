/**
 * 
 */
package euler.math;

import java.util.ArrayList;
import java.util.Stack;

/**
 * @author nock
 * 
 */
public class PrimesFactory extends Sieve {
	public PrimesFactory(int limit) {
		super(limit);
	}

	/**
	 * Gets the nth prime. This may trigger a seive.
	 * 
	 * @param n
	 * @return prime
	 * @throws Exception
	 */
	public int getPrime(int n) throws Exception {
		if (n >= seiveLimit)
			return -1;

		if (n > Primes.size())
			seiveTo(seiveLimit);
		return Primes.get(n);
	}

	// public void increaseSeiveLimit(int delta) {
	// seiveLimit *= (int) Math.pow(10, delta);
	// }

	// private static int loadBalancer = 100;

	@Override
	protected void seiveTo(int n) throws Exception {
		// we start with only the degenerate case 2
		Primes = new Stack<Integer>();
		Primes.add(2);
		seedSeive = new ArrayList<Integer>(seiveBuffer);

		seedSeive.add(COMPOSITE); // 0
		seedSeive.add(COMPOSITE); // 1
		seedSeive.add(PRIME); // 2
		seedSeive.add(PRIME); // 3
		for (int i = 4; i < seiveBuffer; i++) {
			if (i % 2 == 0) {
				seedSeive.add(COMPOSITE);
				System.out.println(String.format("%d is composite", i));
			} else {
				seedSeive.add(PRIME);
				System.out.println(String.format("%d might be prime", i));
			}
		}

		int p = 3;
		while (p < seiveLimit) {
			while (p < seiveLimit && !isComposite(p)) {
				p += 2;
				tryGrowFor(p);
			}

			Primes.add(p);
			// i++;
			// System.out.println(Integer.toString(p) + " is prime #"
			// + Integer.toString(i));

			if (p <= seiveLimit / 2) {
				synchronized (lernaeanHydra) {
					Hydra hydra = new Hydra(p);
					hydra.start();
					lernaeanHydra.add(hydra);
				}
			}

			p += 2;
			tryGrowFor(p);
		}

		// synchronized (lernaeanHydra) {
		// for (Hydra hydra : lernaeanHydra) {
		// if (hydra.isAlive())
		// hydra.interrupt();
		// }
		// }

		System.out.println(Integer.toString(Primes.size()) + " primes found!");

		// }
	}

	public Integer[] getPrimesArray() throws Exception {
		Integer[] primes = new Integer[Primes.size()];
		Primes.toArray(primes);
		return primes;
	}

	public Stack<Integer> getPrimes() {
		return Primes;
	}

	public boolean isPrime(int q) throws Exception {
		if (q < seiveBuffer)
			throw new Exception("Argument out of range");
		return Primes.contains(q);
	}

	public boolean isPrimeSet(Stack<Integer> family) throws Exception {
		for (int p : family) {
			if (isPrime(p))
				return false;
		}
		return true;
	}

	public int getNumberOfPrimes() {
		return Primes.size();
	}

	public boolean primesEmpty() {
		return Primes == null || Primes.empty();
	}

	public Integer pop() {
		return Primes.pop();
	}
}
