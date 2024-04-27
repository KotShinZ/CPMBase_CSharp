using System.Numerics;
using System.Diagnostics;

namespace CPMBase;

public static class Utill
{
    static Random random = new Random();
    public static T JsonSerializerClone<T>(T src)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(src);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    ///  リストからランダムな要素を取得
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        return list[random.Next(list.Count)];
    }

    /// <summary>
    ///  Bashスクリプトを実行
    /// </summary>
    /// <param name="scriptPath"></param>
    /// <param name="arg"></param>
    public static void RunBashScriptWithArgument(string scriptPath, string arg = "")
    {
        // ProcessStartInfoの設定
        ProcessStartInfo procStartInfo = new ProcessStartInfo("bash", $"{scriptPath} {arg}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Processオブジェクトの作成と開始
        Process proc = new Process
        {
            StartInfo = procStartInfo
        };

        proc.Start();

        // スクリプトの実行結果を読み取る
        string result = proc.StandardOutput.ReadToEnd();

        // 実行結果を表示
        Console.WriteLine(result);

        proc.WaitForExit();
    }
}
