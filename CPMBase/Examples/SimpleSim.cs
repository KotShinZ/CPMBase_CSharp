using System.Numerics;
using System.Runtime.CompilerServices;
using CPMBase.Base;
using CPMBase.CPM;

namespace CPMBase;

public class SimpleSim : CPMSimurationBase
{
    public override RangePosition range { get => new RangePosition(100, 100, 1, false); }
    public override int end { get => 100; }
    public override int writeNum { get => 100; }
    public override Dimention dim { get => Dimention._2d; }
    public override Vector2 resolution => new Vector2(500, 500);
    public override bool isOutputJson => true;

    public override bool isPlotMSD => true;

    public override int preSimulateTime => 10;




    public override void Add_Cell()
    {
        cPMAreaArray.Add(
                new Cell(r: 10, 5, 5, maxact: 105, lact: 20, T: 1), //細胞のパラメータ
                new RangePosition(55, 45, 55, 45, 0, 0) //細胞の位置
            ); // 細胞を追加

        //((StepUpdaterWithWrite)updater).OnWrite += s => { cPMAreaArray.Test(); };
    }
}
