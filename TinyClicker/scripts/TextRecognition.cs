using System;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TinyClickerUI
{
    internal class TextRecognition
    {
        public static int ParseBalance(Image window)
        {
            int balance;
            Bitmap source = ImageHandler.CropCurrentBalance(window);
            string text;

            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.Default))
                {
                    using (var img = source)
                    {
                        using (var page = engine.Process(img))
                        {
                            text = page.GetText();
                        }
                    }
                }

                balance = Convert.ToInt32(Regex.Replace(text, "[^0-9]", ""));
                return balance;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
