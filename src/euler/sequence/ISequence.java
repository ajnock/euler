package euler.sequence;

import euler.math.ISeive;

public interface ISequence extends ISeive {
	/**
	 * This function will be called recursively upon it's input until {@link ISequence#seive(long)} return 
	 * @param i
	 * @return
	 */
	long recurse(long i);
}
