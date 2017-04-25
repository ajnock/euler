package euler;

import problems.Problem104;
import problems.Problem012;
import problems.Problem160;
import problems.Problem162;
import problems.Problem282;
import problems.Problem050;
import problems.Problem058;
import problems.Problem078;
import problems.solved.Problem001;
import problems.solved.Problem002;
import problems.solved.Problem003;
import problems.solved.Problem004;
import problems.solved.Problem007;
import problems.solved.Problem014;
import problems.solved.Problem035;
import problems.solved.Problem041;
import problems.solved.Problem079;

/**
 * 
 */

/**
 * @author nock Solves a Project Euler problem
 */
public class SolutionFactory {

	public static int[] PROBLEMS = { 1, 2, 3, 4, 7, 12, 14, 41, 50, 58, 104,
			162, 79, 160, 282 };

	public static int[] PROBLEMS_TO_SOLVE = { 58 };
	public static int[] PROBLEMS_UNSOLVED = { 12, 50, 58, 104, 162, 160, 282 };
	public static int[] PROBLEMS_SOLVED = { 1, 2, 3, 4, 7, 14, 35, 41, 79 };
	public static boolean SHOW_ANSWERED_ONLY = false;

	/**
	 * Solves the given problems.
	 * 
	 * @param args
	 *            problem numbers
	 * @throws Exception
	 * @throws NumberFormatException
	 */
	public static void main(String args[]) {
		if (args == null || args.length == 0) {

			int[] array;
			if (PROBLEMS_TO_SOLVE.length == 0)
				array = PROBLEMS_UNSOLVED;
			else
				array = PROBLEMS_TO_SOLVE;

			int len = array.length;
			SolvableProblem[] problems = new SolvableProblem[len];
			Thread[] threads = new Thread[len];
			for (int i = 0; i < len; i++) {
				try {
					SolvableProblem p = SolutionFactory
							.GenerateProblem(array[i]);
					problems[i] = p;
					threads[i] = new Thread(p);
				} catch (NullPointerException ex) {
					System.out.println(String.format(
							"Add problem %i to the switch list!",
							new Object[] { args }));
					threads[i] = new Thread();
				}
			}
			for (int i = 0; i < len; i++) {
				try {
					threads[i].run();
				} catch (NullPointerException ex) {
					System.out.println(String.format(
							"Add problem %i to the switch list!",
							new Object[] { args }));
				}
			}
			for (int i = 0; i < len; i++) {
				try {
					threads[i].join();
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			for (int i = 0; i < len; i++) {
				System.out.println(problems[i]);
			}
		} else {
			for (int i = 0; i < args.length; i++) {
				SolvableProblem p = SolutionFactory.GenerateProblem(Integer
						.parseInt((args[i])));
				display(p);
			}
		}
	}

	public static void display(SolvableProblem enigma) {
		String className = enigma.getClass().toString();
		if (!(enigma.hasBeenSolved() && SHOW_ANSWERED_ONLY)
				&& !(!enigma.hasBeenSolved() && !SHOW_ANSWERED_ONLY)) {
			System.out.println("Skipping " + className);
		} else {
			System.out.println("Solving " + className);
			String solution = null;
			try {
				Object obj = enigma.solve();
				if (obj != null)
					solution = obj.toString();
			} catch (Exception ex) {
				ex.printStackTrace();
			}
			if (solution != null && solution.length() != 0)
				System.out.println("Solution is: " + solution);
			else
				System.out.println("No Solution");
		}
		System.out.println();
	}

	public static SolvableProblem GenerateProblem(int number) {
		switch (number) {
		case 1:
			return new Problem001();
		case 2:
			return new Problem002();
		case 3:
			return new Problem003();
		case 4:
			return new Problem004();
		case 7:
			return new Problem007();
		case 12:
			return new Problem012();
		case 14:
			return new Problem014();
		case 35:
			return new Problem035();
		case 41:
			return new Problem041();
		case 50:
			return new Problem050();
		case 58:
			return new Problem058();
		case 162:
			return new Problem162();
		case 104:
			return new Problem104();
		case 78:
			return new Problem078();
		case 79:
			return new Problem079();
		case 160:
			return new Problem160();
		case 282:
			return new Problem282();
		default:
			return null;
		}
	}
}
