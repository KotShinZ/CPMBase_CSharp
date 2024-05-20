using System.ComponentModel.DataAnnotations;
using System.Numerics;
using CPMBase.Base;
using CPMBase.CPM;
using KGySoft.CoreLibraries;
using Newtonsoft.Json;

namespace CPMBase;

[Serializable]
public class CPMArea : CellArea
{
    public int cellId => cell.id; //細胞のID

    [JsonIgnore]
    public Cell cell; //このエリアに含まれる細胞

    public int nextSame
    {
        get => _nextSame;
        set
        {
            preNextSame = _nextSame;
            _nextSame = value;
        }
    }
    private int _nextSame; //隣接する同じ細胞の数
    public int preNextSame; //前の隣接する同じ細胞の数

    [JsonIgnore]
    public int nextDiff => ((int)dim) * 2 - nextSame; //隣接する異なる細胞の数

    public Dimention dim;

    private static int[] sobelArray2D = new int[] { 1, 2, 1, 0, 0, 0, -1, -2, -1 };

    private static int[] sobelArray3D = new int[] { 1, 2, 1, 2, 4, 2, 1, 2, 1, 0, 0, 0, 0, 0, 0, -1, -2, -1, -2, -4, -2, -1, -2, -1, -1, -2, -1, -2, -4, -2, -1, -2, -1, 0, 0, 0, 0, 0, 0, 1, 2, 1, 2, 4, 2, 1, 2, 1 };

    public float activity
    {
        get { return _activity; } //1秒(全セル探索)で 1 / maxact 分だけ減少
        private set
        {
            //CPM_Base.sumActivity += value - _activity; //合計を更新
            _activity = MathF.Max(value, minActivity);
        }
        /*get { return MathF.Max( _activity - (float)(StepUpdater.instance.time - activityTime) * (1 / cell.maxact), 0); } //1秒(全セル探索)で 1 / maxact 分だけ減少
        private set
        {
            activityTime = StepUpdater.instance.time;
            CPM_Base.sumActivity += value - _activity; //合計を更新
            _activity = MathF.Max(value, minActivity);
        }*/
    }
    float _activity;
    // double activityTime = 0;
    float minActivity = 0.01f;

    public bool isNextOther = false; //隣が異なる細胞か

    [JsonIgnore]
    public CPMBase.Base.Datas.LinkedListNode<CPMArea> node;

    public CPMArea(Dimention dim, Position position = null, CellAreaArray cellAreaArray = null, Cell cell = null) : base(position, cellAreaArray)
    {
        this.dim = dim;
        this.cell = cell ?? EmptyCell.emptyCell;
        activity = minActivity;
        node = new CPMBase.Base.Datas.LinkedListNode<CPMArea>(this);
    }

    /// <summary>
    /// 活動量の更新
    /// </summary>
    public void UpdateActivity(float mul = 1)
    {
        //Console.WriteLine(1 / (cell.maxact * mul));
        activity = MathF.Max(0, MathF.Min(1, activity - 1 / cell.maxact));
    }

    public void SetActivityToOne()
    {
        activity = 1;
    }

    /// <summary>
    /// 隣と自分の活動量の平均を返す
    /// </summary>
    /// <returns></returns>
    public float GetNextActivity()
    {
        float act = 0;
        int num = 1;
        NextFunc((c, d) =>
        {
            if (c.cell == cell)
            {
                act += ((CPMArea)c).activity;
                num++;
            }
            return false;
        }, dim);
        act += activity;
        return act / num;
    }

    /// <summary>
    ///  前の細胞の情報を削除し、このエリアに細胞をセットする
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetArea"></param>
    public void SwapCell(Cell target, CPMArea targetArea) //このセルは浸食先, targetは浸食元
    {
        if (this.cell == target) return;

        var prevCell = this.cell; //2番
        this.cell = target; //1番

        this.cell.OnSwaped(targetArea);
        target.OnSwaped(this);

        nextSame = 0;

        NextFunc((c, d) =>
        {
            if (c.cell == cell)
            {
                c.nextSame += 1;
                nextSame += 1;
            }
            else if (c.cell == prevCell) c.nextSame -= 1;
            //c.CullNextSame();
            return false;
        }, dim);

        //cell.areas.Add(this); //細胞にエリアを登録
        //prevCell.areas.Remove(this); //前の細胞からエリアを削除
    }

    /// <summary>
    /// 隣の同じ細胞をカウントする  
    /// </summary>
    public int CullNextSame(Cell _cell)
    {
        var _nextSame = 0;
        NextFunc((c, d) =>
        {
            if (c.cell == _cell)
            {
                _nextSame++;
            }
            return false;
        }, dim);
        //CullNextSovel(); //ソーベルフィルターで隣接する細胞をカウント
        return _nextSame;
    }

    public void CullNextSame()
    {
        nextSame = CullNextSame(cell);
    }

    /// <summary>
    /// ソーベルフィルターで隣接する細胞をカウント
    /// </summary>
    public void CullNextSovel()
    {
        var sobelArray = dim == Dimention._2d ? sobelArray2D : sobelArray3D;
        var array = parent.cellAreas;
        nextSame = 0;
        parent.AreaFunc(position.arrayPosition, 1, (c) =>
        {
            nextSame += sobelArray[0] * (((CPMArea)c).cell == cell ? 1 : 0);
        });
    }

    /// <summary>
    ///  隣の細胞に対して関数を実行する
    /// </summary>
    /// <param name="action"></param>
    /// <param name="dim"></param>
    public void NextFunc(Func<CPMArea, Vector3, bool> action, Dimention dim)
    {
        base.NextFunc(
            (c, v) => { return action((CPMArea)c, DirectionHelper.GetVector(v)); },
            dim);
    }

    /// <summary>
    /// 隣に違う細胞があるかどうか
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public virtual bool IsNextToOtherCell()
    {
        bool b = false;

        NextFunc((other, dir) =>
        {
            var next = ((CPMArea)other).cell != cell; //違う細胞が隣にあるかどうか
            if (next)
            {
                b |= true;
                return true;
            }
            else return false;
        }, dim);

        return b;
    }

    /// <summary>
    /// 指定の細胞と隣り合っているかどうか
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public virtual bool IsNextToCell(Cell _cell)
    {
        bool b = false;

        NextFunc((other, dir) =>
        {
            var next = ((CPMArea)other).cell == _cell; //違う細胞が隣にあるかどうか
            if (next)
            {
                b |= true;
                return true;
            }
            else return false;
        }, dim);

        return b;
    }




}
