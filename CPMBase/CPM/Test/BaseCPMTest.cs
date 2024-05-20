using CPMBase.Base;
using CPMBase.CPM;

namespace CPMBase;

public abstract class BaseCPMTest
{
    public virtual bool isPrint => true;

    public virtual bool Test(Dictionary<Cell, List<CPMArea>> data){
        GetTest(data, out List<float> trueValue, out List<float> realValue);
       if(isPrint) Error(trueValue, realValue);
        return trueValue == realValue;
    }

    public abstract void GetTest(Dictionary<Cell, List<CPMArea>> data, out List<float> trueValue, out List<float> realValue);

    public virtual void Error(List<float> trueValue, List<float> realValue){
        //Console.WriteLine(StepUpdater.instance.stepNum);
        var ok = true;
        for(var n = 0; n < trueValue.Count; n++)
        {
            if (trueValue[n] != realValue[n]){
                Console.WriteLine(this.GetType().Name + " : Error: " + trueValue[n] + " != " + realValue[n]);
                ok = false;
            }
        }
        if(ok)Console.WriteLine(this.GetType().Name + " : Success: " + trueValue);   
    }
}
