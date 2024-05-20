namespace CPMBase;

public static class UtillList
{
    /// <summary>
    ///  リストからランダムな要素を取得
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        return list[Randomizer.Next(list.Count)];
    }

    /// <summary>
    /// リストの中身をシャッフルする
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count <= 0) { return; }
        int j = 0;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            j = Randomizer.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }


}
