using System.Numerics;

namespace CPMBase;

public class IMSD
{
    public Dictionary<double, double> datas = new Dictionary<double, double>();

    public void Add(double time, double data)
    {
        datas.Add(time, data);
    }

    public Dictionary<double, double> Cull(int n = -1)
    {
        Dictionary<double, double> result = new Dictionary<double, double>();
        var num = n == -1 ? datas.Count : n;


        for (int i = 1; i <= num; i++)
        {
            double sum = 0;
            int count = 0;
            for (int t = 0; t < i; t++)
            {
                sum += datas.ElementAt(t).Value - datas.ElementAt(t + i).Value;
                count++;
            }
        }

        return result;
    }

    public static double CalculateMSD(List<double> positions)
    {
        int n = positions.Count;
        if (n < 2)
        {
            throw new ArgumentException("The list must contain at least two positions.");
        }

        double totalDisplacement = 0.0;
        for (int i = 0; i < n - 1; i++)
        {
            double displacement = positions[i + 1] - positions[i];
            totalDisplacement += displacement * displacement;
        }

        return totalDisplacement / (n - 1);
    }
}
