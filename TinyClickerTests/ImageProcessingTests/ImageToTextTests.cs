using Microsoft.VisualBasic;
using OpenCvSharp;
using System.Drawing;
using TinyClicker;
using Xunit;

namespace TinyClickerTests.ImageProcessing
{
    public class ImageToTextTests
    {
        // Tesseract Tests
        // Accuracy of the OCR for the exact numbers
        Rectangle _rect;
        ImageEditor _imageEditor;
        ImageToText _imageToText;
        public ImageToTextTests()
        {
            _rect = new Rectangle();
            _imageEditor = new ImageEditor(_rect);
            _imageToText = new ImageToText(_imageEditor);
        }

        [Fact]
        public void ParseBalanceFromImage_96245()
        {
            var image = TestHelper.LoadBalanceSample("96245");
            int expected = 96245;
            int actual = _imageToText.ParseBalance(image);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_188214()
        {
            var image = TestHelper.LoadBalanceSample("188214");
            int expected = 188214;
            int actual = _imageToText.ParseBalance(image);
            Assert.Equal(expected, actual);
        }

        // Accuracy of the OCR for the amount of digits

        [Fact]
        public void ParseBalanceFromImage_8_digits_1()
        {
            var image = TestHelper.LoadBalanceSample("10130000");
            int expectedLength = 8;
            int actual = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actual);
        }

        [Fact]
        public void ParseBalanceFromImage_8_digits_2()
        {
            var image = TestHelper.LoadBalanceSample("11220000");
            int expectedLength = 8;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void ParseBalanceFromImage_8_digits_3()
        {
            var image = TestHelper.LoadBalanceSample("32M");
            int expectedLength = 8;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void ParseBalanceFromImage_7_digits_1()
        {
            var image = TestHelper.LoadBalanceSample("1253000");
            int expectedLength = 7;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void ParseBalanceFromImage_7_digits_2()
        {
            var image = TestHelper.LoadBalanceSample("3463M");
            int expectedLength = 7;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void ParseBalanceFromImage_6_digits_1()
        {
            var image = TestHelper.LoadBalanceSample("109407");
            int expectedLength = 6;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        [Fact]
        public void ParseBalanceFromImage_6_digits_2()
        {
            var image = TestHelper.LoadBalanceSample("445007");
            int expectedLength = 6;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }


        [Fact]
        public void ParseBalanceFromImage_5_digits()
        {
            var image = TestHelper.LoadBalanceSample("64357");
            int expectedLength = 5;
            int actualLength = _imageToText.ParseBalance(image).ToString().Length;
            Assert.Equal(expectedLength, actualLength);
        }

        
    }
}
