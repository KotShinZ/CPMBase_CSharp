using CPMBase.CPM;

namespace CPMBase;

public class GetCellTest : SpeadTester
{
    /*CPM_Base cpm = new CPM_Base(new RangePosition(100, 100, 1, false), dim: Dimention._2d);

    public GetCellTest(int num) : base(num)
    {
        cpm.AddCellsRect(
            () => new Cell(r: 30, 1, 1, maxact: 100, lact: 100), //細胞のパラメータ\
            new Position(10, 10, 0), //細胞の大きさ
            25
        ); // 細胞を追加
        cpm.Init(new Base.StepUpdater(1,100));
    }

    public override void Test()
    {
        var c = cpm.GetRandomNextCell();
    }*/
    public GetCellTest(int num) : base(num)
    {
    }
}