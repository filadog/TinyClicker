using System;
using System.Drawing;
using System.IO;
using TinyClickerUI;
using Xunit;

namespace TinyClickerTests.TextProcessorTests
{
    public class TextProcessorTests
    {

        [Fact]
        public void ParseBalanceFromImage_96245()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\96245.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 96245;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_188214()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\188214.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 188214;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }
    }
}
