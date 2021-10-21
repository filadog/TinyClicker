using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Interop;
using Tesseract;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TinyClicker
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
                using (var engine = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.Default)) //@"./tessdata", "eng", EngineMode.Default
                {
                    using (var img = source)
                    {
                        using (var page = engine.Process(img))
                        {
                            text = page.GetText();

                            //Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                            //Console.WriteLine("Text (GetText): \r\n{0}", text);
                            
                            
                            //double.TryParse(text, out balance);
                            //Console.WriteLine("Current balance: {0}", balance);
                        }
                    }
                }

                //Console.WriteLine("Text: " + text);
                balance = Convert.ToInt32(Regex.Replace(text, "[^0-9]", ""));
                //Console.WriteLine(balance);
                return balance;
            }
            catch (Exception)
            {
                //Trace.TraceError(e.ToString());
                //Console.WriteLine("Unexpected Error: " + e.Message);
                //Console.WriteLine("Details: ");
                //Console.WriteLine(e.ToString());
                return 0;
            }
        }
    }
}
