using System.Numerics;

namespace CPMBase;

/// <summary>
///   3次元の範囲を表すクラス  
/// </summary>
public class Range3 : ICalculateOperators<Range3, Vector3, Range3>
{
    private static Random random = new Random();


    public Range x;
    public Range y;
    public Range z;

    public Vector3 center => new Vector3((float)x.center, (float)y.center, (float)z.center);


    public Range3(Range x, Range y, Range z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Range3(double maxX = 1, double minX = 0, double maxY = 1, double minY = 0, double maxZ = 1, double minZ = 0)
    {
        x = new Range(maxX, minX);
        y = new Range(maxY, minY);
        z = new Range(maxZ, minZ);
    }

    public Range3(Vector3 max, Vector3 min)
    {
        x = new Range(max.X, min.X);
        y = new Range(max.Y, min.Y);
        z = new Range(max.Z, min.Z);
    }

    public Range3(Vector3 max)
    {
        x = new Range(max.X);
        y = new Range(max.Y);
        z = new Range(max.Z);
    }




    /// <summary>
    ///  0からMaxの範囲（isCenterなら中心が0になる）
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <param name="maxZ"></param>
    /// <param name="isCenter"></param>
    public Range3(double maxX, double maxY, double maxZ, bool isCenter = false)
    {
        var two = double.CreateChecked(2);
        if (isCenter)
        {
            x = new Range(maxX / two, -maxX / two);
            y = new Range(maxY / two, -maxY / two);
            z = new Range(maxZ / two, -maxZ / two);
        }
        else
        {
            x = new Range(maxX, 0);
            y = new Range(maxY, 0);
            z = new Range(maxZ, 0);
        }
        Length = new Vector3((float)maxX, (float)maxY, (float)maxZ);
    }

    public Vector3 Length;
    public Vector2 Length2D => new Vector2(Length.X, Length.Y);

    /// <summary>
    ///  valueがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(Vector3 value, bool equal = false)
    {
        return x.Contains(value.X) && y.Contains(value.Y) && z.Contains(value.Z);
    }

    /// <summary>
    ///  rangeがこのrangeに含まれるかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(Range3 range)
    {
        return x.Contains(range.x) && y.Contains(range.y) && z.Contains(range.z);
    }

    /// <summary>
    ///  rangeがこのrangeと重なっているかどうか
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Overlaps(Range3 range)
    {
        return x.Overlaps(range.x) && y.Overlaps(range.y) && z.Overlaps(range.z);
    }


    /// <summary>
    ///  range同士の重なっている部分を返す
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public Range3 Intersection(Range3 range)
    {
        return new Range3(x.Intersection(range.x), y.Intersection(range.y), z.Intersection(range.z));
    }

    public Vector3 GetRandom()
    {
        return new Vector3([(float)x.GetRandom(), (float)y.GetRandom(), (float)z.GetRandom()]);
    }

    public void Add(Vector3 value)
    {
        x.Add(value.X);
        y.Add(value.Y);
        z.Add(value.Z);
    }

    public static Range3 operator +(Range3 left, Vector3 right)
    {
        return new Range3(left.x + right.X, left.y + right.Y, left.z + right.Z);
    }

    public static Range3 operator -(Range3 left, Vector3 right)
    {
        return new Range3(left.x - right.X, left.y - right.Y, left.z - right.Z);
    }

    public static Range3 operator *(Range3 left, Vector3 right)
    {
        return new Range3(left.x * right.X, left.y * right.Y, left.z * right.Z);
    }

    public static Range3 operator /(Range3 left, Vector3 right)
    {
        return new Range3(left.x / right.X, left.y / right.Y, left.z / right.Z);
    }

    public override string ToString()
    {
        return $"x:{x}, y:{y}, z:{z}  ";
    }
}
