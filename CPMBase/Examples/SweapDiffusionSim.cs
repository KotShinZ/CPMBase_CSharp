using System.Runtime.CompilerServices;
using CPMBase.ExSimrations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ScottPlot.Colormaps;

namespace CPMBase;

public class SweapDiffusionSim : ISimration
{
    public virtual int sweapNum => 5;

    public List<Task> sims = new();

    public List<CPMSimurationBase> cPMSimurationBases = new();

    public virtual Action<CPMSimurationBase, int> sweapAction => (sim, n) =>
    {
        sim.cPMAreaArray.AllCellFunc(c => c.kAdhesion += 10 * n);
    };

    public Type type => typeof(DiffusionSim);

    public async Task Run()
    {
        for (int i = 0; i < sweapNum; i++)
        {
            var sim = (CPMSimurationBase)Activator.CreateInstance(type);

            if (sim == null || sim is not CPMSimurationBase) throw new Exception("Type is not CPMSimurationBase");

            cPMSimurationBases.Add(sim);
            sim.id = i;

            var task = Task.Run(() =>
            {
                sim.pathName += "_Sweep/" + sim.id;
                sim.PreInit();
                sim.Init();
                sweapAction(sim, sim.id);
                sim.Start();
                sim.End();
                sim.Final();
            });

            sims.Add(task);

            Console.WriteLine("Sweap" + i + " Start");
        }

        await Task.WhenAll(sims);
    }

    public void PreInit()
    {
        throw new NotImplementedException();
    }

    public void Init()
    {
        throw new NotImplementedException();
    }

    public Task Start()
    {
        throw new NotImplementedException();
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void Final()
    {
        throw new NotImplementedException();
    }
}
