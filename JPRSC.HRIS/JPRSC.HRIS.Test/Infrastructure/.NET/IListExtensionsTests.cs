using System.Collections.Generic;
using Xunit;

namespace JPRSC.HRIS.Test.Infrastructure.NET
{
    public class IListExtensionsTests
    {
        [Fact]
        public void RemoveTrailingWhiteSpace_GivenNoTrailingWhiteSpace_ShouldReturnUnmodified()
        {
            // Arrange
            var list = new List<string>() { "A", "B", "C" };

            // Act
            var withoutTrailingWhiteSpace = list.RemoveTrailingWhitespace();

            // Assert
            Assert.Equal(3, withoutTrailingWhiteSpace.Count);
        }

        [Fact]
        public void RemoveTrailingWhiteSpace_GivenTrailingWhiteSpace01_ShouldReturnListWithNoTrailingWhiteSpace()
        {
            // Arrange
            var list = new List<string>() { "A", "B", "C", "" };

            // Act
            var withoutTrailingWhiteSpace = list.RemoveTrailingWhitespace();

            // Assert
            Assert.Equal(3, withoutTrailingWhiteSpace.Count);
        }

        [Fact]
        public void RemoveTrailingWhiteSpace_GivenTrailingWhiteSpace02_ShouldReturnListWithNoTrailingWhiteSpace()
        {
            // Arrange
            var list = new List<string>() { "A", "B", "C", "D", "E", "", "" };

            // Act
            var withoutTrailingWhiteSpace = list.RemoveTrailingWhitespace();

            // Assert
            Assert.Equal(5, withoutTrailingWhiteSpace.Count);
        }

        [Fact]
        public void RemoveTrailingWhiteSpace_GivenTrailingWhiteSpace03_ShouldReturnListWithNoTrailingWhiteSpace()
        {
            // Arrange
            var list = new List<string>() { "A", "", "", "", "" };

            // Act
            var withoutTrailingWhiteSpace = list.RemoveTrailingWhitespace();

            // Assert
            Assert.Equal(1, withoutTrailingWhiteSpace.Count);
        }
    }
}
