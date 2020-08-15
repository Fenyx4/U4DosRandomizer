using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace U4DosRandomizer.UnitTests
{
    [TestClass]
    public class WorldMapTests
    {
        [TestMethod]
        public void Between_NoWrap_Inside()
        {
            // Arrange

            // Act
            var result = WorldMap.Between(5, 2, 7);

            // Assert
            Assert.IsTrue(result, "5 is between 2 to 7");
        }

        [TestMethod]
        public void Between_NoWrap_Outside()
        {
            // Arrange

            // Act
            var result = WorldMap.Between(8, 2, 7);

            // Assert
            Assert.IsFalse(result, "8 is outside 2 to 7");
        }

        [TestMethod]
        public void Between_Wrap_Inside()
        {
            // Arrange

            // Act
            var result = WorldMap.Between(8, 7, 2);

            // Assert
            Assert.IsTrue(result, "8 is between 7 to 2");
        }

        [TestMethod]
        public void Between_Wrap_Outside()
        {
            // Arrange

            // Act
            var result = WorldMap.Between(5, 7, 2);

            // Assert
            Assert.IsFalse(result, "5 is outside 7 to 2");
        }
    }
}
