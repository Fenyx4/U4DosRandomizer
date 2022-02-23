using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer.UnitTests
{
    [TestClass]
    public class SextantCoordToHex
    {
        [TestMethod]
        public void AA_To_CorrectHex()
        {
            // Arrange 

            // Act 
            var result = ItemOptions.SextantCoordToHex('A', 'A');

            // Assert 
            Assert.AreEqual(0x0, result);
        }

        [TestMethod]
        public void BP_To_CorrectHex()
        {
            // Arrange 

            // Act 
            var result = ItemOptions.SextantCoordToHex('B', 'P');

            // Assert 
            Assert.AreEqual(0x1F, result);
        }
    }
}
