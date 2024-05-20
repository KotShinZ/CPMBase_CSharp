using CPMBase.Base;
using CPMBase.CPM;

namespace CPMBase;

public class NextSameTest : BaseCPMTest
{
    public override bool isPrint => false;

    public override void GetTest(Dictionary<Cell, List<CPMArea>> data, out List<float> trueValue, out List<float> realValue)
    {
        var _trueValue = new List<float>();
        var _realValue = new List<float>();

        var cPMAreaArray = (CPMAreaArray)data.Values.ElementAt(0)[0].parent;
        //Console.WriteLine(StepUpdater.instance.stepNum);

        cPMAreaArray.AllFunc(c =>
        {
            _realValue.Add(((CPMArea)c).nextSame);
            var v = ((CPMArea)c).CullNextSame(((CPMArea)c).cell);
            _trueValue.Add(v);
            if (_realValue.Last() != _trueValue.Last())
            {
                Console.WriteLine(c.position.arrayPosition);
                Console.WriteLine(((CPMArea)c).GetType());
            }
        });

        trueValue = _trueValue;
        realValue = _realValue;
    }
}
