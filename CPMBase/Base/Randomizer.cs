namespace CPMBase;

public class Randomizer
{
    public static int seed = 0;

    private static Random random = new Random();

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
        return random.Next();
        //seed = seed * 1103515245 + 12345;
        //return (seed / 65536) % 32768;
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
        return random.NextSingle();
    }

    public static float NextFloat(float min, float max)
    {
        return NextFloat() * (max - min) + min;
    }

    public static double NextDouble()
    {
        return random.NextDouble();
    }

    public static double NextDouble(double min, double max)
    {
        return NextDouble() * (max - min) + min;
    }

}
