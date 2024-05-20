namespace CPMBase;

public abstract class CPMSelectorBase
{
    protected CPMAreaArray area;
    public bool enabled = false;

    public CPMSelectorBase(CPMAreaArray area)
    {
        this.area = area;
        enabled = true;
    }

    public abstract CPMArea Select();
}
