package euler.math;

public interface IPrimeSeive extends ISeive {
	/**
	 * Override this and return true to stop the seive
	 * 
	 * @param i
	 * @return
	 */
	boolean seiveInteger(long i);

	/**
	 * Override this and return true to stop the seive
	 * 
	 * @param i
	 * @return
	 */
	boolean seivePrime(long p);
}
