using System.Drawing;
using CPMBase.CPM;

namespace CPMBase;

public class EmptyCell : Cell
{
    public static EmptyCell emptyCell = new EmptyCell();

    public EmptyCell() : base(0, 0, 0, 0)
    {
    }

    public override float CullEnergy(CPMArea add = null, CPMArea remove = null)
    {
        return 0;
    }

    public override void SetRandomColor()
    {
        color = Color.White;
    }
}
