using System.Diagnostics;

namespace CPMBase;

public class SpeadTester
{
    protected int num;

    float per = 0.1f;

    public SpeadTester(int num)
    {
        this.num = num;
    }

    public virtual void Test()
    {

    }

    public void Start()
    {
        Console.WriteLine("計測開始");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < num; i++)
        {
            if (i % (int)(num * per) == 0)
            {
                Console.WriteLine(i / num * 100 + "%");
            }
            Test();
        }
        sw.Stop();
        Console.WriteLine(this.GetType().Name + "    実行速度" + sw.ElapsedMilliseconds);
    }
}
