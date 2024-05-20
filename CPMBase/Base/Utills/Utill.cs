using System.Numerics;
using System.Diagnostics;

namespace CPMBase;

public static class Utill
{
    public static T JsonSerializerClone<T>(T src)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(src);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    public static double CalculateMSE(List<double> actual, List<double> predicted)
    {
        if (actual.Count != predicted.Count)
        {
            throw new ArgumentException("The length of the actual values and predicted values must be the same.");
        }

        double mse = 0.0;
        int n = actual.Count;

        for (int i = 0; i < n; i++)
        {
            double error = actual[i] - predicted[i];
            mse += error * error;
        }

        return mse / n;
    }

    public static double CalculateMSE(double actual, List<double> predicted)
    {
        var actualList = new List<double>();
        for (int i = 0; i < predicted.Count; i++)
        {
            actualList.Add(actual);
        }
        return CalculateMSE(actualList, predicted);
    }

    public static double CalculateMSE(List<double> predicted)
    {
        var predictedList = new List<double>();
        for (int i = 0; i < predicted.Count; i++)
        {
            predictedList.Add(0);
        }
        return CalculateMSE(predictedList, predictedList);
    }

    public static List<double> CalculateMSD(List<Vector3> positions)
    {
        List<double> msd = new List<double>();
        int count = positions.Count;

        for (int t = 0; t < count; t++)
        {
            msd[t] += (positions[t] - positions[0]).LengthSquared() / count;
        }

        return msd;
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
