package euler.sequence;

public abstract class SequenceGenerator implements ISequence, Runnable {
	protected final long seed;
	private long currentValue;

	public long getCurrentValue() {
		return currentValue;
	}
	public SequenceGenerator(long initialValue) {
		seed = initialValue;
	}

	public SequenceGenerator() {
		seed = 1;
	}
	public void printSequence() {
		Thread thread = new Thread(this);
		thread.run();
		try {
			thread.join();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
	}

	@Override
	public void run() {
		currentValue = seed;
		while (!seive(currentValue)) {
			currentValue = recurse(currentValue);
		}
	}
}
