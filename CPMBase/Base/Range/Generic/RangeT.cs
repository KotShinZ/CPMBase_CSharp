using System.Numerics;
using System;


namespace CPMBase;

public class Range<T>: ICalculateOperators<Range<T>, T, Range<T>> where T : INumber<T>
{
    protected static Random random = new Random();

    public T max;
    public T min;
    
    public Range(T max, T min) 
    {
        this.max = max;
        this.min = min;
    }

    public Range(T max) 
    {
        this.max = max;
        this.min = T.Zero;
    }

    public Range()
    {
        this.max = T.One;
        this.min = T.Zero;
    }

    public T Length => max - min;

    public bool Contains(T value)
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// rangeがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(Range<T> range)
    {
        return range.min.CompareTo(min) >= 0 && range.max.CompareTo(max) <= 0;
    }


    /// <summary>
    ///  rangeがこのrangeと重なっているかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Overlaps(Range<T> range)
    {
        return range.min.CompareTo(max) <= 0 && range.max.CompareTo(min) >= 0;
    }

    public T Max(T one, T other)
    {
        return one.CompareTo(other) > 0 ? one : other;
    }
    public T Min(T one, T other)
    {
        return one.CompareTo(other) > 0 ? one : other;
    }

    /// <summary>
    ///  range同士の重なっている部分を返す
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public Range<T> Intersection(Range<T> range)
    {
        return new Range<T>(Max(min, range.min), Min(max, range.max));
    }

    public Range<T> Union(Range<T> range)
    {
        return new Range<T>(Min(min, range.min), Max(max, range.max));
    }

    /// <summary>
    ///  valueがこのrangeに含まれるかどうか 
    /// 例　
    /// value = 3 , range = 1~5 -> true
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsContain(T value)
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    ///  valueをこのrangeに収める
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public T Clamp(T value)
    {
        return Max(min, Min(max, value));
    }

    /// <summary>
    ///  線形補間 (tは0~1の間の値を取る)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public T Lerp(T t)
    {
        return min + Length * t;
    }

    /// <summary>
    /// 線形補間 (0~1の間の値を返す)
    /// </summary>
    public T InverseLerp(T value)
    {
        return (value - min) / Length;
    }

    /// <summary>
    ///  range内のランダムな値を取得する
    /// </summary>
    /// <returns></returns>
    public virtual T GetRandom()
    {
        // T型の最小値と最大値の範囲でランダムな値を生成します。
        // ここでは、T型がdoubleに変換可能であると仮定して、
        // NextDoubleを使用してランダムな値を生成し、
        // その後、T型の範囲に合わせて変換します。
        
        // minとmaxの間のランダムな値を生成します。
        double range = (double)(object)max - (double)(object)min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + (double)(object)min;

        return T.CreateChecked(scaled);
    }

    public Range<R> Cast<R> () where R : INumber<R>
    {
        return new Range<R>(R.CreateChecked(max), R.CreateChecked(min));
    }

    public void Add(T value)
    {
        max += value;
        min += value;
    }

    public static Range<T> operator +(Range<T> left, T right)
    {
        return new Range<T>(left.max + right, left.min + right);
    }

    public static Range<T> operator *(Range<T> left, T right)
    {
        return new Range<T>(left.max * right, left.min * right);
    }

    public static Range<T> operator -(Range<T> left, T right)
    {
        return new Range<T>(left.max - right, left.min - right);
    }

    public static Range<T> operator /(Range<T> left, T right)
    {
        return new Range<T>(left.max / right, left.min / right);
    }
}
