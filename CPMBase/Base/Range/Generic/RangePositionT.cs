namespace CPMBase;
using System.Numerics;

public class RangePosition<T> where T : INumber<T>
{
    public Range3<T> arrayRange;
    public Range3<T> realRange;

    public Vector<T> dV;

    public T arrayVolume;
    public T realVolume;

    public RangePosition(Range3<T> arrayRange, Range3<T> realRange)
    {
        this.arrayRange = arrayRange;
        this.realRange = realRange;
        this.dV = new Vector<T>(new T[] {
            realRange.x.Length / arrayRange.x.Length,
            realRange.y.Length / arrayRange.y.Length,
            realRange.z.Length / arrayRange.z.Length
        });
        Init();
    }

    public RangePosition(Range3<T> range, Vector<T> dV, bool isArrayOrReal)
    {
        if (isArrayOrReal)
        {
            this.arrayRange = range;
            this.realRange = range * dV;
            this.dV = dV;
        }
        else
        {
            this.realRange = range;
            this.arrayRange = new Range3<T>(
                range.x.min / dV[0],
                range.x.max / dV[0],
                range.y.min / dV[1],
                range.y.max / dV[1],
                range.z.min / dV[2],
                range.z.max / dV[2]
            );
            this.dV = dV;
        }
    }

    public RangePosition(T arrayMaxX, T arrayMaxY, T arrayMaxZ, bool isCenter = false)
    {
        this.arrayRange = new Range3<T>(arrayMaxX, arrayMaxY, arrayMaxZ, isCenter);
        this.realRange = new Range3<T>(arrayMaxX, arrayMaxY, arrayMaxZ, isCenter);
        Init();
    }

    public RangePosition(T arrayMaxX, T arrayMinX, T arrayMaxY, T arrayMinY, T arrayMaxZ, T arrayMinZ)
    {
        this.arrayRange = new Range3<T>(arrayMaxX, arrayMinX, arrayMaxY, arrayMinY, arrayMaxZ, arrayMinZ);
        this.realRange = new Range3<T>(arrayMaxX, arrayMinX, arrayMaxY, arrayMinY, arrayMaxZ, arrayMinZ);
        Init();
    }

    public RangePosition(Vector<T> arrayRange, Vector<T> realRange, bool isCenter)
    {
        if (isCenter)
        {
            this.arrayRange = new Range3<T>(arrayRange);
            this.realRange = new Range3<T>(arrayRange * realRange - arrayRange * realRange / T.CreateChecked(2));
            this.dV = new Vector<T>(new T[] {
                    realRange.GetElement(0) / arrayRange.GetElement(0),
                    realRange.GetElement(1) / arrayRange.GetElement(1),
                    realRange.GetElement(2) / arrayRange.GetElement(2)
                });
        }
        else
        {
            this.arrayRange = new Range3<T>(arrayRange);
            this.realRange = new Range3<T>(realRange);
            this.dV = new Vector<T>(new T[] {
                    realRange.GetElement(0) / arrayRange.GetElement(0),
                    realRange.GetElement(1) / arrayRange.GetElement(1),
                    realRange.GetElement(2) / arrayRange.GetElement(2)
                });
        }
        Init();
    }

    /// <summary>
    ///  配列の大きさを指定して作成
    ///  配列の大きさと実際の大きさが同じ
    ///  実際の大きさは1要素の1辺が1m
    /// </summary>
    /// <param name="arrayRange"></param>
    public RangePosition(Vector<T> arrayRange) : this(arrayRange, arrayRange, Vector<T>.One) { }

    public RangePosition(Vector<T> arrayRange, Vector<T> realRange, Vector<T> dV)
    {
        this.arrayRange = new Range3<T>(arrayRange);
        this.realRange = new Range3<T>(realRange);
        this.dV = dV;
        Init();
    }

    public virtual void Init()
    {
        arrayVolume = arrayRange.x.Length * arrayRange.y.Length * arrayRange.z.Length;
        realVolume = realRange.x.Length * realRange.y.Length * realRange.z.Length;
    }

    public RangePosition<R> Cast<R>() where R : INumber<R>
    {
        return new RangePosition<R>(arrayRange.Cast<R>(), realRange.Cast<R>());
    }

    public RangePosition<T> AddArray(Vector<T> value)
    {
        arrayRange.x.Add(value.GetElement(0));
        arrayRange.y.Add(value.GetElement(1));
        arrayRange.z.Add(value.GetElement(2));
        realRange.x.Add(value.GetElement(0) * dV[0]);
        realRange.y.Add(value.GetElement(1) * dV[1]);
        realRange.z.Add(value.GetElement(2) * dV[2]);

        return this;
    }

    public RangePosition<T> AddReal(Vector<T> value)
    {
        realRange.x.Add(value.GetElement(0));
        realRange.y.Add(value.GetElement(1));
        realRange.z.Add(value.GetElement(2));
        arrayRange.x.Add(value.GetElement(0) / dV[0]);
        arrayRange.y.Add(value.GetElement(1) / dV[1]);
        arrayRange.z.Add(value.GetElement(2) / dV[2]);

        return this;
    }

    /// <summary>
    ///    配列の位置を実際の位置に変換
    /// </summary>
    /// <param name="arrayPosition"></param>
    /// <returns></returns>
    /*public Vector<T> GetRealPosition(Vector<T> arrayPosition)
    {
        return new Vector<T>(
            arrayPosition.X * dV[0] + realRange.x.min,
            arrayPosition.Y * dV[1] + realRange.y.min,
            arrayPosition.Z * dV[2] + realRange.z.min
        );
    }

    /// <summary>
    ///  実際の位置を配列の位置に変換
    /// </summary>
    /// <param name="realPosition"></param>
    /// <returns></returns>
    public Vector3 GetArrayPosition(Vector3 realPosition)
    {
        return new Vector3(
            (float)((realPosition.X - realRange.x.min) / dV.X),
            (float)((realPosition.Y - realRange.y.min) / dV.Y),
            (float)((realPosition.Z - realRange.z.min) / dV.Z)
        );
    }*/
}
