using CPMBase.Base;

namespace CPMBase;

public class ActivityConstraint : BaseConstraint
{

    public ActivityConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    protected override float CullDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        if (areaCell.lact != 0)
            return -1 * areaCell.lact * (GetNextActivity(area) - GetNextActivity(otherArea)); // (0 ~ 1)*-lact

        return 0;
    }

    /// <summary>
    /// 隣と自分の活動量の平均を返す
    /// </summary>
    /// <returns></returns>
    public virtual float GetNextActivity(CPMArea area)
    {
        float act = 0;
        int num = 1;
        area.NextFunc((c, d) =>
        {
            if (c.cell == area.cell)
            {
                act += ((CPMArea)c).activity;
                num++;
            }
            return false;
        }, cPMAreaArray.dim);
        act += area.activity;

        return act / num;
    }

}
