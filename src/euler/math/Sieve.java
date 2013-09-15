package euler.math;

import java.util.ArrayList;
import java.util.Stack;

public abstract class Sieve  {

	protected class Hydra extends Thread {
		public int hashCode() {
			return n;
		}
	
		private volatile boolean isReady;
	
		private synchronized void growHead() {
			synchronized (this) {
				heads++;
				System.out.println(String.format("Hydra %d: Growing head %d",
						n, heads));
				this.notify();
			}
		}
	
		public boolean equals(Object obj) {
			return obj != null && obj.getClass() == Hydra.class
					&& ((Hydra) obj).n == n;
		}
	
		@Override
		public void run() {
			multiple = 2 * n;
			while (multiple <= seiveLimit) {
				isReady = false;
				while (multiple < seiveBuffer * heads) {
					synchronized (seedSeive) {
						System.out.println(String.format(
								"Hydra %d: %d is composite", n, multiple));
						seedSeive.set(multiple % seiveBuffer, n);
					}
					multiple += n;
				}
	
				if (multiple > seiveLimit)
					die();
	
				isReady = true;
				int nextHead = heads + 1;
	
				System.out.println(String.format(
						"Hydra %d: Waiting to grow head %d at %d", n, nextHead,
						multiple));
	
				isWaiting = true;
				while (heads < nextHead)
					synchronized (this) {
						try {
							this.wait();
						} catch (InterruptedException e) {
							System.out
									.println(String
											.format("Hydra %d: Heraclese caught %d heads on fire at multiple %d",
													n, heads, multiple));
							e.printStackTrace();
						}
					}
				isWaiting = false;
				System.out.println(String.format("Hydra %d: I have %d heads",
						n, heads));
	
			}
		}
	
		// private void setSeive(boolean value) {
		// if (multiple - offset >= seedSeive.size()) {
		// System.out.println("Hydra " + Integer.toString(n) +
		// ": Growing to "
		// + Integer.toString(multiple));
		// int j;
		// for (j = seedSeive.size(); j < multiple - offset; j++) {
		// System.out.println("Hydra " + Integer.toString(n) + ": "
		// + Integer.toString(seedSeive.size())
		// + " might be prime");
		// seedSeive.add(PRIME);
		// }
	
		// System.out.println("Hydra " + Integer.toString(n) + ": "
		// + Integer.toString(j) + " is composite");
		// seedSeive.add(COMPOSITE);
		//
		// } else {
		// System.out.println(String.format("Hydra %d: %d is composite",
		// n, multiple));
		// seedSeive.set(multiple % seiveBuffer, value);
		// }
		// }
	
		volatile boolean isWaiting = false;
	
		private void die() {
			synchronized (lernaeanHydra) {
				System.out.println(String.format(
						"Hydra %d: Dying at %d / %d with %d heads.", n,
						multiple, seiveLimit, heads));
				lernaeanHydra.remove(this);
			}
		}
	
		public Hydra(int i) {
			synchronized (this) {
				n = i;
				multiple = n * 2;
				heads = 1;
				System.out.println(String.format("I am Hydra %d", n));
				// System.out.println(String.format("Hydra %d: Growing head %d",
				// n, heads));
			}
		}
	
		private volatile int multiple;
		private final int n;
		private volatile int heads;
	
		public int getMultiple() {
			return multiple;
		}
	
		public int getN() {
			return n;
		}
	}

	/**
	 * Array of all Primes found;
	 */
	protected Stack<Integer> Primes;
	public static final int COMPOSITE = 1;
	public static final int PRIME = 0;
	protected final ArrayList<Hydra> lernaeanHydra;
	private int maxInSeive;
	/**
	 * Array of all number seived. True is prime;
	 */
	protected volatile ArrayList<Integer> seedSeive;
	protected final int seiveBuffer;
	protected final int seiveLimit;

