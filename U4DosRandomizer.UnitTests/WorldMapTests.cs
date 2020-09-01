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

        [TestMethod]
        public void ActualExample()
        {
            // Arrange
            // stygian 148, 11
            // shapeLoc 146, 4
            var shapeLoc = new Coordinate(146, 4);

            // Act
            var result = WorldMap.Between(146, WorldMap.Wrap(shapeLoc.X - 12), WorldMap.Wrap(shapeLoc.X + 12)) && WorldMap.Between(255, WorldMap.Wrap(shapeLoc.Y - 12), WorldMap.Wrap(shapeLoc.Y + 12));

            // Assert
            Assert.IsTrue(result, "146, 255 is inside 146-12, ");
        }

        [TestMethod]
        public void ActualExampleOutside()
        {
            // Arrange
            // stygian 148, 11
            // shapeLoc 146, 4
            var shapeLoc = new Coordinate(146, 4);

            // Act
            var result = WorldMap.Between(146, WorldMap.Wrap(shapeLoc.X - 12), WorldMap.Wrap(shapeLoc.X + 12)) && WorldMap.Between(200, WorldMap.Wrap(shapeLoc.Y - 12), WorldMap.Wrap(shapeLoc.Y + 12));

            // Assert
            Assert.IsFalse(result, "146, 255 is outside 146-12, ");
        }
    }
}
