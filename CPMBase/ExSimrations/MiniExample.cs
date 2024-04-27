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
    /// 小さいシミュレーション
    /// それなりの範囲で、2つの細胞の動きを再現する
    /// </summary>
    public class MiniExample : ISimration
    {
        //範囲
        public static RangePosition range = new RangePosition(200, 200, 1, false);

        //出力先
        public PathObject path = new PathObject("/workspaces/CPMBase_CSharp/Output/MiniExample", "image", extention: ".png");
        public PathObject pathPer = new PathObject("/workspaces/CPMBase_CSharp/Output/MiniExample", "per", extention: ".png");

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 100000);

        //public float writeDuration = 10;
        //public float writeDuration => 0.01f * (float)updater.endTime; //割合で決める
        public float writeDuration => (float)updater.endTime / 100; //数で決める

        public float T = 23; //ボルツマン温度

        public void PreInit()
        {
        }

        public void Init()
        {
            var cpm = new CPM_Base(range, dim: Dimention._2d);
            cpm.T = T;

            cpm.Add(
                new Cell(20 * 20, (int)(20 * 4 * 1), 1, 1, maxact: 2000, lact: 100), //細胞のパラメータ
                new RangePosition(50, 40, 50, 40, 0, 0) //細胞の位置
            ); // 細胞を追加

            cpm.Add(
                new Cell(20 * 20, (int)(20 * 4 * 1), 1, 1, maxact: 5000, lact: 100), //細胞のパラメータ
                new RangePosition(150, 140, 50, 40, 0, 0) //細胞の位置
            ); // 細胞を追加

            cpm.Add(
                new Cell(20 * 20, (int)(20 * 4 * 1), 1, 1, maxact: 10000, lact: 100), //細胞のパラメータ
                new RangePosition(50, 40, 150, 140, 0, 0) //細胞の位置
            ); // 細胞を追加

            cpm.Add(
                new Cell(20 * 20, (int)(20 * 4 * 1), 1, 1, maxact: 20000, lact: 100), //細胞のパラメータ
                new RangePosition(150, 140, 150, 140, 0, 0) //細胞の位置
            ); // 細胞を追加

            updater.Add(cpm);
            updater.SetWrite(writeDuration, path);

            updater.OnWrite = () =>
            {
                pathPer.name = "Activity" + (int)(updater.stepNum / writeDuration);
                cpm.WriteSKActivty((float)updater.time, updater.stepNum, pathPer);
            };
        }

        public async Task Start()
        {
            updater.StartSync();
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