using CPMBase.Base;

namespace CPMBase;

public class AdhesionConstraint : BaseConstraint
{
    public AdhesionConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    protected override float CullDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        if (areaCell.kAdhesion != 0 && otherAreaCell.kAdhesion != 0)
            return areaCell.CullAdhesionFactor(area) + areaCell.CullAdhesionFactor(otherArea); //細胞のエネルギー差に接着因子を加える   0 ~ +3
        return 0;
    }
}
