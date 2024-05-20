using CPMBase.CPM;

namespace CPMBase;

public class VolumeTest : BaseCPMTest
{
    public override bool isPrint => false;

    public override void GetTest(Dictionary<Cell, List<CPMArea>> data, out List<float> trueValue, out List<float> realValue)
    {
        trueValue = new List<float>();
        realValue = new List<float>();

        foreach (var cell in data.Keys)
        {
            trueValue.Add(data[cell].Count);
            realValue.Add(cell.A);
        }
    }
}
