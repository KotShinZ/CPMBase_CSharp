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
using CPMBase.CPM;
using System.Net.Quic;

namespace CPMBase;

public class CPMAreaArray : CellAreaArray, ITimePathWrite
{
    public CPMAreaArray(RangePosition range, Dimention dim, Func<CPMArea> func = null) : base(range, func, dim)
    {
        this.dim = dim;
        this.size = range;

        cellAreas = new CellArea[(int)size.arrayRange.Length.X, (int)size.arrayRange.Length.Y, (int)size.arrayRange.Length.Z]; //配列の大きさを設定

        AllFunc((x, y, z, c) =>
        {
            CellArea newArea = func != null ? func.Invoke() : new CPMArea(dim);
            newArea.parent = this;
            newArea.position = new Position(new Vector3(x, y, z));

            SetCell(x, y, z, newArea);
        });

        AllFunc((x, y, z, c) =>
        {
            c.SetInitNextAreas();
        });
    }


    public List<Cell> cells = new List<Cell>(); //細胞のリスト
    private int cellNum => cells.Count; //現在の細胞の数
    //private int maxCellNum => cells.;//細胞の最大数

    public int writeResolusion = 512; //書き出す画像の解像度

    [JsonIgnore]
    public SKBitmap preBitmap;

    public LinePlotter linePlotter = new();

    #region Add_Cell

    /// <summary>
    /// 細胞を一つ追加する
    /// </summary>
    /// <param name="cell">細胞</param>
    /// <param name="center">追加する中心の場所</param>
    /// <param name="initSize">大きさ</param> <summary>
    /// 
    /// </summary>
    public void Add(Cell cell, RangePosition cellarea)
    {
        /* if (cellNum >= maxCellNum)
         {
             Console.WriteLine("細胞の数が最大数を超えたため追加できません");
             return; //細胞の数が最大数を超えたら追加しない
         }*/

        if (cells.Find(c => c == cell) == null)
        {
            cells.Add(cell);
            cell.id = cells.Count;
        }

        AreaFunc(cellarea, (c) =>
        {
            ((CPMArea)c).cell = cell;
        }); //細胞の領域に細胞を追加
    }

    public void Add(Cell cell, Position center, Position size = null)
    {
        var _size = size ?? new Position(10, 10, 1);
        Add(cell, new RangePosition(center.arrayPosition.X + _size.arrayPosition.X / 2, center.arrayPosition.X - _size.arrayPosition.X / 2, center.arrayPosition.Y + _size.arrayPosition.Y / 2, center.arrayPosition.Y - _size.arrayPosition.Y / 2, center.arrayPosition.Z + _size.arrayPosition.Z / 2, center.arrayPosition.Z - _size.arrayPosition.Z / 2));
    }

    public void Add(Cell cell, Vector3 center, Vector3 size)
    {
        Add(cell, new Position(center), new Position(size));
    }

    /// <summary>
    /// rangeの範囲のランダムな位置に細胞を追加する
    /// </summary>
    /// <param name="cell">細胞</param>
    /// <param name="range">追加する範囲</param>
    /// <param name="num">追加する数</param>
    public void AddArea(Func<Cell> cell, RangePosition range, int num, Position cellSize = null)
    {
        for (int i = 0; i < num; i++)
        {
            var point = range.GetRandomPosition();
            Add(cell.Invoke(), point, cellSize);
        }
    }

    public void AddCellsRect(Func<Cell> cell, Position cellSize, int num, Position center = null)
    {
        var _center = center ?? new Position(size.arrayRange.center);
        var sqrtNum = (int)MathF.Sqrt(num);
        var leftUp = new Position(_center.arrayPosition.X - cellSize.arrayPosition.X / 2 * MathF.Sqrt(num), _center.arrayPosition.Y + cellSize.arrayPosition.Y / 2 * MathF.Sqrt(num), 0);
        for (int i = 0; i < sqrtNum; i++)
        {
            for (int j = 0; j < sqrtNum; j++)
            {
                Add(cell.Invoke(), new Position(leftUp.arrayPosition.X + i * cellSize.arrayPosition.X, leftUp.arrayPosition.Y - j * cellSize.arrayPosition.Y, 0), cellSize);
            }
        }
    }

    public void AddCellsVolume(Func<Cell> cell, Position cellSize, int num, Position center = null)
    {
    }
    #endregion

    #region Get_Cell

