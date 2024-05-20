using CPMBase.Base;
using CPMBase.CPM;

namespace CPMBase;

public class PerimeterConstraint : BaseConstraint
{
    public PerimeterConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    protected override float CullDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        areaCell.CullL_nums(area, otherArea, true);
        otherAreaCell.CullL_nums(area, otherArea, false);

        /*var list = cPMAreaArray.GetCellAreaList();
        foreach (var cell in list)
        {
            var _L = 0;
            foreach (var _area in cell.Value)
            {
                _L += area.IsNextToOtherCell() ? 1 : 0;
            }
            cell.Key.L = _L;
        }*/

        CullEnergy(areaCell, out float preArea, out float nowArea);
        CullEnergy(otherAreaCell, out float preOther, out float nowOther);
        //Console.WriteLine(areaCell.L + " " + otherAreaCell.L + " " + (nowArea + nowOther) + " " + (preArea + preOther));

        return (nowArea + nowOther) - (preArea + preOther);
    }

    public void CullEnergy(Cell cell, out float pre, out float now)
    {
        if (cell is not EmptyCell)
        {
            pre = cell.kL * (cell.preL - cell.L0) * (cell.preL - cell.L0);
            now = cell.kL * (cell.L - cell.L0) * (cell.L - cell.L0);
        }
        else
        {
            pre = 0;
            now = 0;
        }
    }

}
