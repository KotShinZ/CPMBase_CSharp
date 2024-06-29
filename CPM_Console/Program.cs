using System;
using System.Threading.Tasks;
using CPMBase;
using CPMBase.Base;
using CPMBase.Base.Datas;
using CPMBase.CPM;
using CPMBase.CPMDiffusion;
using CPMBase.Examples;


public class Program
{
    public static int seed = 0;

    public static void Main(string[] args)
    {
        ((ISimration)new SimpleSim<CPMArea>()).Run();
        //((ISimration)new OnlyDiffusion()).Run();
        //Start().GetAwaiter().GetResult();
        //Class1.Main();
    }

    public static async Task Start()
    {
        Console.WriteLine("実行開始");
        ISimration sim = new SweapDiffusionSim<CPMArea>();
        await sim.Run();
        Console.WriteLine("実行終了");
    }
}