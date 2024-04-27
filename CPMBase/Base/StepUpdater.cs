using System.Diagnostics;
using System.Numerics;

namespace CPMBase.Base;

public class StepUpdater
{
    public double dt = 0.1; //時間刻み
    public double time; //現在の時間
    public double endTime;  //終了時間

    public List<IUpdatable> updatables; //毎時間更新される処理を管理するためのリスト

    public Task task; //非同期で行われている処理を管理するためのクラス

    public Random random = new Random(); //乱数

    public float printDurationPer = 0.1f; //進捗を表示する間隔

    public Stopwatch stopwatch = new Stopwatch(); //時間計測

    public int stepNum = 0; //ステップ数

    public Action preUpdateFunc; //毎時間更新される処理を管理するためのデリゲート(floatで書き出し時間間隔を受け取る(0~1の間で受け取る))
    public Action postUpdateFunc; //毎時間更新される処理を管理するためのデリゲート(floatで書き出し時間間隔を受け取る(0~1の間で受け取る))

    public Action OnWrite; //ファイルを書き込む処理を管理するためのデリゲート


    public bool isWrite = false; //ファイルを書き込むかどうか
    public float writeTime = 1; //ファイルを書き込む間隔
    public int writeStep = 0; //ファイルを書き込むステップ数
    public bool isStepOrTime = false; //ファイルを書き込むステップ数か時間か true:ステップ数 false:時間
    public PathObject writepath; //ファイルを書き込むパス

    public Vector2 resolution = default; //画像の解像度

    //public List<TimeFunc> timeFuncs = new List<TimeFunc>(); //時間によって実行される処理を管理するためのリスト


    public StepUpdater(double dt, double endTime)
    {
        this.dt = dt;
        this.endTime = endTime;
        time = 0;
        updatables = new List<IUpdatable>();
    }

    public void SetWrite(float time, PathObject path, Vector2 resolution = default)
    {
        isWrite = true;
        writeTime = time;
        isStepOrTime = false;
        writepath = path;
        this.resolution = resolution;
    }

    public void SetWriteStep(int step, PathObject path, Vector2 resolution = default)
    {
        isWrite = true;
        writeStep = step;
        isStepOrTime = true;
        writepath = path;
        this.resolution = resolution;
    }

    /// <summary>
    /// 毎時間更新される処理を追加
    /// </summary>
    public void Add(IUpdatable updatable)
    {
        updatables.Add(updatable);
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
    PathObject GetPath(float time, int step, PathObject pathObject)
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
        foreach (var updatable in updatables)
        {
            updatable.Init(this); //シミュレーション開始時に１度だけ呼び出される
        }
        while (time < endTime)  //終了時間まで繰り返す
        {
            if (time % (endTime * printDurationPer) < dt)
            {
                Console.WriteLine("進捗: " + (int)(time / endTime * 100) + "%"); //進捗を表示
            }


            preUpdateFunc?.Invoke(); //毎時間更新される処理を実行
            foreach (var updatable in updatables)
            {
                updatable.Update(this); //毎時間更新される処理を実行
            }
            postUpdateFunc?.Invoke(); //毎時間更新される処理を実行

            if (isWrite)
            {
                if (isStepOrTime)
                {
                    if (stepNum % writeStep == 0)
                    {
                        Write();
                    }
                }
                else
                {
                    if (time % writeTime < dt)
                    {
                        Write();
                    }
                }
            }

            time += dt;
            stepNum++;
        }

    }

    /// <summary>
    ///  すべてのUpdatablesのWriteメソッドを呼び出す
    /// </summary>
    public void Write()
    {
        foreach (var updatable in updatables)
        {
            OnWrite?.Invoke();
            var writable = updatable as ITimePathWrite;
            writable.Write((float)time, stepNum, GetPath((float)time, stepNum, writepath)); //ファイルを書き込む
        }
    }

    /// <summary>  
    ///  シミュレーションを停止  
    /// </summary>
    public void Stop()
    {
        task.Wait();
    }


    /// <summary>
    /// シミュレーションをリセット
    /// </summary>
    public void Reset()
    {
        time = 0;
        task.Dispose();
    }
}