    /// <summary>
    /// 隣の異なる細胞を一つランダムで取得
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="otherCell"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool GetNextToOtherCell(CPMArea cell, out CPMArea otherCell, out Direction direction)
    {
        List<CPMArea> list = new List<CPMArea>();
        List<Direction> directions = new List<Direction>();

        cell.NextFunc((other, dir) =>
        {
            var next = ((CPMArea)other).cell != cell.cell; //違う細胞が隣にあるかどうか
            if (next)
            {
                list.Add((CPMArea)other);
                directions.Add(dir);
            }
            return false;
        }, dim);

        if (list.Count > 0)
        {
            var num = Randomizer.Next(list.Count);
            otherCell = list[num];
            direction = directions[num];

            return true;
        }
        else
        {
            otherCell = null;
            direction = default;
            return false;
        }


    }

    /// <summary>
    /// セルとそのセルのエリアを取得
    /// </summary>
    /// <returns></returns>
    public Dictionary<Cell, List<CPMArea>> GetCellAreaList()
    {
        Dictionary<Cell, List<CPMArea>> dict = new();
        // 各細胞のエリアを整理
        AllFunc(c =>
            {
                if (dict.ContainsKey(((CPMArea)c).cell))
                {
                    dict[((CPMArea)c).cell].Add((CPMArea)c);
                }
                else
                {
                    dict[((CPMArea)c).cell] = new List<CPMArea>() { (CPMArea)c };
                }
            }
            );
        return dict;
    }

    /// <summary>
    /// ランダムに境界の細胞を取得
    /// </summary>
    /// <returns></returns>
    public CPMArea GetRandomNextCell()
    {
        int num = 0;
        while (true)
        {
            var area = (CPMArea)GetRandomCell();
            //Console.WriteLine(area == null);
            if (area.nextSame < 4)
            {
                //Console.WriteLine(area.IsNextToOtherCell());
                return area;

            }

            num++;
            if (num > 10000000)
            {
                Console.WriteLine("境界の細胞が見つかりませんでした");
                throw new Exception("境界の細胞が見つかりませんでした");
            }
        }
    }


    public Dictionary<Cell, Position> GetCellCenter()
    {
        Dictionary<Cell, Position> _out = new();
        Dictionary<Cell, List<Position>> dict = new();
        AllFunc((x, y, z, c) =>
        {
            if (dict.ContainsKey(((CPMArea)c).cell))
            {
                dict[((CPMArea)c).cell].Add(new Position(new Vector3(x, y, z)));
            }
            else
            {
                dict[((CPMArea)c).cell] = new List<Position>() { new Position(new Vector3(x, y, z)) };
            }
        });

        foreach (var key in dict.Keys)
        {
            Position sum = new Position(new Vector3(0, 0, 0));
            foreach (var pos in dict[key])
            {
                sum += pos;
            }
            if (dim != Dimention._3d)
            {
                sum.position.Z = 0;
                sum.arrayPosition.Z = 0;
            }
            var center = sum / new Position(dict[key].Count, dict[key].Count, dict[key].Count);
            _out.Add(key, center);
        }


        return _out;
    }

    #endregion

    #region Write_Cell

    /// <summary>
    /// セルを個別の色で2D描画
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="z"></param>
    public void WriteImage(SKBitmap bitmap, int z = 0)
    {
        // 各ピクセルに対してランダムな色を割り当て
        var range = size;

        AreaFunc(range, (x, y, z, c) =>
        {
            bitmap.SetPixel(x, (int)(range.arrayRange.y.Length - y - 1), ConvertToSKColor(((CPMArea)c).cell.color));
        }, z2d: z);
    }

    public void WriteImageWithAct(SKBitmap bitmap, int z = 0)
    {
        // 各ピクセルに対してランダムな色を割り当て
        var range = size;

        AreaFunc(range, (x, y, z, c) =>
        {
            var cellarea = ((CPMArea)c);
            SKColor color = cellarea.activity > 0 ? new SKColor((byte)(MathF.Min(cellarea.activity, 1) * 255), 0, 0) : ConvertToSKColor(((CPMArea)c).cell.color);
            bitmap.SetPixel(x, (int)(range.arrayRange.y.Length - y - 1), color);
        }, z2d: z);
    }

