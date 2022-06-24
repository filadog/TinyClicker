using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using TinyClicker.Interfaces;

namespace TinyClicker.Models;

internal class SampleImage : ISampleImage
{
    public string ImageName { get; }

    public int OrderedNumber { get; }

    public Mat Mat { get; }

    public SampleImage(string imageName, int orderNum)
    {
        OrderedNumber = orderNum;
        ImageName = imageName;
        Mat = GenerateMat();
    }

    Mat GenerateMat()
    {
        // Resize mat here
        var img = Image.FromFile(ImageName);
        return BitmapConverter.ToMat((Bitmap)img);
    }
}