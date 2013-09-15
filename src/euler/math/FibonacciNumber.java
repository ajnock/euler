package euler.math;

public class FibonacciNumber {
	private int n;
	private long F;
	private long prev;
	private long prevprev;

	public int toInt() {
		return (int) F;
	}

	public long toLong() {
		return F;
	}

	public String toString() {
		return String.format("F[%d] = %d", n, F);
	}

	public int getN() {
		return n;
	}

	public void zero() {
		n = 2;
		prev = 1;
		prevprev = 0;
		F = 1;
	}

	public FibonacciNumber() {
		zero();
	}

	public FibonacciNumber(int i) {
		zero();
		while (n < i) {
			next();
		}
	}

	public long next() {
		n++;
		prevprev = prev;
		prev = F;
		F = prevprev + prev;
		return F;
	}

	public long previous() {
		n--;
		long tmp = prevprev;
		prevprev = F - prev;
		F = prev;
		prev = tmp;
		return F;
	}
}