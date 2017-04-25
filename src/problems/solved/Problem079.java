package problems.solved;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.util.Arrays;

import euler.EulerProblem;

/**
 * 
 */

/**
 * A common security method used for online banking is to ask the user for three
 * random characters from a passcode. For example, if the passcode was 531278,
 * they may ask for the 2nd, 3rd, and 5th characters; the expected reply would
 * be: 317.
 * 
 * The text file, keylog.txt, contains fifty successful login attempts.
 * 
 * Given that the three characters are always asked for in order, analyse the
 * file so as to determine the shortest possible secret passcode of unknown
 * length.
 * 
 * @author nock
 * 
 */
public class Problem079 extends EulerProblem {

	int[] attempts[];

	public Problem079() {
		attempts = new int[50][3];
		try {
			FileInputStream fstream = new FileInputStream("etc/keylog.txt");
			DataInputStream in = new DataInputStream(fstream);
			BufferedReader br = new BufferedReader(new InputStreamReader(in));
			String strLine;
			int line = 0;
			while ((strLine = br.readLine()) != null) {
				for (int i = 0; i < 3; i++) {
					attempts[line][i] = Integer.parseInt(strLine.substring(i,
							i + 1));
				}
				line++;
			}
			in.close();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see problems.EulerProblem#solve()
	 */
	@Override
	public Object solve() {
		boolean[] numbersAfter[] = new boolean[10][11];
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 11; j++) {
				numbersAfter[i][j] = false;
			}
		}
		for (int i = 0; i < 50; i++) {
			for (int j = 0; j < 3; j++) {
				numbersAfter[attempts[i][j]][10] = true;
				for (int k = j + 1; k < 3; k++) {
					numbersAfter[attempts[i][j]][attempts[i][k]] = true;
				}
			}
		}

		// String retval = "\n";
		// for (int i = 0; i < 10; i++) {
		// retval = retval + i;
		// if (numbersAfter[i][10])
		// retval = retval + " exists and";
		// else
		// retval = retval + " does not exist and";
		// retval = retval + " is followed by:";
		// for (int j = 0; j < 10; j++) {
		// if (numbersAfter[i][j])
		// retval = retval + " " + j;
		// }
		// retval = retval + "\n";
		// }
		// return retval;

		Digit digits[] = new Digit[10];

		for (int i = 0; i < 10; i++) {
			digits[i] = new Digit(i);
			if (!numbersAfter[i][10]) {
				digits[i].count = -1;
			} else {
				for (int j = 0; j < 10; j++) {
					if (numbersAfter[i][j])
						digits[i].count++;
				}
			}
		}

		Arrays.sort(digits);

		String retval = "";
		for (int i = 0; i < 10; i++) {
			retval = retval + digits[i].toString();
		}

		return solution = retval;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see problems.EulerProblem#hasBeenSolved()
	 */
	@Override
	public boolean hasBeenSolved() {
		return true;
	}

	public class Digit implements Comparable<Digit> {
		int value;
		int count;

		public Digit(int i) {
			value = i;
			count = 0;
		}

		public String toString() {
			if (count >= 0)
				return Integer.toString(value);
			else
				return "";
		}

		@Override
		public int compareTo(Digit digit) {
			return digit.count - this.count;
		}
	}
}