	protected void waitForHydraToFinish() {
		System.out.println("Waiting on hydra");

		while (!hydraAreReady()) {
			try {
				Thread.sleep(100);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		System.out.println("Done waiting on hydra");
	}

	protected void tryGrowFor(int p) {
		if (p > maxInSeive) {
			// reset the seive
			synchronized (seedSeive) {
				System.out.println("Resetting seive...");
				maxInSeive += seiveBuffer;
				for (int j = 0; j < seiveBuffer; j++) {
					int q = j + p;
					boolean composite = q % 2 == 0;
					System.out.println(String.format(
							"Resetting seive\t%d is %s", q, composite ? "even"
									: "odd"));
					seedSeive.set(j, composite ? 2 : PRIME);
				}
			}

			restartHydra();
			waitForHydraToFinish();
		}
	}

	protected int getSeiveBlocking(int p) throws Exception {
		if (isReadyToBeTested(p)) {
			synchronized (seedSeive) {
				// System.out.println(String.format("%d => %s", p,
				// seedSeive.get(p % seiveBuffer) == PRIME ? "prime" :
				// "composite"));
				return seedSeive.get(p % seiveBuffer);
			}
		} else
			throw new Exception(String.format("p = %d is too large!", p));
	}

	/**
	 * Tests that p can be tested for primality, that is the Hydra have passed p
	 * in their seive.
	 * 
	 * @param p
	 * @return
	 * @throws Exception
	 */
	private boolean canBeTested(int p) throws Exception {
		if (p > seiveLimit)
			throw new Exception();
		synchronized (seedSeive) {
			if (seiveBuffer > seedSeive.size()) {
				System.out.println(String.format("%d is not ready", p));
				return false;
			}
	
			synchronized (lernaeanHydra) {
				for (Hydra hydra : lernaeanHydra) {
					if (hydra.getN() < p / 2 && !hydra.isWaiting) {
						if (hydra.getMultiple() <= p) {
							System.out
									.println(String
											.format("Hydra %d: %d is not ready, currently at %d",
													hydra.getN(), p,
													hydra.getMultiple()));
							return false;
						}
					} else {
						return true;
					}
				}
			}
		}
		return true;
	}

	protected int getSeive(int p) {
		synchronized (seedSeive) {
			return seedSeive.get(p % seiveBuffer);
		}
	}

	public int getSeiveBuffer() {
		return seiveBuffer;
	}

	public int getSeiveLimit() {
		return seiveLimit;
	}

	private boolean hydraAreReady() {
		synchronized (lernaeanHydra) {
			for (Hydra h : lernaeanHydra) {
				if (!h.isReady)
					return false;
			}
		}
		return true;
	}

	/**
	 * Blocks until p is ready to be tested
	 * 
	 * @param p
	 * @return
	 * @throws Exception
	 */
	private boolean isReadyToBeTested(int p) throws Exception {
		while (!canBeTested(p)) {
			System.out.println(String.format("Sleeping on %d", p));
			Thread.sleep(100);
		}
		return true;
	}

	private void restartHydra() {
		synchronized (lernaeanHydra) {
			for (Hydra hydra : lernaeanHydra) {
				synchronized (hydra) {
					hydra.growHead();
				}
			}
		}
	}

	public void seive() throws Exception {
		seiveTo(seiveLimit);
	}

	protected abstract void seiveTo(int n) throws Exception;

	protected boolean isComposite(int p) {
		int divisor;
		synchronized (seedSeive) {
			divisor = seedSeive.get(p % seiveBuffer);
		}
		if (divisor != PRIME) {
			System.out.println(String.format("%d divides %d", divisor,p));
			return false;
		} else {
			return true;
		}
	}

	public Sieve(int limit) {
		seiveLimit = limit;
		seiveBuffer = 10;
		maxInSeive = seiveBuffer - 1;
		lernaeanHydra = new ArrayList<Hydra>();
	}

}