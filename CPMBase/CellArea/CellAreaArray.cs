using System.Drawing;
using System.Numerics;
using CPMBase.Base;
using CPMBase.CPM;
using Newtonsoft.Json;

namespace CPMBase;

/// <summary>
/// 領域を3次元配列で保持。
/// 次元は変更できる
/// </summary>
public class CellAreaArray
{
    public Dimention dim;

    public CellArea[,,] cellAreas;

    public Vector3 dV = new Vector3(1, 1, 1);

    [JsonIgnore]
    public List<CellArea> cellAreaList = new List<CellArea>();

    public RangePosition size;

    /*public CellAreaArray(RangePosition range, CellArea cell = null, Dimention dim = Dimention._3d, Vector3 dV = default)
    {
        this.dim = dim;
        this.size = range;

        dV = dV == default ? dim.GetUnitVec() : dV;

        cellAreas = new CellArea[(int)size.arrayRange.Length.X, (int)size.arrayRange.Length.Y, (int)size.arrayRange.Length.Z]; //配列の大きさを設定
        arrayRange = size.arrayRange;

        if (cell != null)
        {
            AllFunc((x, y, z, c) =>
            {
                CellArea newArea = Utill.JsonSerializerClone(cell);
                newArea.parent = this;
                newArea.position = new Position(new Vector3(x, y, z), dV, size.realRange);

                SetCell(x, y, z, newArea);
            });
        }
    }*/

    public CellAreaArray(RangePosition range, Func<CellArea> func = null, Dimention dim = Dimention._3d)
    {
        this.dim = dim;
        this.size = range;

        cellAreas = new CellArea[(int)size.arrayRange.Length.X, (int)size.arrayRange.Length.Y, (int)size.arrayRange.Length.Z]; //配列の大きさを設定

        if (func != null)
        {
            AllFunc((x, y, z, c) =>
            {
                CellArea newArea = func != null ? func.Invoke() : new CellArea();
                newArea.parent = this;
                newArea.position = new Position(new Vector3(x, y, z));

                SetCell(x, y, z, newArea);
            });

            AllFunc((x, y, z, c) =>
            {
                c.SetInitNextAreas();
            });
        }


    }

    public void SetCell(int x, int y, int z, CellArea area)
    {
        cellAreas[x, y, z] = area;
        cellAreaList.Add(area);
    }
    public void SetCell(Vector3 arrayPosition, CellArea area)
    {
        SetCell((int)arrayPosition.X, (int)arrayPosition.Y, (int)arrayPosition.Z, area);
    }

    /// <summary>
    /// 角の位置からセルエリアを決めて、セルを設定する
    /// </summary>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="area"></param>
    public void SetCellArea(RangePosition range3, CellArea area)
    {
        var rangeInt = range3.arrayRange;
        for (int z = (int)rangeInt.z.min; z < rangeInt.z.max; z++)
        {
            for (int y = (int)rangeInt.y.min; y < rangeInt.y.max; y++)
            {
                for (int x = (int)rangeInt.x.min; x < rangeInt.x.max; x++)
                {
                    SetCell(x, y, z, Utill.JsonSerializerClone(area));
                }
            }
        }
    }

    /// <summary>
    /// 角の位置からセルエリアを決めて、セルを設定する
    /// </summary>
    /// <param name="corner1"></param>
    /// <param name="corner2"></param>
    /// <param name="area"></param>
    public void SetCellArea(RangePosition range3, Func<CellArea> area)
    {
        var rangeInt = range3.arrayRange;
        for (int z = (int)rangeInt.z.min; z < rangeInt.z.max; z++)
        {
            for (int y = (int)rangeInt.y.min; y < rangeInt.y.max; y++)
            {
                for (int x = (int)rangeInt.x.min; x < rangeInt.x.max; x++)
                {
                    SetCell(x, y, z, area.Invoke());
                }
            }
        }
    }

    public CellArea GetCellArea(int x, int y, int z)
    {
        return cellAreas[x, y, z];
    }

    public CellArea GetCellArea(Vector3 arrayPosition)
    {
        return cellAreas[(int)arrayPosition.X, (int)arrayPosition.Y, (int)arrayPosition.Z];
    }

    public CellArea GetCellArea(Position position)
    {
        return cellAreas[(int)position.arrayPosition.X, (int)position.arrayPosition.Y, (int)position.arrayPosition.Z];
    }

    /// <summary>
    /// 配列の全要素に対して処理を行う
    /// </summary>
    /// <param name="action"></param>
    public void AllFunc(Action<CellArea> action)
    {
        AllFunc((x, y, z, area) => action.Invoke(area));
    }

    public void AllFunc(Action<int, int, int, CellArea> action)
    {
        var _z = dim == Dimention._3d ? size.arrayRange.z.Length : 1;
        var _y = dim == Dimention._3d || dim == Dimention._2d ? size.arrayRange.y.Length : 1;
        for (int z = 0; z < _z; z++)
        {
            for (int y = 0; y < _y; y++)
            {
                for (int x = 0; x < size.arrayRange.x.Length; x++)
                {
                    action.Invoke(x, y, z, cellAreas[x, y, z]);
                }
            }
        }
    }

