using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EulerTest
{
    [TestClass]
    public class EulerTest
    {
        [TestMethod]
        public void TestIsPandigital()
        {
            Assert.IsTrue(Euler.Euler.IsPandigitial(2143));
            Assert.IsFalse(Euler.Euler.IsPandigitial(21430));
            Assert.IsFalse(Euler.Euler.IsPandigitial(143));
            Assert.IsFalse(Euler.Euler.IsPandigitial(10));
            Assert.IsFalse(Euler.Euler.IsPandigitial(21643));
            Assert.IsTrue(Euler.Euler.IsPandigitial(123456789));
            Assert.IsFalse(Euler.Euler.IsPandigitial(1234567890));
        }
    }
}
