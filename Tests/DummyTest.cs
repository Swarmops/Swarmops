using NUnit.Framework;

namespace Tests
{
    class DummyTest
    {

        [TestCase]
        public void ThisTestShouldSuccedd()
        {
            Assert.True(true);
        }

        [TestCase]
        public void ThisTestShouldFail()
        {
            Assert.Fail();
        }


    }
}
