using Xunit;

namespace CursorConvertedEFCoreApp.Tests;

public class SimpleTest
{
    [Fact]
    public void SimpleTest_ShouldPass()
    {
        // Arrange
        var expected = 2;
        var actual = 1 + 1;

        // Assert
        Assert.Equal(expected, actual);
    }
} 