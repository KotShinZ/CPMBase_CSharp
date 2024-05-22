using CPMBase.Base;
using CPMBase.Examples;
using System.Reflection;


public class Program
{
    public static void Main()
    {
        ((ISimration)new SimpleSim()).Run();
    }
}