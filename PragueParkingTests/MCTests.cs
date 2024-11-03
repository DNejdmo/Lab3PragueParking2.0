using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParkingAccess;

namespace PragueParkingTests
{
    [TestClass]
    public class MCTests
    {
        [TestMethod]
        public void GetInfo_ShouldReturnCorrectInfo()
        {
            // Arrange
            var mc = new MC("ABC123");

            // Act
            var result = mc.GetInfo();

            // Assert
            Assert.AreEqual("MC ABC123", result);
        }
    }
}
