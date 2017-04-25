package tests;

import static org.junit.Assert.*;

import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

import junit.framework.Assert;

import org.junit.Before;
import org.junit.Test;

import euler.sequence.CollatzSequence;
import euler.sequence.SequenceGenerator;

public class CollatzSequenceTest extends SequenceTest {

	@Test
	public void testRecurse() {
		// 13 → 40 → 20 → 10 → 5 → 16 → 8 → 4 → 2 → 1
		Assert.assertEquals(40, sequence.recurse(13));
		Assert.assertEquals(20, sequence.recurse(40));
		Assert.assertEquals(10, sequence.recurse(20));
		Assert.assertEquals(5, sequence.recurse(10));
		Assert.assertEquals(16, sequence.recurse(5));
		Assert.assertEquals(8, sequence.recurse(16));
		Assert.assertEquals(4, sequence.recurse(8));
		Assert.assertEquals(2, sequence.recurse(4));
		Assert.assertEquals(1, sequence.recurse(2));
	}

	@Test
	public void testSeive() {
		Map<Long, Boolean> map = new HashMap<Long, Boolean>();
		map.put(1L, true);
		map.put(2L, false);
		map.put(3L, false);
		map.put(200000L, false);
		testSeive(map);
	}

	@Override
	@Before
	public void setUp() throws Exception {
		sequence = new CollatzSequence();
	}
}
