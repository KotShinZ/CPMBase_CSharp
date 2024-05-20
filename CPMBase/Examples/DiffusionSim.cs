using System.Numerics;
using CPMBase.CPM;

namespace CPMBase;

/// <summary>
/// 中心からの拡散を再現したい
/// </summary>
public class DiffusionSim : CPMSimurationBase
{
    public override RangePosition range { get => new RangePosition(750, 750, 1, false); }
    public override int end { get => 20000; }
    public override int writeNum
    {
        get => 1000;
    }
    public override Dimention dim { get => Dimention._2d; }

    public override string pathName => base.pathName;

    public override bool isPlotMSD => true;

    public override bool isOutputJson => true;

    public override int preSimulateTime => 200;

    //public override Vector2 resolution => new Vector2(500, 500);

    public override void Add_Cell()
    {
        cPMAreaArray.AddCellsRect(
                () => new Cell(r: 10, 5, 5, maxact: 105, lact: 20, T: 1f, kAdhesion: 0
                ), //細胞のパラメータ
                new Position(8, 8, 0), //細胞の大きさ

                100 // 細胞
            );
        //((StepUpdaterWithWrite)updater).OnWrite += s => { cPMAreaArray.Test(); };
    }
}
