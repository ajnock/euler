/**
 * 
 */
package euler;

/**
 * @author nock
 * 
 */
public abstract class EulerProblem implements SolvableProblem {

	protected Object solution = "No Solution";

	public String getSolution() {
		return solution.toString();
	}

	@Override
	public void run() {
		System.out.println("Solving " + this.getClass().toString());
		try {
			solution = solve();
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public String toString() {
		String accent = hasBeenSolved() ? "!" : "?";
		return this.getClass().toString() + " => " + solution + accent;
	}
}
