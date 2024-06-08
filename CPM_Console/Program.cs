using System;
using System.Threading.Tasks;
using CPMBase.Base;
using CPMBase.Examples;


public class Program
{
    public static int seed = 0;

    public static void Main(string[] args)
    {
        //ISimration sim = new ParamDiffusionExample();
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