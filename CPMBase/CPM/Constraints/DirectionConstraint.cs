using System.Numerics;
using CPMBase.Base;

namespace CPMBase;

public class DirectionConstraint : BaseConstraint
{
    public DirectionConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    protected override float CullDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        if (areaCell.constantPower != Vector3.Zero)
            return Vector3.Dot(areaCell.constantPower * -1, DirectionHelper.GetVector(direction)); //定数の力を加える
        return 0;
    }
}
