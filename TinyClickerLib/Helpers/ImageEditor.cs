using System;
using System.Drawing;

namespace TinyClicker;

public class ImageEditor
{
    Rectangle _screenRect;
    Rectangle _balanceRect;
    readonly ClickerActionsRepo _actionsRepo;
    bool _isBalanceLocationFound;

    public ImageEditor(Rectangle screenRect, ClickerActionsRepo actionsRepo)
    {
        _screenRect = screenRect;
        _actionsRepo = actionsRepo;
        _isBalanceLocationFound = false;
    }

    public Bitmap GetAdjustedBalanceImage(Image window)
    {
        if (!_isBalanceLocationFound)
        {
            _balanceRect = GetBalanceRect(_screenRect);
        }
        var result = CropCurrentBalance(window);

        // Uncomment to save the balance image for manual checking
        string filename = @"./screenshots/balance.png";
        WindowToImage.SaveScreenshot(result, filename);

        return result;
    }

    public Bitmap CropCurrentBalance(Image window)
    {
        // Crop the image
        var cropRect = _balanceRect;
        var bitmap = new Bitmap(cropRect.Width, cropRect.Height);
        using (var gr = Graphics.FromImage(bitmap))
        {
            gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), cropRect, GraphicsUnit.Pixel);
        }
        
        // Invert the image
        for (int y = 0; y <= (bitmap.Height - 1); y++)
        {
            for (int x = 0; x <= (bitmap.Width - 1); x++)
            {
                Color inv = bitmap.GetPixel(x, y);
                inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                bitmap.SetPixel(x, y, inv);
            }
        }
        return bitmap;
    }

    Rectangle GetBalanceRect(Rectangle screenRect)
    {
        int posX = 0;
        int posY = 0;

        // Check for balance coordinates on the screen
        if (!_isBalanceLocationFound)
        {
            bool found = _actionsRepo.IsImageFound("balanceCoin", out OpenCvSharp.Point location);
            posX = location.X + 15;
            posY = location.Y - 5;
            if (found)
            {
                _isBalanceLocationFound = true;
            }
        }

        // Adjust coordinates to the actual window size
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)posX * 100 / 333) / 100;
        float y1 = ((float)posY * 100 / 592) / 100;
        int x2 = (int)(rectX * x1);
        int y2 = (int)(rectY * y1);

        return new Rectangle(x2, y2, 74, 25);
    }
}
