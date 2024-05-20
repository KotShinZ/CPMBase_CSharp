using CPMBase.CPM;

namespace CPMBase;

public class PerimeterTest: BaseCPMTest
{
    //public override bool isPrint => false;

    public override void GetTest(Dictionary<Cell, List<CPMArea>> data, out List<float> trueValue, out List<float> realValue)
    {
        trueValue = new List<float>();
        realValue = new List<float>();

        foreach (var cell in data.Keys)
        {
            realValue.Add(cell.L);
            var L = 0;
            foreach (var area in data[cell])
            {
                L += area.IsNextToOtherCell() ? 1 : 0;
            }
            trueValue.Add(L);
        }
    }
}
