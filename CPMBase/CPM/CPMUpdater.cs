using System.Data.Common;
using System.Drawing;
using System.IO.Compression;
using System.Numerics;
using CPMBase.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;

using LinkedList = CPMBase.Base.Datas.LinkedList<CPMBase.CPMArea>;
using KGySoft.CoreLibraries;

namespace CPMBase;

public class CPMUpdater : IUpdatable, ITimePathWrite
{
    public CPMAreaArray cellAreaArray;

    public float T = 23; //ボルツマン温度

    public LinkedList areasSortedActivty = new LinkedList();

    public static float sumAct;

    public List<BaseConstraint> constraints = new List<BaseConstraint>();

    CPMBase.Base.Datas.LinkedListNode<CPMArea> targetNode = new CPMBase.Base.Datas.LinkedListNode<CPMArea>(null);

    public CPMUpdater(CPMAreaArray cPMAreaArray)
    {
        cellAreaArray = cPMAreaArray;

        constraints = new List<BaseConstraint>
        {
            new VolumeConstraint(cellAreaArray),
            new PerimeterConstraint(cellAreaArray),
            new AdhesionConstraint(cellAreaArray),
            new DirectionConstraint(cellAreaArray),
            new MooreActivityConstraint(cellAreaArray)
        };
    }

    public void Init(StepUpdater updater)
    {
        cellAreaArray.AllFunc(c => ((CPMArea)c).CullNextSame());  //nextSameを計算
        cellAreaArray.CullInitVolume(); //各細胞の面積を計算
        cellAreaArray.CullInitPerimeter(); //各細胞の周長を計算

        SetNextOther(); //隣り合っているかを確認、リストに追加

        areasSortedActivty.AddFirst(targetNode); //最初のノードを追加

        cellAreaArray.SetInitCenter(); //中心を計算


        void SetNextOther()
        {
            cellAreaArray.AllFunc(c =>
            {
                CPMArea area = (CPMArea)c;
                area.isNextOther = area.IsNextToOtherCell();
                if (area.isNextOther)
                {
                    areasSortedActivty.AddFirst(area.node);
                }
            });
            areasSortedActivty.Shuffle();
        }
    }

    public void Update(StepUpdater updater)
    {
        updater.dt = 1 / (double)areasSortedActivty.Count;
        //dt = 1 / (double)areasSortedActivty.Count;
        GetAreaAndOther(out CPMArea area, out CPMArea otherArea, out Direction direction);
        Update(area, otherArea, direction); //細胞を更新

        if (updater.timeNext == false) cellAreaArray.AllFunc(c => ((CPMArea)c).UpdateActivity());  //活動量を更新
    }

    public void End(StepUpdater updater)
    {
        foreach (var cons in constraints)
        {
            if (cons.isCullAverage) cons.PrintAverage();
        }
    }

    /// <summary>
    ///  Actの累積分布より、ランダムに細胞を選択
    /// </summary>
    /// <param name="stepUpdater"></param>
    /// <returns></returns> <summary>
    public CPMArea GetRandomCellWithActivty(StepUpdater stepUpdater)
    {
        //return (CPMArea)cellAreaArray.GetRandomCell();
        float sum = 0;
        var rand = Randomizer.NextFloat() * sumAct;
        foreach (var area in areasSortedActivty)
        {
            sum += area.activity;
            if (sum >= rand) return area;
        }
        return null;
    }

    /// <summary>
    /// 境界の細胞だけが入っている、LinkedListから次の細胞を取得
    /// </summary>
    public CPMArea GetFromNextList()
    {
        //Console.WriteLine("=================================================================================");
        areasSortedActivty.Swap(targetNode, targetNode.Next);

        //DebugNode(targetNode.Last);
        //DebugNode(targetNode);
        //DebugNode(targetNode.Next);

        var area = targetNode.Last.Value;//targetNode.Last.Value;
                                         //Console.WriteLine(area.position.position);

        if (area == null)
        {
            Console.WriteLine(areasSortedActivty.Tail.Value.position.position);
            throw new Exception("メッセージ");
        }

        if (targetNode.Next == null)
        {
            areasSortedActivty.Remove(targetNode);
            areasSortedActivty.Insert(areasSortedActivty.Head, targetNode);
        }

        return area;
    }

    public void GetAreaAndOther(out CPMArea area, out CPMArea otherArea, out Direction direction)
    {
        var n = 0;
        while (true)
        {
            var randomArea = cellAreaArray.GetRandomCell(); //1秒？
            if (cellAreaArray.GetNextToOtherCell((CPMArea)randomArea, out otherArea, out direction))
            {
                if (otherArea.cell.A <= 1) continue;
                area = (CPMArea)randomArea;
                return;
            }
            n++;
            if (n > 10000000) throw new Exception("エリアを取得できませんでした。");
        }
    }

    void DebugNode(CPMBase.Base.Datas.LinkedListNode<CPMArea> node)
    {
        Console.WriteLine("================================");
        if (node == null) { Console.WriteLine("nullNode"); return; }
        if (node.Value == null) Console.WriteLine("nullValue");
        else Console.WriteLine(((CPMArea)node.Value).position.position);
        if (node.Next == node.Last) Console.WriteLine("Next == Last");

        Console.WriteLine(node.Last == null ? "Last :  Null" : "Last :  " + (node.Last.Value != null ? ((CPMArea)node.Last.Value).position.position : "nullValue"));
        Console.WriteLine(node.Next == null ? "Next :  Null" : "Next :  " + (node.Next.Value != null ? ((CPMArea)node.Next.Value).position.position : "nullValue"));
    }

