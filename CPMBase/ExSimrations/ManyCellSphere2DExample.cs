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
    public class ManyCellSphere2DExample : ISimration
    {
        //範囲
        public static RangePosition range = new RangePosition(1000, 1000, 1, false);

        //出力先
        public PathObject path = new PathObject("/workspaces/CPMBase_CSharp/Output/ManyCellSphere2DExample", "image.png");

        public string pathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 50000000);

        //public float writeDuration = 10;
        //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
        public float writeDuration => (float)updater.endTime / 500; //数で決める

        public float T = 23; //ボルツマン温度

        public CPM_Base cpm;

        void Init()
        {
            Console.WriteLine("初期化を開始します");

            path = new PathObject(pathName, "image.png");
            cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;

            Console.WriteLine("CPM領域を構築致しました");
        }


        public void Run()
        {
            Init();

            Console.WriteLine("細胞を追加します");

            cpm.AddArea(
                () => new Cell(2500, 300, 1, 1), //細胞のパラメータ
                new RangePosition(600, 400, 600, 400, 0, 0), //細胞の位置
                100 // 細胞を追加
            ); // 細胞を追加

            Update();

            End();
        }

        public void Update()
        {
            updater.Add(cpm); //CPMをセット
            updater.SetWrite(writeDuration, path); //書き出し設定

            updater.StartSync(); //シミュレーション開始(非同期)
        }

        public void End()
        {
            Utill.RunBashScriptWithArgument("/workspaces/CPMBase_CSharp/movie.sh", this.GetType().Name); //動画作成
        }

        public void PreInit()
        {
            throw new NotImplementedException();
        }

        void ISimration.Init()
        {
            throw new NotImplementedException();
        }

        public Task Start()
        {
            throw new NotImplementedException();
        }

        public void Final()
        {
            throw new NotImplementedException();
        }
    }
}