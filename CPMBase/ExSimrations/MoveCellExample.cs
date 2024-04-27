using System;
using System.Collections.Generic;
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
    /// 重力をうける細胞のシミュレーション
    /// 2つ目の細胞だけ重力をうけて下に進んでいく
    /// </summary>
    public class MoveCellExample : ISimration
    {
        //範囲
        public static RangePosition range = new RangePosition(100, 100, 1, false);

        //出力先
        public PathObject path;

        public string pathName => "/workspaces/CPMBase_CSharp/Output/" + this.GetType().Name;

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 30000);

        //public float writeDuration = 10;
        //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
        public float writeDuration => (float)updater.endTime / 100; //数で決める

        public float T = 23; //ボルツマン温度

        void Init()
        {
            path = new PathObject(pathName, "image", extention: ".png");
        }

        public void Run()
        {
            Init();

            var cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;

            cpm.Add(
                new Cell(25, 30, 1, 1, 0), //細胞のパラメータ
                new RangePosition(55, 46, 60, 51, 0, 0) //細胞の位置
            ); // 細胞を追加

            cpm.Add(
                new Cell(25, 30, 1, 1, 0, new Vector3(0, -40, 0)), //細胞のパラメータ
                new RangePosition(55, 46, 50, 41, 0, 0) //細胞の位置
            ); // 細胞を追加

            updater.Add(cpm);
            updater.SetWrite(writeDuration, path);

            updater.StartSync();

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

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Final()
        {
            throw new NotImplementedException();
        }
    }
}