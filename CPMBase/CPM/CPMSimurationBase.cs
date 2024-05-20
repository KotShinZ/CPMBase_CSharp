using System.Numerics;
using CPMBase.Base;
using CPMBase.CPM;
using CPMBase.ExSimrations;
using Newtonsoft.Json;

namespace CPMBase;

public abstract class CPMSimurationBase : ISimration
{
    /// <summary>
    /// シミュレーション範囲
    /// </summary>
    public abstract RangePosition range { get; }

    public int id;

    /// <summary>
    /// 終了時間
    /// </summary>
    public abstract int end { get; }

    /// <summary>
    /// 何次元でシミュレーションするか
    /// </summary>
    /// <value></value>
    public abstract Dimention dim { get; }

    /// <summary>
    ///  事前シミュレーション時間
    /// </summary>
    public virtual int preSimulateTime => 0;


    #region WriteParams



    /// <summary>
    /// 出力フォルダ
    /// </summary>
    public virtual string pathName
    {
        get
        {
            if (nowPathName == null) return defaultPathName;
            return nowPathName;
        }
        set => nowPathName = value;
    }

    string defaultPathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

    public string nowPathName;

    /// <summary>
    /// 画像の解像度
    /// </summary>
    public virtual Vector2 resolution => range.arrayRange.Length2D;

    /// <summary>
    /// 書き込み回数
    /// </summary>
    /// <value></value>
    public abstract int writeNum { get; }

    /// <summary>
    /// 書き込み間隔
    /// </summary>
    public float writeDuration => end / writeNum; //数で決める

    /// <summary>
    /// MSDをプロットするか
    /// </summary>
    public virtual bool isPlotMSD => false;

    /// <summary>
    ///  MSDのPathの名前
    /// </summary>
    /// <returns></returns>
    public virtual string MSDImageName => "msd_Image";

    /// <summary>
    ///  Jsonデータを出力するか
    /// </summary>
    public virtual bool isOutputJson => false;


    /// <summary>
    ///  Jsonの名前
    /// </summary> <summary>
    public virtual string jsonName => this.GetType().Name + "_Json";

    /// <summary>
    ///  テストをするか
    /// </summary>
    public virtual bool isTest => false;

    /// <summary>
    /// テストの回数
    /// </summary>
    public virtual int testNum => 10;

    #endregion


    /// <summary>
    /// 前回の続きからシミュレーションをするか
    /// </summary>
    public virtual bool isContinue => false;

    public PathObject path;

    public PathObject MSDPath;

    public PathObject jsonPath;



    public StepUpdater updater;
    public CPMUpdater cPMUpdater;
    public CPMAreaArray cPMAreaArray;

    public void PreInit()
    {
        Console.WriteLine(this.GetType().Name);
        Console.WriteLine("初期化を開始します");

        path = new PathObject(pathName, "image", extention: ".png");
        MSDPath = new PathObject(pathName, MSDImageName, extention: ".png");
        jsonPath = new PathObject(pathName, jsonName, extention: ".json");

        updater = new StepUpdaterWithWrite(
            dt: 1,
            endTime: end,
            writeNum: writeNum,
            path: path,
            resolution: resolution
        );

        if (isTest) ((StepUpdaterWithWrite)updater).OnWrite += s => { cPMAreaArray.Test(); };
        if (isPlotMSD) ((StepUpdaterWithWrite)updater).OnWrite += s => { cPMAreaArray.AddMSDData(updater.nowTime); };

    }

    public void Init()
    {
        if (isContinue)
        {
            cPMAreaArray = JsonConvert.DeserializeObject<CPMAreaArray>(System.IO.File.ReadAllText(pathName + jsonName + ".json"));
            Console.WriteLine("CPM領域を読み込みました");
        }
        else
        {
            cPMAreaArray = new CPMAreaArray(range, dim: dim);
            Console.WriteLine("CPM領域を構築致しました");
        }
        cPMUpdater = new CPMUpdater(cPMAreaArray);

        updater.Add(cPMUpdater); //CPMをセット

        Console.WriteLine("細胞を追加します");

        Add_Cell();
    }

    public abstract void Add_Cell();

    public async Task Start()
    {
        if (preSimulateTime > 0)
        {
            var preUpdater = new StepUpdater(dt: 1, endTime: preSimulateTime);
            preUpdater.Add(cPMUpdater);
            preUpdater.isEnd = false;
            preUpdater.isProgress = false;

            preUpdater.StartSync(); //シミュレーション開始(同期)

            updater.isInit = false;
        }

        cPMAreaArray.SetInitCenter(); //中心の位置を初期化

        cPMUpdater.constraints.ForEach(c => c.isCullAverage = true);

        updater.StartSync(); //シミュレーション開始(同期)
    }

    public void End()
    {
        if (isPlotMSD) cPMAreaArray.linePlotter.Plot(MSDPath); //MSDのプロット
        if (isOutputJson) cPMAreaArray.WriteAsJson(jsonPath); //Jsonの出力
        Utill.RunBashScriptWithArgument("/workspaces/CPMBase_CSharp/movie.sh", pathName); //動画作成
    }

    public void Final()
    {
        //cPMAreaArray.Test();
    }
}
