using Microsoft.VisualStudio.TestTools.UnitTesting;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer.UnitTests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void StripThe_Empty_StillEmpty()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void StripThe_Null_StillNull()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.StripThe();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void StripThe_NoThe_Unchanged()
        {
            // Arrange
            var input = "Bloody Plains";

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual("Bloody Plains", result);
        }

        [TestMethod]
        public void StripThe_TheInTheMiddle_Unchanged()
        {
            // Arrange
            var input = "Bloody The Plains";

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual("Bloody The Plains", result);
        }

        [TestMethod]
        public void StripThe_TheAtBeginningOfWord_Unchanged()
        {
            // Arrange
            var input = "theBloody Plains";

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual("theBloody Plains", result);
        }

        [TestMethod]
        public void StripThe_UpperCase_Changed()
        {
            // Arrange
            var input = "The Bloody Plains";

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual("Bloody Plains", result);
        }

        [TestMethod]
        public void StripThe_LowerCase_Changed()
        {
            // Arrange
            var input = "the Bloody Plains";

            // Act
            var result = input.StripThe();

            // Assert
            Assert.AreEqual("Bloody Plains", result);
        }
    }
}
