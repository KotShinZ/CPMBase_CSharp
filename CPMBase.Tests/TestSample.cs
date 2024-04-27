namespace CPMBase.Tests;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class TestSample
{
    [Fact]
    public void Test1()
    {
        Assert.Equal(2, 1 + 1);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(3, 7, 10)]
    public void Test2(int value1, int value2, int expected)
    {
        Assert.Equal(expected, value1 - value2);
    }

    [Fact(DisplayName = "abcTest")]
    public void Test3()
    {
        Assert.Contains("abc", "abcde");
    }
}