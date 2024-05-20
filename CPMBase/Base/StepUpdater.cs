using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CPMBase.Base;

public class StepUpdater : TimeUpdatable
{
    //public static StepUpdater instance; //インスタンス

    public List<IUpdatable> updatables = new(); //毎時間更新される処理を管理するためのリスト

    public Task task; //非同期で行われている処理を管理するためのクラス

    public float printDurationPer = 0.1f; //進捗を表示する間隔

    public Stopwatch stopwatch = new Stopwatch(); //時間計測

    public int stepNum = 0; //ステップ数

    public bool isInit = true;
    public bool isUpdate = true;

    public bool isEnd = true;

    public bool isProgress = true;

    public Action preUpdateFunc; //毎時間更新される処理を管理するためのデリゲート(floatで書き出し時間間隔を受け取る(0~1の間で受け取る))
    public Action postUpdateFunc; //毎時間更新される処理を管理するためのデリゲート(floatで書き出し時間間隔を受け取る(0~1の間で受け取る))

    public TimeAction preUpdateTimeAction;

    public TimeAction postUpdateTimeAction;

    public StepUpdater(double dt, double endTime)
    {
        nowTime = 0;
        this.dt = dt;
        this.endTime = endTime;
        updatables = new List<IUpdatable>();
        //instance = this;
    }

    /// <summary>
    /// 毎時間更新される処理を追加
    /// </summary>
    public void Add(IUpdatable updatable)
    {
        updatables.Add(updatable);
        if (updatable is TimeUpdatable timeUpdatable)
        {
            timeUpdatable.dt = dt;
            timeUpdatable.endTime = endTime;
        }
    }

    /// <summary>
    /// シミュレーションを開始
    /// </summary>
    public async Task Start()
    {
        Console.WriteLine("シミュレーション開始");
        stopwatch.Start();
        task = Task.Run(() =>
        {
            Simration();
            task.Dispose();
        });

        await task;

        stopwatch.Stop();
        Console.WriteLine("シミュレーション終了" + "時間: " + stopwatch.ElapsedMilliseconds + "ms" + "    ステップ数: " + stepNum);
    }

    /// <summary>
    /// シミュレーションを開始
    /// </summary>
    public void StartSync()
    {
        Console.WriteLine("シミュレーション開始");
        stopwatch.Start();

        Simration();

        stopwatch.Stop();
        Console.WriteLine("シミュレーション終了" + "時間: " + stopwatch.ElapsedMilliseconds + "ms" + "    ステップ数: " + stepNum);
    }

    /// <summary>
    ///  パスに時間の情報を追加
    /// </summary>
    /// <param name="time"></param>
    /// <param name="step"></param>
    /// <param name="pathObject"></param>
    /// <returns></returns>
    int pathNum = 0;
    public PathObject AddTimeToPathObj(StepUpdater updater, PathObject pathObject)
    {
        //pathObject.fullPath = pathObject.path + "/" + time + "_" + step + "_" + pathObject.name;
        pathObject.prename = pathNum + "_";
        pathNum++;
        return pathObject;
    }

    /// <summary>
    ///  シミュレーションを進める
    /// </summary>
    public void Simration()
    {
        if (isInit)
        {
            foreach (var updatable in updatables)
            {
                updatable.Init(this); //シミュレーション開始時に１度だけ呼び出される
            }
        }
        if (isUpdate)
        {
            while (nowTime < endTime)  //終了時間まで繰り返す
            {
                var isEnd = Step(); //ステップを進める
                if (isEnd) { break; }
            }
        }
        if (isEnd)
        {
            foreach (var updatable in updatables)
            {
                updatable.End(this); //シミュレーション開始時に１度だけ呼び出される
                if (updatable is TimeUpdatable timeUpdatable)
                {
                    timeUpdatable.Reset();
                }
            }
        }
        isEnd = false;
    }

    public virtual bool Step()
    {
        PreStep();
        preUpdateFunc?.Invoke(); //毎時間更新される処理を実行
        preUpdateTimeAction?.Invoke((float)dt);

        bool isAllEnd = true;
        bool isTimeUpdatable = false;

        if (nowTime % (endTime * printDurationPer) < dt && isProgress)
        {
            Console.WriteLine("進捗: " + (int)(nowTime / endTime * 100) + "%"); //進捗を表示
        }

        foreach (var updatable in updatables)
        {

            if (updatable is TimeUpdatable timeUpdatable)
            {
                isTimeUpdatable = true;
                if (timeUpdatable.isEnd == false) { isAllEnd = false; }

            }
            updatable.Update(this); //毎時間更新される処理を実行
        }


        postUpdateFunc?.Invoke(); //毎時間更新される処理を実行
        postUpdateTimeAction?.Invoke((float)dt);
        PostStep();
        nowTime += dt;
        stepNum++;
        return isAllEnd && isTimeUpdatable;
    }

    public virtual void PreStep() { }
    public virtual void PostStep() { }

    /// <summary>  
    ///  シミュレーションを停止  
    /// </summary>
    public void Stop() { task.Wait(); }

    /// <summary>
    /// シミュレーションをリセット
    /// </summary>
    public void Reset()
    {
        nowTime = 0;
        task.Dispose();
    }
}
