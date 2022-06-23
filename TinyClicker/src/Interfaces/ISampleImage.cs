using OpenCvSharp;
using System;


namespace TinyClicker.Interfaces;

internal interface ISampleImage
{
    public int OrderedNumber { get; }
    string ImageName { get; }
    Mat Mat { get; }

    Mat GenerateMat();
}
