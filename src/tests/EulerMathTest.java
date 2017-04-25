package tests;

import static org.junit.Assert.*;

import java.util.HashSet;
import java.util.List;

import junit.framework.Assert;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import euler.math.EulerMath;

public class EulerMathTest {
	@Test
	public void testFactorial() throws Exception {
		long[] factorials = new long[] { 1, 1, 2, 6, 24, 120 };
		for (int i = 0; i < factorials.length; i++) {
			Assert.assertEquals(factorials[i], EulerMath.factorial(i));
		}
	}

	@Test
	public void testAckermanFunction() throws Exception {
		// For example A(1, 0) = 2, A(2, 2) = 7 and A(3, 4) = 125.
		int[][] input = new int[][] { { 1, 0, 2 }, { 2, 2, 7 }, { 3, 4, 125 } };
		final int m = 0;
		final int n = 1;
		final int A = 2;
		for (int i = 0; i < input.length;i++) {
		Assert.assertEquals(input[i][A], EulerMath.ackermanFunction(input[i][m], input[i][n]));
	}}

	@Test
	public void testIsPandigital() {
		Assert.assertTrue(EulerMath.isPandigital("1"));
		Assert.assertTrue(EulerMath.isPandigital("14523"));
		Assert.assertTrue(EulerMath.isPandigital("312"));
		Assert.assertTrue(EulerMath.isPandigital("21"));
		Assert.assertTrue(EulerMath.isPandigital("129384756"));

		Assert.assertFalse(EulerMath.isPandigital("13345345324595"));
		Assert.assertFalse(EulerMath.isPandigital("13"));
		Assert.assertFalse(EulerMath.isPandigital("11"));
		Assert.assertFalse(EulerMath.isPandigital("10"));
		Assert.assertFalse(EulerMath.isPandigital("1987654312"));
	}

	@Test
	public void testIsFibonacci() {
		Assert.assertTrue(EulerMath.isFibonacci(1));
		Assert.assertTrue(EulerMath.isFibonacci(2));
		Assert.assertTrue(EulerMath.isFibonacci(3));
		Assert.assertTrue(EulerMath.isFibonacci(5));
		Assert.assertTrue(EulerMath.isFibonacci(8));
		Assert.assertTrue(EulerMath.isFibonacci(13));
		Assert.assertTrue(EulerMath.isFibonacci(21));
		Assert.assertFalse(EulerMath.isFibonacci(6));
	}

	@Test
	public void testGeneratePandigitalSequences() throws Exception {
		// String[] sequences = EulerMath.generatePandigitalSequences(9);
		// Assert.assertEquals(362880, sequences.length);

		for (int i = 1; i < 10; i++) {
			List<String> sequences = EulerMath.generatePandigitalSequences(i);
			Assert.assertEquals(Integer.toString(i), EulerMath.factorial(i),
					sequences.size());

			HashSet<String> hashish = new HashSet<String>();
			for (int j = 0; j < sequences.size(); j++) {
				String message = String.format("%d => %d => %s", new Object[] {
						i, j, sequences.get(j) });
				Assert.assertTrue(message, hashish.add(sequences.get(j)));
				Assert.assertTrue(
						Integer.toString(i) + " => " + Integer.toString(j)
								+ " => " + sequences.get(j),
						EulerMath.isPandigital(sequences.get(j)));
			}
		}
	}

	@Test
	public void testGetFibonacci() throws Exception {
		long[] fibonaccis = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
		for (int i = 0; i < fibonaccis.length; i++) {
			Assert.assertEquals(fibonaccis[i], EulerMath.getFibonacci(i));
		}
	}

	@Test
	public void testBinomialCoefficient() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	public void testBigBinomialCoefficient() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	public void testNextFibonacci() {
		fail("Not yet implemented"); // TODO
	}

}
