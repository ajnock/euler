package problems;

import java.awt.List;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;

import euler.EulerProblem;
import euler.SolvableProblem;
import euler.math.IPrimeSeive;
import euler.math.Primus;
import euler.math.PrimusBase;

/**
 * 
 The prime 41, can be written as the sum of six consecutive primes: 41 = 2 + 3
 * + 5 + 7 + 11 + 13
 * 
 * This is the longest sum of consecutive primes that adds to a prime below
 * one-hundred.
 * 
 * The longest sum of consecutive primes below one-thousand that adds to a
 * prime, contains 21 terms, and is equal to 953.
 * 
 * Which prime, below one-million, can be written as the sum of the most
 * consecutive primes?
 * 
 * @author nock
 * 
 */
public class Problem050 extends EulerProblem implements SolvableProblem {

	private class MyPrimus extends PrimusBase {
		private class PrimeSequence implements Comparable<PrimeSequence>,
				Cloneable {
			long iv;
			long sum;
			int length;
			boolean dead;

			private PrimeSequence(long firstPrime) {
				iv = firstPrime;
				sum = firstPrime;
				length = 1;
			}

			public long add(long prime) {
				sum += prime;
				length++;
				if (length <= 21)
					return 0L;

				return sum;
			}

			@Override
			public int hashCode() {
				return (new Long(iv)).hashCode();
			}

			@Override
			public String toString() {
				return String.format("f(%d,%d) = %d", iv, length, sum);
			}

			@Override
			public int compareTo(PrimeSequence seq) {
				return seq.length - this.length;
			}

			@Override
			public Object clone() {
				PrimeSequence seq = new PrimeSequence(iv);
				seq.sum = sum;
				seq.length = length;
				return seq;
			}

			public boolean isCandidate() {
				return length > 1 && sum % 2 == 1;
			}
		}

		ArrayList<PrimeSequence> in;
		ArrayList<PrimeSequence> out;

		public MyPrimus(long limit) {
			super(limit);
			in = new ArrayList<PrimeSequence>();
			out = new ArrayList<PrimeSequence>();
		}

		@Override
		public boolean seiveInteger(long i) {
			return false;
		}

		@Override
		public boolean seivePrime(long p) {
			@SuppressWarnings("unchecked")
			ArrayList<PrimeSequence> tombstones = (ArrayList<PrimeSequence>) in
					.clone();
			for (int i = 0; i < in.size(); i++) {
				PrimeSequence seq = in.get(0);
				seq.add(p);
				if (seq.sum < limit) {
					tombstones.remove(seq);
					if (seq.isCandidate()) // only odd sums
						out.add((PrimeSequence) seq.clone());
				}
			}

			// for (PrimeSequence t : tombstones) {
			// System.out.println("Killing " + t);
			// }
			// if (in.removeAll(tombstones))
			// System.out.println("Killing " + tombstones.size());
			in.removeAll(tombstones);
			in.add(new PrimeSequence(p));

			// System.out.println(p);

			return false;
		}

		@Override
		public void seive() {
			System.out.println("Seiving primes...");
			super.seive();

			System.out.println("Copying leftovers...");
			for (PrimeSequence p : in) {
				if (p.isCandidate()) {
					out.add(p);
				}
			}

			System.out.println("Sorting...");
			Collections.sort(out);

			for (PrimeSequence seq : out) {
				if (isPrime(seq.sum)) {
					System.out.println(seq);
					answer = seq;
					return;
				}
			}
		}
	}

	@Override
	public Object solve() throws Exception {
		int limit = 1000000;
		PrimusBase primus = new MyPrimus(limit);
		primus.seive();
		return primus.getAnswer();
	}

	@Override
	public boolean hasBeenSolved() {
		// TODO Auto-generated method stub
		return false;
	}

}
