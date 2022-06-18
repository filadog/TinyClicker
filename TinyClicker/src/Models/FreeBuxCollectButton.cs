using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace TinyClicker;

internal class FreeBuxCollectButton : ISampleImage
{
    public readonly string name = "FreeBuxCollectButton";
    private string imagePath = @".\samples\free_bux_collect_button.png";

    public FreeBuxCollectButton()
    {
        Mat = GenerateMat(imagePath);
    }

    public Mat Mat { get => Mat; private set => Mat = value; }

    public Mat GenerateMat(string imagePath)
    {
        var img = Image.FromFile(imagePath);
        return BitmapConverter.ToMat((Bitmap)img);
    }
}