using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CPMBase.Base;
using CPMBase.CPM;
using CPMBase.Dependency.Vector;
using CPMBase.ExSimrations;

namespace CPMBase;

public class MiddleCells : ISimration
{
    //範囲
    public static RangePosition range = new RangePosition(500, 500, 1, false);

    //出力先
    public PathObject path;

    public string pathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

    //時間の更新
    public int end = 1000;

    public StepUpdater updater;

    //public float writeDuration = 10;
    //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
    public float writeDuration => end / 1000; //数で決める

    public float T = 1f; //ボルツマン温度

    public CPM_Base cpm;

    public void PreInit()
    {
        updater = new StepUpdater(dt: 0.3, endTime: end);
        //updater.isStepOrTime = true;
    }

    void ISimration.Init()
    {
        /*
            Console.WriteLine("ManyCellSphere2DExample ");
            Console.WriteLine("初期化を開始します");

            path = new PathObject(pathName, "image.png");
            cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;

            Console.WriteLine("CPM領域を構築致しました");

            updater.Add(cpm); //CPMをセット
            updater.SetWrite(writeDuration, path); //書き出し設定
            updater.resolution = new Vector2(500, 500); //解像度設定


            Console.WriteLine("細胞を追加します");

            cpm.AddCellsRect(
                () => new Cell(r: 5, 1, 1, maxact: 35f, lact: 10, T: 5, kAdhesion: 50), //細胞のパラメータ
                new Position(8, 8, 0), //細胞の大きさ

                100 // 細胞
            ); // 細胞を追加*/
    }

    public async Task Start()
    {
        updater.StartSync(); //シミュレーション開始(非同期)
    }

    public void End()
    {
        Utill.RunBashScriptWithArgument("/workspaces/CPMBase_CSharp/movie.sh", this.GetType().Name); //動画作成
    }

    public void Final()
    {

    }
}

