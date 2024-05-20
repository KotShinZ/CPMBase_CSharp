using System.Numerics;
using CPMBase.Base;

namespace CPMBase;

public class StepUpdaterWithWrite : StepUpdater
{
    public Action<StepUpdater> OnWrite; //ファイルを書き込む処理を管理するためのデリゲート

    public Action<StepUpdater> writeAction; //ファイルを書き込む処理を管理するためのデリゲート

    public bool isWrite = false; //ファイルを書き込むかどうか
    public double writeNowTime = 10000000000000000; //ファイルを書き込む時間

    public float writeNum = 1; //ファイルを書き込む間隔


    //ファイルを書き込むステップ数か時間か true:ステップ数 false:時間
    public PathObject writepath; //ファイルを書き込むパス

    public Vector2 resolution = default; //画像の解像度

    public StepUpdaterWithWrite(double dt, double endTime, float writeNum, PathObject path, Action<StepUpdater> writeAction = null, Vector2 resolution = default) : base(dt, endTime)
    {
        isWrite = true;
        this.writeNum = writeNum;
        writepath = path;
        this.writeAction = writeAction;

        this.resolution = resolution == default ? new Vector2(256, 256) : resolution;
    }

    /// <summary>
    ///  すべてのUpdatablesのWriteメソッドを呼び出す
    /// </summary>
    public virtual void Write()
    {
        foreach (var updatable in updatables)
        {
            OnWrite?.Invoke(this);
            writeAction?.Invoke(this);
            var writable = updatable as ITimePathWrite;

            if (writable == null) continue;
            writable.WriteImage((float)nowTime, stepNum, AddTimeToPathObj(this, writepath), resolution); //ファイルを書き込む
            //writable.Write((float)time, stepNum, AddTimeToPathObj((float)time, stepNum, writepath)); //ファイルを書き込む
        }
    }

    public override void PreStep()
    {
        if (isWrite && writeNowTime >= endTime / writeNum)
        {
            //Console.WriteLine("step" + StepUpdater.instance.stepNum);
            Write();
            writeNowTime = 0;
        }
        writeNowTime += dt;

        //Console.WriteLine(((CPMUpdater)updatables[0]).cellAreaArray.cells[0].L);
        //Console.WriteLine("step" + StepUpdater.instance.stepNum);
        //Write();
    }

}
