using PragueParkingAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParkingTests
{
    [TestClass]
    public class VehicleTests
    {
        [TestMethod]
        public void ParkingTime_ShouldBeSetToCurrentTime()
        {
            // Arrange
            var currentTime = DateTime.Now;

            // Act
            var car = new Car("DEF456");

            // Assert
            Assert.IsTrue((car.ParkingTime - currentTime).TotalSeconds < 1,
                "ParkingTime should be set to the current time upon creation.");
        }
    }
}