using System.Security.Cryptography.X509Certificates;
using CPMBase.Base;
using CPMBase.CPM;
using Microsoft.AspNetCore.Mvc;

namespace CPMBase;

public class VolumeConstraint : BaseConstraint
{
    public VolumeConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    protected override float CullDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        areaCell.CullA(add: otherArea);
        otherAreaCell.CullA(remove: area);
        
        CullEnergy(areaCell, out float preArea, out float nowArea);
        CullEnergy(otherAreaCell, out float preOther, out float nowOther);

        

        return (nowArea + nowOther) - (preArea + preOther);
    }

    public void CullEnergy(Cell cell, out float pre, out float now)
    {
        if (cell is not EmptyCell)
        {
            pre = cell.kA * (cell.preA - cell.A0) * (cell.preA - cell.A0);
            now = cell.kA * (cell.A - cell.A0) * (cell.A - cell.A0);
        }
        else
        {
            pre = 0;
            now = 0;
        }
    }

}
