using System.Numerics;
using CPMBase.CPM;

namespace CPMBase;

public class FourSim : CPMSimurationBase
{
    public override RangePosition range { get => new RangePosition(500, 500, 1, false); }
    public override int end { get => 10000; }
    public override int writeNum { get => 1000; }
    public override Dimention dim { get => Dimention._2d; }

    public override Vector2 resolution => new Vector2(500, 500);

    public override int preSimulateTime => 10;

    public override void Add_Cell()
    {
        cPMAreaArray.Add(
                new Cell(r: 10, 5, 5, maxact: 105, lact: 10, T: 1f), //細胞のパラメータ
                new RangePosition(130, 120, 130, 120, 0, 0) //細胞の位置
            ); // 細胞を追加

        cPMAreaArray.Add(
                new Cell(r: 10, 5, 5, maxact: 105, lact: 20, T: 1f), //細胞のパラメータ
                new RangePosition(380, 370, 130, 120, 0, 0) //細胞の位置
            ); // 細胞を追加

        cPMAreaArray.Add(
                new Cell(r: 10, 5, 5, maxact: 105, lact: 40, T: 1f), //細胞のパラメータ
                new RangePosition(130, 120, 380, 370, 0, 0) //細胞の位置
            ); // 細胞を追加

        cPMAreaArray.Add(
                new Cell(r: 10, 5, 5, maxact: 105, lact: 60, T: 1f), //細胞のパラメータ
                new RangePosition(380, 370, 380, 370, 0, 0) //細胞の位置
            ); // 細胞を追加
    }
}
