namespace CPMBase;


using System;
using System.Numerics;

/// <summary>
///   3次元の範囲を表すクラス  Decimal版(計算誤差が少ない、体積が大きい、小さい場合に使用)
/// </summary>
public class Range3Decimal : Range3<Decimal>
{
    /// <summary>
    ///  rangeがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(Range3Decimal range)
    {
        return x.Contains(range.x) && y.Contains(range.y) && z.Contains(range.z);
    }

    /// <summary>
    ///  rangeがこのrangeと重なっているかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Overlaps(Range3Decimal range)
    {
        return x.Overlaps(range.x) && y.Overlaps(range.y) && z.Overlaps(range.z);
    }
}
