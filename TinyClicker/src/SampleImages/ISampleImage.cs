using OpenCvSharp;
using System;


namespace TinyClicker.src.SampleImages
{
    internal interface ISampleImage
    {
        Mat Mat { get; }

        Mat GenerateMat(string imagePath);
    }
}
