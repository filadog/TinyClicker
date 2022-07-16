using System;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TinyClicker;

internal class ImageToText
{
    readonly TesseractEngine _tesseract;
    readonly ImageEditor _imageEditor;

    public ImageToText(ImageEditor editor)
    {
        _tesseract = new TesseractEngine(@"./tessdata", "pixel", EngineMode.LstmOnly);
        _imageEditor = editor;
    }

    public int ParseBalance(Image window)
    {
        Bitmap source = _imageEditor.GetAdjustedBalanceImage(window);
        string result;
        try
        {
            using (var page = _tesseract.Process(source, PageSegMode.SingleLine))
            {
                result = page.GetText().Trim();
                source.Dispose();
                return ResultToBalance(result);
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }

    int ResultToBalance(string result)
    {
        if (result.Contains('M'))
        {
            int endIndex = result.IndexOf('M');
            result = result[..endIndex];
            result = TrimWithRegex(result);
            result += "000";
            return Convert.ToInt32(result);
        }
        else if (result.Contains(' '))
        {
            int endIndex = result.IndexOf(' ');
            result = result[..endIndex];
            return Convert.ToInt32(TrimWithRegex(result));
        }
        else
        {
            return Convert.ToInt32(TrimWithRegex(result));
        }
    }

    string TrimWithRegex(string str)
    {
        str = Regex.Replace(str, "[^0-9]", "").Trim();
        return str;
    }
}
