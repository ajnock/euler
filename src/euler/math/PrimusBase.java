/**
 * 
 */
package euler.math;

import java.util.List;
import java.util.Stack;

/**
 * @author nock
 * 
 */
public abstract class PrimusBase implements IPrimeSeive {
	// private Map<Long, Long> seive;
	protected List<Long> seive;
	protected long limit;

	public PrimusBase(long limit) {
		this.seive = new Stack<Long>();// new HashMap<Long, Long>();
		this.limit = limit;
		this.answer = "No Solution";
	}

	public boolean seive(long i) {
		return false;
	}

	protected Object answer;

	public List<Long> getPrimes() {
		return seive;
		// Set<Long> primes;
		// synchronized (seive) {
		// primes = seive.keySet();
		// }
		// // no one needs to know we exclude the degenerate case
		// // primes.add(2L);
		// return primes;
	}

	public void seive() {
		recordPrime(2L);
		long i = 3L;
		// long len = 1L;
		while (i <= limit) {
			// long multiple;
			boolean isPrime = true;
			for (long p : getPrimes()) {
				if (p == 2L)
					continue;
				if (p > i / 3L + 1L)
					break;
				// multiple = check(p, i);
				// if (multiple == i) {
				if (i % p == 0L) {
					// System.out.println(String.format("%d is a multiple of %d",
					// multiple, p));
					isPrime = false;
					break;
				}
			}

			if (isPrime) {
				// len += 2L;

				// double percentPrime = (double)(len / i);
				// int precision = 100;
				// if (Math.ceil(percentPrime * precision) > precision)
				// System.out.println(String.format("%d\t%d/%.3d = %d%%", len,
				// i, limit, percentPrime));

				// System.out.println(String.format("%d => %d/%d | %d", i, len,
				// limit, limit - i));

				// double percentPrime = Math.round(100L * (len / i));
				// System.out.println(String.format("%d \t %d \t %d%% \t %d", i,
				// len, percentPrime, limit));

				if (recordPrime(i))
					return;
				// seive.put(i, i);
			}

			if (seiveInteger(i))
				return;

			i += 2L;
		}
	}

	/**
	 * @param p
	 */
	protected boolean recordPrime(long p) {
		seive.add(p);
		return seivePrime(p);
	}

	// private long check(long p, long i) {
	// long multiple = seive.get(p);
	// while (multiple < i)
	// multiple += p;
	// seive.put(p, multiple);
	// return multiple;
	// }

	public boolean isPrime(long p) {
		// return seive.containsKey(p);
		return seive.contains(p);
	}

	public int getPrimesCount() {
		return seive.size();
	}

	public boolean primesEmpty() {
		return seive.isEmpty();
	}

	public long getSeiveLimit() {
		return limit;
	}

	/**
	 * @return the answer
	 */
	public Object getAnswer() {
		return answer;
	}
}
