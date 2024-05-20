using System.Numerics;
using System.Runtime.CompilerServices;
using CPMBase.CPM;

namespace CPMBase;

public class SmallestSim : CPMSimurationBase
{
    public override RangePosition range { get => new RangePosition(20, 20, 1, false); }
    public override int end { get => 10; }
    public override int writeNum { get => 1000000; }
    public override Dimention dim { get => Dimention._2d; }

    public override Vector2 resolution => new Vector2(500, 500);

    public override void Add_Cell()
    {
        cPMAreaArray.Add(
                new Cell(r:2, 1, 1, maxact: 1, lact: 0, T: 5), //細胞のパラメータ
                new RangePosition(11, 9, 11, 9, 0, 0) //細胞の位置
            ); // 細胞を追加
        cPMAreaArray.Add(
            new Cell(r: 2, 1, 1, maxact: 1, lact: 0, T: 5), //細胞のパラメータ
            new RangePosition(13, 11, 11, 9, 0, 0) //細胞の位置
        ); // 細胞を追加

        updater.postUpdateFunc += () =>{cPMAreaArray.Test();};
    }
}
