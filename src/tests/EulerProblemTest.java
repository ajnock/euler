package tests;

import static org.junit.Assert.*;

import java.util.Dictionary;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import euler.EulerProblem;
import euler.SolutionFactory;
import euler.SolvableProblem;

public class EulerProblemTest {

	public EulerProblemTest() {
	}

	String[] answers;

	@Before
	public void setUp() throws Exception {
		answers = new String[430];
		answers[1] = "233168";
		answers[2] = "4613732";
		answers[4] = "906609";
		answers[7] = "104743";
		answers[35] = "55";
		answers[41] = "7652413";
		answers[79] = "73162890";
	}

	@After
	public void tearDown() throws Exception {
	}

	@Test
	public void testGetSolution() throws Exception {
		for (int i = 0; i < answers.length; i++) {
			if (answers[i] == null)
				continue;
			EulerProblem problem = (EulerProblem) SolutionFactory
					.GenerateProblem(i);
			Object solution = problem.solve();
			assertEquals(problem.toString(), solution.toString(), problem.getSolution());
			assertEquals(answers[i], problem.getSolution());
		}
	}

	@Test
	public void testRun() throws InterruptedException {
		for (int i = 0; i < answers.length; i++) {
			if (answers[i] == null)
				continue;
			EulerProblem problem = (EulerProblem) SolutionFactory
					.GenerateProblem(i);
			Thread t = new Thread(problem);
			t.start();
			t.join();
			// assertEquals(problem.toString(), answers[i],
			// problem.getSolution());
		}
	}

	@Test
	public void testToString() {
		for (int i = 0; i < answers.length; i++) {
			if (answers[i] == null)
				continue;
			EulerProblem problem = (EulerProblem) SolutionFactory
					.GenerateProblem(i);
			try {
				problem.solve();
			} catch (Exception e) {
				System.out.println(i);
				e.printStackTrace();
				assertFalse(Integer.toString(i), true);
			}

			String str = problem.toString();
			assertTrue(str, str.endsWith("!"));
		}
	}
}
