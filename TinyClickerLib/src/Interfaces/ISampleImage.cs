using OpenCvSharp;
using System;


namespace TinyClickerLib.Interfaces;

internal interface ISampleImage
{
    public int OrderedNumber { get; }
    string ImageName { get; }
    Mat Mat { get; }
}
