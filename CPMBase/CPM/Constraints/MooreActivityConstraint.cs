namespace CPMBase;

public class MooreActivityConstraint : ActivityConstraint
{
    public MooreActivityConstraint(CPMAreaArray cPMAreaArray) : base(cPMAreaArray)
    {
    }

    public override float GetNextActivity(CPMArea area)
    {
        float act = 0;
        int num = 1;
        area.MooreNextFunc((c, d) =>
        {
            if (((CPMArea)c).cell == area.cell)
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
