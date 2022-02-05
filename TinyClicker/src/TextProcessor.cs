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
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.LstmOnly))
                {
                    engine.SetVariable("tessedit_char_whitelist", "0123456789");
                    using (var img = source)
                    {
                        using (var page = engine.Process(img, PageSegMode.SparseText))
                        {
                            text = page.GetText();
                            text = text.Trim();
                        }
                    }
                }
                
                // Check the balance if it is in the range of thousands or millions
                if (text.Length > 5 && text[1] == '.' || text[1] == ',')
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
