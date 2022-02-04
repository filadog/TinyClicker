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
        public void ParseBalanceFromImage_23826()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\23826.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 23826;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }

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
        public void ParseBalanceFromImage_103657()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\103657.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 103657;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_117870()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\117870.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 117870;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_132317()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\132317.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 132317;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_149478()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\149478.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 149478;
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

        [Fact]
        public void ParseBalanceFromImage_203635()
        {
            string fileName = Environment.CurrentDirectory + @"\samples\Tests\BalanceImageSamples\203635.png";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Could not find file at path: {fileName}");
            }
            var image = Image.FromFile(fileName);
            int expected = 203635;
            int actual = TextProcessor.ParseBalance(image);

            Assert.Equal(expected, actual);
        }
    }
}
