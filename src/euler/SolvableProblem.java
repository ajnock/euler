package euler;

public interface SolvableProblem extends Runnable{
	/**
	 * Solves the problem and returns the solution
	 * 
	 * @return Solution string
	 */
	public Object solve() throws Exception;

	/**
	 * True iff the problems has been solved and tested.
	 * 
	 * @return boolean
	 */
	public boolean hasBeenSolved();
}
