using System;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace TinyClicker;

internal class ImageToText
{
    TesseractEngine _tesseract;
    ImageEditor _imageEditor;

    public ImageToText(ImageEditor editor)
    {
        _tesseract = new TesseractEngine(@"./tessdata", "digits_comma", EngineMode.LstmOnly);
        _imageEditor = editor;
    }

    public int ParseBalance(Image window)
    {
        Bitmap source = _imageEditor.GetBalanceImageAdjusted(window);
        string result = "";
        try
        {
            //engine.SetVariable("tessedit_char_whitelist", "0123456789M,.");
            using (var page = _tesseract.Process(source, PageSegMode.SingleLine))
            {
                result = page.GetText().Trim();
                return ResultToBalance(result);
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public int ResultToBalance(string result)
    {
        int balance = 0;
        if (result[result.Length - 1] == '1' || result[result.Length - 1] == '0')
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
