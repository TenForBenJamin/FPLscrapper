using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using Xunit;

namespace ConsoleApp1.FPL
{
    internal class MainFPL
    {
        private readonly string[] fruits = { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

        [Fact]
        public void TestIfContainsBanana()
        {
            // Act & Assert
            Assert.Contains("Bananas", fruits); // Checks if "Banana" is in the array
        }
    }

}
