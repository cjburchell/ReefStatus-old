
namespace MyCompany.Extensions.Test
{
    using NUnit.Framework;

    /// <summary>
    /// String extensions tests
    /// </summary>
    [TestFixture]
    public static class StringExtensionsTest
    {
        /// <summary>
        /// Unit test for the reverse extension.
        /// </summary>
        [Test]
        public static void TestReverse()
        {
            // Setup
            var testString = "HELLO";

            // Act
            var result = testString.Reverse();

            //Test
            Assert.That(result, Is.EqualTo("OLLEH"));
        }
    }
}
