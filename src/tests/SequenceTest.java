package tests;

import java.util.Dictionary;
import java.util.Map;
import java.util.Map.Entry;

import junit.framework.Assert;

import org.junit.After;
import org.junit.Before;

import euler.sequence.ISequence;

public abstract class SequenceTest {

	protected ISequence sequence;
	protected void testSeive(Map<Long, Boolean> map) {
		for ( Entry<Long, Boolean> entry : map.entrySet() ) {
		    Long key = entry.getKey();
		    boolean value = entry.getValue();
		    Assert.assertEquals(key.toString(),value, sequence.seive(key));
		}
	}

	@Before
	public abstract void setUp() throws Exception;

	@After
	public void tearDown() throws Exception {
	}

}