    public void AllFunc(Action<Position, CellArea> action)
    {
        AllFunc((x, y, z, area) => action.Invoke(new Position(new Vector3(x, y, z)), area));
    }


    /// <summary>
    ///  配列の範囲を指定して処理を行う
    /// </summary>
    /// <param name="range"></param>
    /// <param name="action"></param>
    public void AreaFunc(RangePosition range, Action<CellArea> action, int z2d = 0)
    {
        AreaFunc((int)range.arrayRange.x.max, (int)range.arrayRange.x.min, (int)range.arrayRange.y.max, (int)(range.arrayRange.y.min), (int)(range.arrayRange.z.max), (int)(range.arrayRange.z.min), action, z2d);
    }

    public void AreaFunc(RangePosition range, Action<int, int, int, CellArea> action, int z2d = 0)
    {
        AreaFunc((int)range.arrayRange.x.max, (int)range.arrayRange.x.min, (int)range.arrayRange.y.max, (int)(range.arrayRange.y.min), (int)(range.arrayRange.z.max), (int)(range.arrayRange.z.min), action, z2d);
    }

    public void AreaFunc(Range3<int> range, Action<CellArea> action)
    {
        AreaFunc(range.x.max, range.x.min, range.y.max, range.y.min, range.z.max, range.z.min, action);
    }

    public void AreaFunc(int maxX, int minX, int maxY, int minY, int maxZ, int minZ, Action<CellArea> action = null, int z2d = 0)
    {
        AreaFunc(maxX, minX, maxY, minY, maxZ, minZ, (x, y, z, area) => action.Invoke(area));
    }

    public void AreaFunc(int maxX, int minX, int maxY, int minY, int maxZ, int minZ, Action<int, int, int, CellArea> action = null, int z2d = 0)
    {
        //Console.WriteLine("maxX:" + maxX + " minX:" + minX + " maxY:" + maxY + " minY:" + minY + " maxZ:" + maxZ + " minZ:" + minZ);
        var _maxz = dim == Dimention._3d ? maxZ : z2d + 1;
        var _maxy = dim == Dimention._3d || dim == Dimention._2d ? maxY : 1;
        var _minz = dim == Dimention._3d ? minZ : z2d;
        var _miny = dim == Dimention._3d || dim == Dimention._2d ? minY : 0;

        for (int z = _minz; z < _maxz; z++)
        {
            for (int y = _miny; y < _maxy; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    action.Invoke(x, y, z, cellAreas[x, y, z]);
                }
            }
        }
    }

    public void AreaFunc(int max, int min, Action<CellArea> action)
    {
        AreaFunc(max, min, max, min, max, min, action);
    }

    public void AreaFunc(Vector3 center, int size, Action<CellArea> action)
    {
        AreaFunc((int)center.X + size, (int)center.X - size, (int)center.Y + size, (int)center.Y - size, (int)center.Z + size, (int)center.Z - size, action);
    }

    public void AreaFunc(Vector3 center, int sizeX, int sizeY, int sizeZ, Action<CellArea> action)
    {
        AreaFunc((int)center.X + sizeX, (int)center.X - sizeX, (int)center.Y + sizeY, (int)center.Y - sizeY, (int)center.Z + sizeZ, (int)center.Z - sizeZ, action);
    }



    /// <summary>
    ///  隣のエリアに対して処理を行う
    /// </summary>
    /// <param name="area">中心のエリア</param>
    /// <param name="action">行うアクション(trueを返すと終了する)</param>
    /// <param name="dim">次元</param>
    public void InitNextFunc(CellArea area, Func<CellArea, Vector3, bool> action, Dimention dim)
    {
        void Func(params Vector3[] vector3)
        {
            var array = vector3.OrderBy(a => Guid.NewGuid()).ToArray();

            foreach (var v in array)
            {
                var p = area.position.arrayPosition + v;

                try
                {
                    if (area.position.arrayPosition.X + v.X < 0 || area.position.arrayPosition.Y + v.Y < 0 || area.position.arrayPosition.Z + v.Z < 0) continue;
                    if (area.position.arrayPosition.X + v.X >= size.arrayRange.Length.X || area.position.arrayPosition.Y + v.Y >= size.arrayRange.Length.Y || area.position.arrayPosition.Z + v.Z >= size.arrayRange.Length.Z) continue;

                    var cell = GetCellArea(p);
                    var b = action.Invoke(cell, v);
                    if (b) return;
                }
                catch { }
            }
        }
        if (dim == Dimention._3d)
        {
            Func(new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1));
        }
        else if (dim == Dimention._2d)
        {
            Func(new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0));
        }
        else if (dim == Dimention._1d)
        {
            Func(new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0));
        }

    }

    public CellArea GetRandomCell()
    {
        //Console.WriteLine(cellAreaList[Randomizer.Next(0,cellAreaList.Count)]);
        return cellAreaList[Randomizer.Next(cellAreaList.Count)];
    }
}
