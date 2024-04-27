using System.Numerics;

namespace CPMBase;

/// <summary>
///  3次元の範囲を表す ジェネリック版
/// </summary>
/// <typeparam name="T"></typeparam>
public class Range3<T> : ICalculateOperators<Range3<T>, Vector<T>, Range3<T>> where T : INumber<T>
{
    public Range<T> x;
    public Range<T> y;
    public Range<T> z;

    public Range3(Range<T> x, Range<T> y, Range<T> z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    ///  3つの範囲を指定
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="minX"></param>
    /// <param name="maxY"></param>
    /// <param name="minY"></param>
    /// <param name="maxZ"></param>
    /// <param name="minZ"></param>
    public Range3(T maxX, T minX, T maxY, T minY, T maxZ, T minZ)
    {
        x = new Range<T>(maxX, minX);
        y = new Range<T>(maxY, minY);
        z = new Range<T>(maxZ, minZ);
    }

    /// <summary>
    ///  0からMaxの範囲（isCenterなら中心が0になる）
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <param name="maxZ"></param>
    /// <param name="isCenter"></param>
    public Range3(T maxX, T maxY, T maxZ, bool isCenter = false)
    {
        var two = T.CreateChecked(2);
        if (isCenter)
        {
            x = new Range<T>(maxX / two, -maxX / two);
            y = new Range<T>(maxY / two, -maxY / two);
            z = new Range<T>(maxZ / two, -maxZ / two);
        }
        else
        {
            x = new Range<T>(maxX, T.Zero);
            y = new Range<T>(maxY, T.Zero);
            z = new Range<T>(maxZ, T.Zero);
        }
        Length = new Vector<T>([maxX, maxY, maxZ]);
    }

    /// <summary>
    /// 0から1の範囲
    /// </summary>
    public Range3()
    {
        x = new Range<T>(T.One, T.Zero);
        y = new Range<T>(T.One, T.Zero);
        z = new Range<T>(T.One, T.Zero);
        Length = new Vector<T>([T.One, T.One, T.One], 3);
    }

    public Range3(Vector<T> max, Vector<T> min)
    {
        x = new Range<T>(max.GetElement(0), min.GetElement(0));
        y = new Range<T>(max.GetElement(1), min.GetElement(1));
        z = new Range<T>(max.GetElement(2), min.GetElement(2));
        Length = max - min;
    }

    public Range3(Vector<T> max)
    {
        x = new Range<T>(max.GetElement(0), T.Zero);
        y = new Range<T>(max.GetElement(1), T.Zero);
        z = new Range<T>(max.GetElement(2), T.Zero);
        Length = max;
    }

    public Vector<T> Length;

    /// <summary>
    ///  valueがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(Vector<T> value)
    {
        return x.Contains(value.GetElement(0)) && y.Contains(value.GetElement(1)) && z.Contains(value.GetElement(2));
    }

    /// <summary>
    ///  rangeがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(Range3<T> range)
    {
        return x.Contains(range.x) && y.Contains(range.y) && z.Contains(range.z);
    }

    /// <summary>
    ///  rangeがこのrangeと重なっているかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Overlaps(Range3<T> range)
    {
        return x.Overlaps(range.x) && y.Overlaps(range.y) && z.Overlaps(range.z);
    }


    /// <summary>
    ///  range同士の重なっている部分を返す
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public Range3<T> Intersection(Range3<T> range)
    {
        return new Range3<T>(x.Intersection(range.x), y.Intersection(range.y), z.Intersection(range.z));
    }

    public Vector<T> GetRandom()
    {
        return new Vector<T>(new T[] { x.GetRandom(), y.GetRandom(), z.GetRandom() });
    }

    public Range3<R> Cast<R>() where R : INumber<R>
    {
        return new Range3<R>(x.Cast<R>(), y.Cast<R>(), z.Cast<R>());
    }

    public void Add(Vector<T> value)
    {
        x.Add(value.GetElement(0));
        y.Add(value.GetElement(1));
        z.Add(value.GetElement(2));
    }

    public static Range3<T> operator +(Range3<T> left, Vector<T> right)
    {
        return new Range3<T>(left.x + right.GetElement(0), left.y + right.GetElement(1), left.z + right.GetElement(2));
    }

    public static Range3<T> operator -(Range3<T> left, Vector<T> right)
    {
        return new Range3<T>(left.x - right.GetElement(0), left.y - right.GetElement(1), left.z - right.GetElement(2));
    }

    public static Range3<T> operator *(Range3<T> left, Vector<T> right)
    {
        return new Range3<T>(left.x * right.GetElement(0), left.y * right.GetElement(1), left.z * right.GetElement(2));
    }

    public static Range3<T> operator /(Range3<T> left, Vector<T> right)
    {
        return new Range3<T>(left.x / right.GetElement(0), left.y / right.GetElement(1), left.z / right.GetElement(2));
    }
}
