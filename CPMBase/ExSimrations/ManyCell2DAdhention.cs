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

namespace CPMBase.ExSimrations
{
    /// <summary>
    /// 接着力を持つ細胞のシミュレーション
    /// 重力をうけ、下に行こうとする細胞が接着力でくっつき動かなくなることを再現する
    /// </summary>

    public class ManyCell2DAdhention : ISimration
    {
        public static RangePosition range = new RangePosition(1000, 1000, 1, false);

        //出力先
        public PathObject path = new PathObject("/workspaces/CPMBase_CSharp/Output/ManyCell2DAdhention", "image.png");

        public string pathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 500);

        //public float writeDuration = 10;
        //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
        public float writeDuration => (float)updater.endTime / 100; //数で決める

        public float T = 1f; //ボルツマン温度

        public CPM_Base cpm;

        public void PreInit()
        {
        }

        void ISimration.Init()
        {
            /*Console.WriteLine("初期化を開始します");

            path = new PathObject(pathName, "image.png");
            cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;

            Console.WriteLine("CPM領域を構築致しました");

            updater.Add(cpm); //CPMをセット
            updater.SetWrite(writeDuration, path); //書き出し設定
            updater.resolution = new Vector2(1000, 1000); //解像度設定


            Console.WriteLine("細胞を追加します");

            cpm.AddArea(
                () => new Cell(r: 30, 1, 1, maxact: 100, lact: 10), //細胞のパラメータ
                new RangePosition(600, 400, 600, 400, 0, 0), //範囲
                100,// 細胞を追加
                new Position(30, 30, 0) //細胞の大きさ
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
}
