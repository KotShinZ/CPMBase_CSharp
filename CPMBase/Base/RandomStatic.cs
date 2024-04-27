namespace CPMBase;

public class RandomStatic
{
    public static int seed = 0;

    public static void SetSeed(int value)
    {
        seed = value;
    }

    /// <summary>
    ///  乱数を生成する
    /// </summary>
    /// <returns></returns>
    public static int Next()
    {
        seed = seed * 1103515245 + 12345;
        return (seed / 65536) % 32768;
    }

    public static int Next(int max)
    {
        return Next() % max;
    }

    public static int Next(int min, int max)
    {
        return Next(max - min) + min;
    }

    public static float NextFloat()
    {
        return (float)Next() / 32768.0f;
    }

    public static float NextFloat(float max)
    {
        return NextFloat() * max;
    }

    public static float NextFloat(float min, float max)
    {
        return NextFloat(max - min) + min;
    }

    public static double NextDouble()
    {
        return (double)Next() / 32768.0;
    }

    public static double NextDouble(double max)
    {
        return NextDouble() * max;
    }

    public static double NextDouble(double min, double max)
    {
        return NextDouble(max - min) + min;
    }

    public static bool NextBool()
    {
        return Next() % 2 == 0;
    }

    public static bool NextBool(float probability)
    {
        return NextFloat() < probability;
    }

    public static T NextEnum<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Next(values.Length));
    }

    public static T NextEnum<T>(T[] values)
    {
        return values[Next(values.Length)];
    }


}
