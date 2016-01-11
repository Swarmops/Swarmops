using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Swarmops.Logic.Financial;

namespace Swarmops.Tests.Financial
{
    [TestFixture]
    class BitcoinUtilityTest
    {
        [Test]
        [TestCase]
        public void ValidateBitcoinAddressTest()
        {
            Assert.IsTrue(BitcoinUtility.IsValidBitcoinAddress("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i")); // VALID
            Assert.IsTrue(BitcoinUtility.IsValidBitcoinAddress("1Q1pE5vPGEEMqRcVRMbtBK842Y6Pzo6nK9")); // VALID
            Assert.IsTrue(BitcoinUtility.IsValidBitcoinAddress("3KS6AuQbZARSvqnaHoHfL1Xhm3bTLFAzoK")); // VALID MULTISIG
            Assert.IsFalse(BitcoinUtility.IsValidBitcoinAddress("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62X")); // checksum changed, original data
            Assert.IsFalse(BitcoinUtility.IsValidBitcoinAddress("1ANNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i")); // data changed, original checksum
            Assert.IsFalse(BitcoinUtility.IsValidBitcoinAddress("1A Na15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i")); // invalid chars
            Assert.IsFalse(BitcoinUtility.IsValidBitcoinAddress("BZbvjr")); // checksum is fine, address too short
        }
    }
}