    /// <summary>
    /// 一つの細胞を更新
    /// </summary>
    public void Update(CPMArea area, CPMArea otherArea, Direction direction)
    {
        var dH = GetDH(area, otherArea, direction); //エネルギー差を計算
        float t = area.cell is not EmptyCell ? area.cell.T : otherArea.cell.T; //温度

        var p = Math.Exp(-dH / t); //ボルツマン因子

        if (dH < 0 || Randomizer.NextFloat() < p) //エネルギー差が負の場合
        {
            otherArea.SwapCell(area.cell, area); //細胞を入れ替える
            OnSwap(area, otherArea); //細胞を入れ替えた後の処理
        }
        else
        {
            //細胞を入れ替えない
            area.cell.BackPreSwap(); //細胞を入れ替える前の状態に戻す
            otherArea.cell.BackPreSwap(); //細胞を入れ替える前の状態に戻す
        }
        //Console.WriteLine("L : "+cellAreaArray.cells[0].L);
    }


    /// <summary>
    /// 細胞を入れ替えた後の処理
    /// </summary>
    /// <param name="area"></param>
    /// <param name="otherArea"></param> <summary>
    /// </summary>
    void OnSwap(CPMArea area, CPMArea otherArea)
    {
        if (area.cell is not EmptyCell)
        {
            otherArea.SetActivityToOne(); //活動量を1にする				
        }

        /*otherArea.NextFunc((c, d) =>
        {
            UpdateNextOther((CPMArea)c);
            return false;
        }, cellAreaArray.dim); //nextSameを計算
        UpdateNextOther(otherArea);*/
    }

    /// <summary>
    /// エリアが境界にあるかという情報を更新
    /// </summary>
    /// <param name="_area"></param>
    void UpdateNextOther(CPMArea _area)
    {
        var next = _area.IsNextToOtherCell();

        if (next && !_area.isNextOther)
        {
            areasSortedActivty.AddFirst(_area.node);
        }
        else if (!next && _area.isNextOther)
        {
            areasSortedActivty.Remove(_area.node);
        }
        _area.isNextOther = next;
    }

    /// <summary>
    /// dHを計算
    /// </summary>
    /// <param name="area"></param>
    /// <param name="otherArea"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public float GetDH(CPMArea area, CPMArea otherArea, Direction direction)
    {
        var dH = 0f;
        foreach (var constraint in constraints)
        {
            dH += constraint.GetDH(area, otherArea, direction);
        }
        /*
            var cell = area.cell; //細胞 (浸食する側)
            var otherCell = otherArea.cell; //隣の細胞　(浸食される側）

            var pre = cell.nowEnergy + otherCell.nowEnergy; //変更前の細胞の合計のエネルギー(sum)

            var now = cell.CullEnergy(add: otherArea) + otherCell.CullEnergy(remove: otherArea); //変更後の細胞の合計のエネルギー(sum)

            var dH = now - pre; //細胞のエネルギー差 面積は-1 ~ +1, 周長は-3 ~ +3

            //Console.WriteLine(dH);

            if (cell.kAdhesion != 0)
                dH += cell.CullAdhesionFactor((CPMArea)area); //細胞のエネルギー差に接着因子を加える   0 ~ +3

            if (cell.constantPower != Vector3.Zero)
                dH += Vector3.Dot(cell.constantPower * -1, DirectionHelper.GetVector(direction)); //定数の力を加える

            dH += -1 * cell.lact * (area.GetNextActivity() - otherArea.GetNextActivity()); // (0 ~ 1)*-lact
    */
        /*Console.WriteLine("pre : " + pre);
        Console.WriteLine("now : " + now);
        Console.WriteLine("A : " + cell.A + "  L : " + cell.L + "   otherA : " + otherCell.A + "  otherL : " + otherCell.L);
        Console.WriteLine("preA : " + cell.preA + "  preL : " + cell.preL + "   preotherA : " + otherCell.preA + "  preotherL : " + otherCell.preL);
        Console.WriteLine("\n");*/

        return dH;
    }

    void PrintDH(
        CPMArea area,
        CPMArea otherArea,
        Direction direction,
        float dH
    )
    {
        var cell = area.cell; //細胞 (浸食する側)
        var otherCell = otherArea.cell; //隣の細胞　(浸食される側）

        var da = cell.kA * (cell.A - cell.A0); //1固定
        //var dl = L - L0; //-3 ~ 3
        //return kA * da * da + kL * dl * dl; //-2 ~ 4

        Console.WriteLine("area : " + area.position.position);
        Console.WriteLine("otherArea : " + otherArea.position.position);
        Console.WriteLine("direction : " + direction);
        Console.WriteLine("dH : " + dH);
    }

    public void Write(float time, int step, PathObject pathObject)
    {
        //cellAreaArray.Write(time, step, pathObject);
    }

    public void WriteImage(float time, int step, PathObject pathObject, Vector2 resolution)
    {
        cellAreaArray.WriteImage(time, step, pathObject, resolution);
    }

    public void WriteAsJson(PathObject path, string name = "JsonData")
    {
        path.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
    }


}
