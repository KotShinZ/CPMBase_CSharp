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
    public class AdhesionExample : ISimration
    {
        //範囲
        public static RangePosition range = new RangePosition(100, 100, 1, false);

        //出力先
        public PathObject path = new PathObject("/workspaces/CPMBase_CSharp/Output/AdhesionExample", "image.png");

        public string pathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 60000);

        //public float writeDuration = 10;
        //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
        public float writeDuration => (float)updater.endTime / 100; //数で決める

        public float T = 23; //ボルツマン温度

        public CPM_Base cpm;

        void Init()
        {
            Console.WriteLine("初期化を開始します");

            path = new PathObject(pathName, "image.png");
            cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;
        }


        public void Run()
        {
            Init();

            cpm.Add(
                new Cell(5, 1, 1, -10), //細胞のパラメータ
                new RangePosition(55, 46, 60, 51, 0, 0) //細胞の位置
            ); // 細胞を追加

            cpm.Add(
                new Cell(5, 1, 1, -10), //細胞のパラメータ
                new RangePosition(55, 46, 50, 41, 0, 0) //細胞の位置
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
            //Console.WriteLine("Current Directory: " + Environment.CurrentDirectory);
            Utill.RunBashScriptWithArgument("/movie.sh", this.GetType().Name); //動画作成
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