    /// <summary>
    /// CPMの画像をビットマップに書き込む(補完あり、解像度固定)
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="z"></param>
    public void WriteImageInterpolate(SKBitmap bitmap, int z = 0)
    {
        // 各ピクセルに対してランダムな色を割り当て
        var range = size;

        var height = bitmap.Height;
        var width = bitmap.Width;

        SKColor white = new SKColor(255, 255, 255);
        SKColor black = new SKColor(0, 0, 0);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var x2 = (int)(x * range.arrayRange.x.Length / width); //補完するための座標
                var y2 = (int)(y * range.arrayRange.y.Length / -height); //補完するための座標
                var cellarea = (CPMArea)GetCellArea(x2, y2, z);

                SKColor color = ConvertToSKColor(cellarea.cell.color);

                SKColor c = cellarea.isNextOther ? black : color;
                bitmap.SetPixel(x, y, c);
            }
        }
    }

    /// <summary>
    /// CPMの画像をビットマップに書き込む(補完あり、解像度固定)
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="z"></param>
    public void WriteImageInterpolateWithAct(SKBitmap bitmap, int z = 0)
    {
        // 各ピクセルに対してランダムな色を割り当て
        var range = size;

        var height = bitmap.Height;
        var width = bitmap.Width;

        SKColor white = new SKColor(255, 255, 255);
        SKColor black = new SKColor(0, 0, 0);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var x2 = (int)(x * range.arrayRange.x.Length / width); //補完するための座標
                var y2 = (int)(y * range.arrayRange.y.Length / height); //補完するための座標
                var cellarea = (CPMArea)GetCellArea(x2, y2, z);

                SKColor color = ConvertToSKColor(cellarea.cell.color);
                SKColor act = Leap(white, color, cellarea.activity);

                SKColor c = cellarea.nextSame < 4 && cellarea.cell is not EmptyCell ? black : act;
                //SKColor c2 = Leap(black, color, cellarea.nextSame / 4f);
                bitmap.SetPixel(x, height - 1 - y, cellarea.cell is not EmptyCell ? c : white);
            }
        }

        SKColor Leap(SKColor color1, SKColor color2, float t)
        {
            return new SKColor((byte)(color1.Red * (1 - t) + color2.Red * t), (byte)(color1.Green * (1 - t) + color2.Green * t), (byte)(color1.Blue * (1 - t) + color2.Blue * t));
        }
    }

    public SKColor ConvertToSKColor(Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    public void Write(float time, int step, PathObject path)
    {
        WriteSK(time, step, path, new Vector2(256, 256));
    }

    public void WriteSK(float time, int step, PathObject path, Vector2 resolution, Vector2 writeResolusion = default)
    {
        //var resolusion = writeResolusion == default ? new Vector2((float)range.arrayRange.x.Length, (float)range.arrayRange.y.Length) : writeResolusion;
        preBitmap ??= new SKBitmap((int)resolution.X, (int)resolution.Y);
        WriteImageInterpolateWithAct(preBitmap); // CPMの画像をビットマップに書き込む
        path.WriteToImgFile(preBitmap); //ビットマップを書き出す
    }

    public void WriteAsJson(PathObject path, string name = "JsonData")
    {
        path.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public void WriteImage(float time, int step, PathObject pathObject, Vector2 resolution)
    {
        WriteSK(time, step, pathObject, resolution);
    }

    /// <summary>
    /// MSDを記録する
    /// </summary>
    /// <param name="linePlotter"></param>
    /// <param name="time"></param>
    public void AddMSDData(double time)
    {
        var dict = GetCellCenter();
        var lengths = new List<float>();
        foreach (var key in dict.Keys)
        {
            if (key is EmptyCell) continue;

            lengths.Add((dict[key] - key.initPosition).position.Length());
        }

        linePlotter.AddData(time, lengths.Average());
    }

    #endregion

    #region Test

    public bool Test()
    {
        var ok = true;

        var data = GetCellAreaList();

        List<BaseCPMTest> tests = new List<BaseCPMTest>()
        {
            new VolumeTest(),
            new NextSameTest(),
            new PerimeterTest(),
        };

        foreach (var test in tests)
        {
            ok &= test.Test(data);
        }

        return ok;
    }



    #endregion

    public void AllCellFunc(Action<Cell> action)
    {
        for (int i = 0; i < cellNum; i++)
        {
            action(cells[i]);
        }
    }

    public void SetInitCenter()
    {
        var center = GetCellCenter();
        foreach (var area in center)
        {
            area.Key.initPosition = area.Value;
        }
    }

    /// <summary>
    /// 各細胞のエネルギーを一から計算
    /// </summary>
    public void CullInitVolume()
    {
        var dict = GetCellAreaList();

        foreach (var cell in dict.Keys)
        {
            int A = dict[cell].Count; //細胞の面積を計算

            cell.A = A;
            cell.A = A;
        }
    }

    /// <summary>
    /// 各細胞のエネルギーを一から計算
    /// </summary>
    public void CullInitPerimeter()
    {
        var dict = GetCellAreaList();

        foreach (var cell in dict.Keys)
        {
            int L = 0;
            foreach (var area in dict[cell])
            {
                //L += area.nextDiff; //隣接する異なる細胞の数を計算
                L += area.IsNextToOtherCell() ? 1 : 0;
            }
            cell.L = L;
            cell.L = L;
        }
    }
}
