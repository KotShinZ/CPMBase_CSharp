namespace CPMBase;


/// <summary>
/// １次元の範囲を表すクラス
/// </summary>
public class Range : ICalculateOperators<Range, double, Range>
{
    protected static Random random = new Random();

    public double max;
    public double min;

    public double center => (max + min) / 2;

    public Range(double max, double min)
    {
        this.max = max;
        this.min = min;
    }

    public Range(double max)
    {
        this.max = max;
        this.min = 0;
    }

    public Range()
    {
        this.max = 1;
        this.min = 0;
    }

    public double Length => max - min;

    public bool Contains(double value, bool equal = false)
    {
        return value < max && value > min;
    }

    /// <summary>
    /// rangeがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(Range range)
    {
        return range.min > min && range.max < max;
    }


    /// <summary>
    ///  rangeがこのrangeと重なっているかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Overlaps(Range range)
    {
        return range.min.CompareTo(max) <= 0 && range.max.CompareTo(min) >= 0;
    }

    public double Max(double one, double other)
    {
        return one.CompareTo(other) > 0 ? one : other;
    }
    public double Min(double one, double other)
    {
        return one.CompareTo(other) > 0 ? one : other;
    }

    /// <summary>
    ///  range同士の重なっている部分を返す
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public Range Intersection(Range range)
    {
        return new Range(Max(min, range.min), Min(max, range.max));
    }

    public Range Union(Range range)
    {
        return new Range(Min(min, range.min), Max(max, range.max));
    }

    /// <summary>
    ///  valueがこのrangeに含まれるかどうか 
    /// 例　
    /// value = 3 , range = 1~5 -> true
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsContain(double value)
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    ///  valueをこのrangeに収める
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public double Clamp(double value)
    {
        return Max(min, Min(max, value));
    }

    /// <summary>
    ///  線形補間 (tは0~1の間の値を取る)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public double Lerp(double t)
    {
        return min + Length * t;
    }

    /// <summary>
    /// 線形補間 (0~1の間の値を返す)
    /// </summary>
    public double InverseLerp(double value)
    {
        return (value - min) / Length;
    }

    /// <summary>
    ///  range内のランダムな値を取得する
    /// </summary>
    /// <returns></returns>
    public virtual double GetRandom()
    {
        // T型の最小値と最大値の範囲でランダムな値を生成します。
        // ここでは、T型がdoubleに変換可能であると仮定して、
        // NextDoubleを使用してランダムな値を生成し、
        // その後、T型の範囲に合わせて変換します。

        // minとmaxの間のランダムな値を生成します。
        double range = (double)(object)max - (double)(object)min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + (double)(object)min;

        return double.CreateChecked(scaled);
    }

    public void Add(double value)
    {
        max += value;
        min += value;
    }

    public static Range operator +(Range left, double right)
    {
        return new Range(left.max + right, left.min + right);
    }

    public static Range operator *(Range left, double right)
    {
        return new Range(left.max * right, left.min * right);
    }

    public static Range operator -(Range left, double right)
    {
        return new Range(left.max - right, left.min - right);
    }

    public static Range operator /(Range left, double right)
    {
        return new Range(left.max / right, left.min / right);
    }

    public override string ToString()
    {
        return $"max:{max}, min:{min}";
    }
}
