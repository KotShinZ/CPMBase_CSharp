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
        //ISimration sim = new SweapDiffusionSim();
        //sim.Run();
        Start().GetAwaiter().GetResult();
    }

    public static async Task Start()
    {
        ISimration sim = new SweapDiffusionSim();
        await sim.Run();
        Console.WriteLine("実行終了");
    }
}
