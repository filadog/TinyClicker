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
                using (var engine = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.LstmOnly))
                {
                    //engine.SetVariable("tessedit_char_whitelist", "0123456789M,.");
                    using (var img = source)
                    {
                        using (var page = engine.Process(img, PageSegMode.SingleLine))
                        {
                            text = page.GetText().Trim();
                        }
                    }
                }
                
                // Check the balance exponent (thousands or millions)
                //if (text.Length > 5 && text[1] == '.' || text[1] == ',')
                //{
                //    text = Regex.Replace(text, "[^0-9]", "").Remove(4);
                //    text += "000";
                //    balance = Convert.ToInt32(text);
                //}

                if (text[text.Length - 1] == '1')
                {
                    text = Regex.Replace(text, "[^0-9]", "");
                    if (text[text.Length - 2] == '0')
                    {
                        text = text.Remove(4);
                    }
                    text += "000";
                    balance = Convert.ToInt32(text);
                }
                else
                {
                    balance = Convert.ToInt32(Regex.Replace(text, "[^0-9]", "").Trim());
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
