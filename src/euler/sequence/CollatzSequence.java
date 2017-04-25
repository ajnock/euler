package euler.sequence;

public class CollatzSequence extends SequenceGenerator {
	public CollatzSequence(long initialValue) {
		super(initialValue);
	}

	public CollatzSequence() {
		super();
	}

	@Override
	public long recurse(long i) {
		// System.out.println(i);
		if (i % 2 == 0)
			i = i / 2;
		else
			i = 3 * i + 1;
		return i;
	}

	@Override
	public boolean seive(long i) {
		// System.out.print(i);
		// System.out.print(' ');
		return i == 1;
	}

	public static void main(String args[]) {
		CollatzSequence seq = new CollatzSequence(1000001);
		seq.printSequence();
	}
}