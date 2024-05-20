using CPMBase.Base;
using Newtonsoft.Json;

namespace CPMBase;

public class StepContinuer
{
    public StepUpdater stepUpdater;

    public StepContinuer()
    {
    }

    public StepContinuer(StepUpdater stepUpdater)
    {
        this.stepUpdater = stepUpdater;
    }

    public void Constinue<T>(string jsonPath, int endTime) where T : IUpdatable
    {
        Constinue<T>(new List<string> { jsonPath }, endTime);
    }

    public void Constinue<T>(List<string> jsonPath, int endTime) where T : IUpdatable
    {
        stepUpdater.endTime = endTime;
        jsonPath.ForEach(path =>
        {
            var updatable = JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(path));
            if (updatable == null)
            {
                Console.Error.WriteLine("Error: " + path + " is not found.");
                return;
            }
            stepUpdater.Add(updatable);
        });

        stepUpdater.Start();
    }
}
