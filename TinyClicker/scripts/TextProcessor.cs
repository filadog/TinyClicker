using System;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace TinyClickerUI
{
    internal class TextProcessor
    {
        public static int ParseBalance(Image window)
        {
            int balance;
            Bitmap source = ImageProcessor.FetchBalanceImageAdjusted(window);
            string text;

            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.Default))
                {
                    using (var img = source)
                    {
                        using (var page = engine.Process(img, PageSegMode.SingleLine))
                        {
                            text = page.GetText();
                        }
                    }
                }

                // Check the balance if it is in range of thousands or millions
                if (text[1] == '.' || text[1] == ',')
                {
                    text = Regex.Replace(text, "[^0-9]", "");
                    text = text.Remove(4);
                    text = text + "000";
                    balance = Convert.ToInt32(text);
                }
                else
                {
                    balance = Convert.ToInt32(Regex.Replace(text, "[^0-9]", ""));
                }

                return balance;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
