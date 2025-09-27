using Xunit;

namespace ParanoidAndroid.Tests
{
    public static class SampleLogic
    {
        // Example pure function we can evolve separately from RimWorld runtime.
        public static int ClampToPositive(int value) => value < 0 ? 0 : value;
    }

    public class SampleLogicTests
    {
        [Theory]
        [InlineData(-5, 0)]
        [InlineData(0, 0)]
        [InlineData(3, 3)]
        public void ClampToPositive_Works(int input, int expected)
        {
            Assert.Equal(expected, SampleLogic.ClampToPositive(input));
        }
    }
}
