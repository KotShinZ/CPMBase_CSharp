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
    /// 最小限のシミュレーション
    /// 狭い範囲で、2つの細胞の動きを再現する
    /// </summary>
    public class MicroExample : ISimration
    {
        //範囲
        public static RangePosition range = new RangePosition(30, 30, 1, false);

        //出力先
        public PathObject path = new PathObject("/workspaces/CPMBase_CSharp/Output/MicroExample", "image", extention: ".png");

        //時間の更新
        public StepUpdater updater = new StepUpdater(dt: 1, endTime: 30);

        //public float writeDuration = 0;
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
                new Cell(20, 40, 1, 1), //細胞のパラメータ
                new Position(10, 15, 0), //細胞の位置
                new Position(4, 4, 0) //細胞の大きさ
            ); // 細胞を追加

            cpm.Add(
                new Cell(20, 10, 1, 1), //細胞のパラメータ
                new Position(20, 15, 0), //細胞の位置
                new Position(4, 4, 0) //細胞の大きさ
            ); // 細胞を追加

            updater.Add(cpm);

            updater.SetWrite(10000000, path);
            updater.isWrite = false;
            //updater.Write();

            updater.preUpdateFunc += () =>
            {
                Console.WriteLine(
                "A1 : " + cpm.cells[0].A + "   " + "L1 : " + cpm.cells[0].L + "   " + "A2 : " + cpm.cells[1].A + "   " + "L2 : " + cpm.cells[1].L
                );
                updater.Write();
                //cpm.WriteAsJson(updater, path);
            };
        }

        public async Task Start()
        {
            updater.StartSync();
        }

        public void End()
        {
            //cpm.WriteAsJson(updater, path);
            Utill.RunBashScriptWithArgument("/workspaces/CPMBase_CSharp/movie.sh", this.GetType().Name); //動画作成
        }

        public void Final()
        {
        }
    }
}