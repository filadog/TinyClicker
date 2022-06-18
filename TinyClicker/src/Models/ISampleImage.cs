using OpenCvSharp;
using System;


namespace TinyClicker;

internal interface ISampleImage
{
    Mat Mat { get; }

    Mat GenerateMat(string imagePath);
}
