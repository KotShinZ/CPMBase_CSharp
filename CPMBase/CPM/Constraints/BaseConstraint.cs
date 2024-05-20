using CPMBase.Base;
using CPMBase.CPM;

namespace CPMBase;

public abstract class BaseConstraint
{
    public CPMAreaArray cPMAreaArray;

    public Cell areaCell;
    public Cell otherAreaCell;

    public bool print = true;

    public bool isCullAverage = false;

    public float sum = 0;
    public float average => sum / averageCount;
    public float averageCount = 0;

    public BaseConstraint(CPMAreaArray cPMAreaArray)
    {
        this.cPMAreaArray = cPMAreaArray;
    }

    public float GetDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        areaCell = area.cell;
        otherAreaCell = otherArea.cell;

        var dh = CullDH(area, otherArea, direction);

        if (print) Print(dh);
        if (isCullAverage) CullAverage(dh);

        return dh;
    }

    protected abstract float CullDH(CPMArea area, CPMArea otherArea, Direction direction);

    public void Print(float dh)
    {
        //if (dh != 0) Console.WriteLine(this.GetType().Name + " : " + dh);
    }

    public void CullAverage(float dh)
    {
        sum += dh;
        averageCount++;
    }

    public void PrintAverage()
    {
        Console.WriteLine(this.GetType().Name + " --- Average :  " + average);
    }
}
