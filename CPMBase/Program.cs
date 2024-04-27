using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Numerics;
using CPMBase;
using CPMBase.ExSimrations;
using SkiaSharp;


public class Program
{
    public static int seed = 0;

    public static void Main(string[] args)
    {
        ISimration sim = new MiniExample();
        sim.Run();
    }

    public void a()
    {
        /* Console.WriteLine("Hello World!");
         RangePosition rangePosition = new RangePosition(100, 0, 100, 0, 100, 0);
         var bitmap = new SKBitmap(256, 256);
         //var canvas = new SKCanvas(bitmap);
         //rangePosition.DrawAxis(canvas, 256, 256);
         new PathObject("/HeatMapTest", "test", extention: ".png").WriteToImgFile(bitmap);*/
    }
}
