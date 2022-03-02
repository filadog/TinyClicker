using System;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace TinyClickerUI
{
    internal class ImageToText
    {
        public static int ParseBalance(Image window)
        {
            Bitmap source = ImageEditor.FetchBalanceImageAdjusted(window);
            string result = "";
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.LstmOnly))
                {
                    //engine.SetVariable("tessedit_char_whitelist", "0123456789M,.");
                    using (var page = engine.Process(source, PageSegMode.SingleLine))
                    {
                        result = page.GetText().Trim();
                        return ResultToBalance(result);
                    }
                }
                
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int ResultToBalance(string result)
        {
            int balance = 0;
            if (result[result.Length - 1] == '1')
            {
                result = Regex.Replace(result, "[^0-9]", "");
                if (result[result.Length - 2] == '0')
                {
                    result = result.Remove(4);
                }
                result += "000";
                balance = Convert.ToInt32(result);
            }
            else
            {
                balance = Convert.ToInt32(Regex.Replace(result, "[^0-9]", "").Trim());
            }
            return balance;
        }
    }
}
