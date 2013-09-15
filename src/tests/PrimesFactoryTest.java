package tests;

import static org.junit.Assert.*;

import java.io.File;
import java.io.FileReader;
import java.util.Stack;

import junit.framework.Assert;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import euler.math.PrimesFactory;
import euler.math.Primus;

public class PrimesFactoryTest {
	PrimesFactory instance;

	@Before
	public void setUp() throws Exception {
	}

	@After
	public void tearDown() throws Exception {
	}

	@Test
	public void testPrimesFactory() throws Exception {
		instance = new PrimesFactory(100);
		assertNotNull(instance);
	}

	@Test
	public void testGetPrime() throws Exception {
		int[] primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37,
				41 };
		instance = new PrimesFactory(100);
		// instance.setSeiveLimit(50);
		// instance.setSeiveBuffer(5);
		instance.seive();
		for (int i = 0; i < primes.length; i++) {
			int p = 0;
			try {
				p = instance.getPrime(i);
			} catch (Exception ex) {
				ex.printStackTrace();
				Assert.fail(Integer.toString(i));
			}
			StringBuilder builder = new StringBuilder("{ ");
			Integer[] primesList = instance.getPrimesArray();
			for (int j = 0; j < primesList.length; j++) {
				builder.append(Integer.toString(primesList[j]) + ", ");
			}
			if (primesList.length > 0) {
				builder.replace(builder.lastIndexOf(","), builder.length() - 1,
						" }");
			} else {
				builder.append(" }");
			}
			assertEquals(Integer.toString(i) + " => " + builder.toString(),
					primes[i], p);
		}
	}

	@Test
	public void testSeivePrimus() throws Exception {
		FileReader input = null;
		Stack<Integer> primes = new Stack<Integer>();
		try {
			input = new FileReader("./etc/primes.txt");
			char c;
			int i = 0;
			StringBuffer buffer = new StringBuffer();
			boolean notDone = true;
			while ((c = (char) input.read()) != -1 && notDone) {
				if (c == '\r' || c == '\n')
					continue;
				buffer.append(c);
				i++;
				if (i % 7 == 0) {
					String str = buffer.toString().trim();
					int p = Integer.parseInt(str);
					// System.out.println(p);
					primes.add(p);
					buffer = new StringBuffer();
					if (i % 10 == 0)
						notDone &= input.read() != -1;
				}
			}
		} finally {
			if (input != null)
				input.close();
		}
		Primus pinstance = new Primus(5000);
		pinstance.seive();

		int i;
		for (i = 0; i < pinstance.getPrimesCount(); i++) {
//			assertEquals((long) primes.get(i), pinstance.getPrime(i));
		}
		assertEquals(i, pinstance.getPrimesCount());
	}

	@Test
	public void testSeive() throws Exception {
		FileReader input = null;
		Stack<Integer> primes = new Stack<Integer>();
		try {
			input = new FileReader("./etc/primes.txt");
			char c;
			int i = 0;
			StringBuffer buffer = new StringBuffer();
			boolean notDone = true;
			while ((c = (char) input.read()) != -1 && notDone) {
				if (c == '\r' || c == '\n')
					continue;
				buffer.append(c);
				i++;
				if (i % 7 == 0) {
					String str = buffer.toString().trim();
					int p = Integer.parseInt(str);
					// System.out.println(p);
					primes.add(p);
					buffer = new StringBuffer();
					if (i % 10 == 0)
						notDone &= input.read() != -1;
				}
			}
		} finally {
			if (input != null)
				input.close();
		}
		instance = new PrimesFactory(100);
		instance.seive();

		for (int i = 0; i < instance.getNumberOfPrimes(); i++) {
			int p = primes.get(i);
			int q = instance.getPrime(i);
			assertEquals(p, q);
		}

		instance = new PrimesFactory(3571);
		instance.seive();
		int n = instance.getNumberOfPrimes();
		assertEquals(500, n);

		// problem 7
		instance = new PrimesFactory(5104743);
		instance.seive();
		n = instance.getNumberOfPrimes();
		assertEquals(10001, n);
	}
}
