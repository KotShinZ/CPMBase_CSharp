using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CPMBase.CPM
{
    /// <summary>
    /// ひとつの細胞を表すクラス
    /// </summary>
    public class Cell
    {
        public Position initPosition; //初期位置

        public int A0; //目標面積

        public int preA; //前の面積
        public int A
        {
            get
            {
                return _A;
            }
            set
            {
                preA = _A;
                _A = value;
            }
        } //面積
        private int _A; //面積

        public int L0; //目標周長
        public int L
        {
            get
            {
                return _L;
            }
            set
            {
                preL = _L;
                _L = value;
            }

        } //周長

        //public int L_easy => (L > 0) ? 1 : 0; //外側にあるかどうか

        public int preL { get; private set; } //前の周長
        public int _L { get; private set; } //周長

        public float kA; //面積係數

        public float kL; //周長係數

        public Color color; //セルの色

        public int id = 0; //セルのID

        //public List<CPMArea> areas; //細胞の領域

        public float nowEnergy
        {
            get
            {
                return _energy;
            }
            set
            {
                preEnergy = _energy;
                _energy = value;
            }
        } //エネルギー
        private float preEnergy; //前のエネルギー
        private float _energy; //エネルギー

        public float kAdhesion; //接着力の定数

        public Vector3 constantPower; //一定の力をかける


        public float maxact = 1; //活動持続性（最小値 1）

        public float lact = 0; //活動量

        public float T = 1;

        public Cell(
            int A0,
            int L0,
            float kA,
            float kL,
            float kAdhesion = 0f,
            Vector3 constantPower = default(Vector3),
            float minusEnergySwaped = 0f,
            float maxact = 1,
            float lact = 0,
            float T = 0.5f
        )
        {
            this.A0 = A0;
            this.L0 = L0;
            this.kA = kA;
            this.kL = kL;

            this.kAdhesion = kAdhesion;
            this.constantPower = constantPower == default(Vector3) ? new Vector3(0, 0, 0) : constantPower;

            this.maxact = maxact;
            this.lact = lact;
            this.T = T;

            SetRandomColor();
        }

        public Cell(
            int r,
            float kA,
            float kL,

            float kAdhesion = 0f,
            Vector3 constantPower = default(Vector3),

            float minusEnergySwaped = 0f,
            float maxact = 1,
            float lact = 0,
            float T = 1f
        )
        {
            this.A0 = (int)(r * r * MathF.PI);
            this.L0 = (int)(2 * r * MathF.PI);
            this.kA = kA;
            this.kL = kL;
            this.kAdhesion = kAdhesion;
            this.constantPower = constantPower == default(Vector3) ? new Vector3(0, 0, 0) : constantPower;

            this.maxact = maxact;
            this.lact = lact;
            this.T = T;

            SetRandomColor();
        }
        /*
                public void SetAreas(List<CPMArea> areas)
                {
                    this.areas = areas;
                }

                /// <summary>
                ///  範囲内の細胞を登録
                /// </summary>
                /// <param name="range"></param>
                /// <param name="array"></param>
                public void SetAreas(RangePosition range, CellAreaArray array)
                {
                    array.AreaFunc(range, (c) => area.Add(c));
                }

                public void AddArea(CellArea area)
                {
                    this.area.Add(area);
                }

                public void RemoveArea(CellArea area)
                {
                    this.area.Remove(area);
                }*/

        /// <summary>
        /// 表面積を計算
        /// </summary>
        /// <param name="add"></param>
        /// <param name="remove"></param> <summary>
        /// 
        /// </summary>
        public void CullA(CPMArea add = null, CPMArea remove = null)
        {
            A += (add == null ? 0 : 1) - (remove == null ? 0 : 1);
        }

        /// <summary>
        /// 細胞の周長を更新(辺の数で更新)
        /// </summary>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        public void CullL(CPMArea add = null, CPMArea remove = null)
        {
            var minus = 0; //隣の細胞で隣接している細胞の数が変わる場合の数
            var nextNum = 0; //隣接している同じ細胞の数

            if (add != null) //細胞を追加する場合(addはこの細胞と同じではない)
            {
                add.NextFunc((c, v) =>
                {
                    if (c.cell == this)//隣の同じ細胞をカウントする
                    {
                        nextNum++;
                    }
                    return false;
                }, add.dim);
                //L += (int)add.dim * 2 - nextNum + minus;

                //L -= nextNum;
                L += (int)add.dim * 2 - nextNum * 2; //次元に応じて辺が増え、重なっている部分は2倍ずつ減る
                return;
            }
            else if (remove != null) //細胞を削除する場合
            {
                /*remove.NextFunc((c, v) =>
                {
                    if (c.cell == this)//隣の同じ細胞をカウントする
                    {
                        nextNum++;
                        if (c.nextSame == 4) minus--;
                        //すでに隣接している同じｈ細胞の数が4の場合、その細胞は隣接することになるので
                    }
                    return false;
                }, remove.dim);
                L += nextNum - minus;*/

                remove.NextFunc((c, v) =>
                {
                    if (c.cell == this)//隣の同じ細胞をカウントする
                    {
                        nextNum++;
                    }
                    return false;
                }, remove.dim);

                L += nextNum - ((int)remove.dim * 2 - nextNum);
            }
            else
            {
                preL = L;
                return;
            }
        }

        /// <summary>
        /// 隣接する同じ細胞の数を更新
        /// </summary>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        public void UpdateNextSame(CPMArea add = null, CPMArea remove = null)
        {
            int sameNum = 0;

            if (add != null) //細胞を追加する場合(addはこの細胞と同じではない)
            {
                add.NextFunc((c, v) =>
                {
                    if (c.cell == this)//隣の同じ細胞をカウントする
                    {
                        add.nextSame++; //隣のすべての細胞のnextSameを増やす
                        sameNum++;
                    }
                    return false;
                }, add.dim);

                add.nextSame = sameNum;
            }
            else if (remove != null) //細胞を削除する場合
            {
                remove.NextFunc((c, v) =>
                {
                    if (c.cell == this)//隣の同じ細胞をカウントする
                    {
                        remove.nextSame--; //隣のすべての細胞のnextSameを減らす
                    }
                    return false;
                }, remove.dim);
            }

        }

        /// <summary>
        /// 細胞の周長を更新(面の数で更新)
        /// </summary>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        public void CullL_nums(CPMArea area, CPMArea other, bool addOrRemove)
        {
            var der = 0; //増減

            if (addOrRemove) //細胞を追加する場合(addはこの細胞と同じではない)
            {
                var sameCellNum = 0;

                other.NextFunc((c, v) =>
                {
                    if (((CPMArea)c).cell == this)//同じ細胞の時
                    {
                        sameCellNum++;
                        //Console.WriteLine("c.nextSame : " + c.nextSame);
                        //c.CullNextSame(this); //隣のセルの同じ細胞の数を更新
                        //Console.WriteLine("c.nextSame : " + c.nextSame);
                        //Console.WriteLine("c.nextSame : " + c.nextSame);
                        if (((CPMArea)c).nextDiff == 1)
                        {
                            der -= 1; //自分以外の3つが隣接している場合、どのセルとも隣り合うので周長は減る
                        }
                    }
                    return false;
                }, other.dim);

                if (sameCellNum != 4) der += 1; //追加する細胞の分だけ増える

            }
            else //細胞を削除する場合
            {
                der -= 1; //追加する細胞の分だけ減る

                other.NextFunc((c, v) =>
                {
                    //Console.Write(c.cell.GetType() + "   ");
                    if (c.cell == this)//同じ細胞の時
                    {
                        //Console.Write(c.position.arrayPosition);
                        //Console.WriteLine("   c.nextDiff : " + c.nextDiff);
                        //c.CullNextSame(); //隣のセルの同じ細胞の数を更新
                        if (((CPMArea)c).nextDiff == 0)
                        {
                            der += 1; //すべての隣のセルが同じ細胞の場合、周長は増える
                            //Console.WriteLine(der);
                        }
                    }
                    return false;
                }, other.dim);
            }
            ///Console.WriteLine("der : " + der);
            /// 


            L += der;
        }

        /// <summary>
        ///     細胞の系のエネルギーを計算する
        /// <param name="add">この細胞が追加される前提で計算する(浸食する細胞と同じ)</param>
        /// <param name="remove">この細胞が削除される前提で計算する(浸食される細胞と同じ)</param>
        /// </summary>
        /// <returns>エネルギー量</returns>
        public virtual float CullEnergy(CPMArea add = null, CPMArea remove = null)
        {
            CullA(add, remove);
            //CullL(add, remove);
            //CullL_nums(add, remove);

            nowEnergy = CullEnergy(A, L);

            //Console.WriteLine("nowEnergy : " + nowEnergy);
            //Console.WriteLine("A : " + A);
            //Console.WriteLine("L : " + L);
            return nowEnergy;
        }

        /// <summary>
        ///  ある細胞に対する細胞の接着力を計算する（デフォルトは1）
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual float CullAdhesionFactor(CPMArea cell)
        {
            return kAdhesion * cell.nextDiff;
        }



        /// <summary>
        /// エネルギーを計算する関数
        /// </summary>
        /// <param name="A"></param>
        /// <param name="L"></param>
        /// <returns></returns>
        public virtual float CullEnergy(int A, int L)
        {
            var da = A - A0; //1固定
            var dl = L - L0; //-3 ~ 3
            return kA * da * da + kL * dl * dl; //-2 ~ 4
        }

        /// <summary>
        /// 面積と周長、エネルギーを前の値に戻す
        /// </summary>
        public virtual void BackPreSwap()
        {
            A = preA;
            L = preL;
            nowEnergy = preEnergy;
            //nowAEnergy = preAEnergy;
            //nowLEnergy = preLEnergy;
        }

        public virtual void OnSwaped(CPMArea otherArea)
        {
        }

        /// <summary>
        ///     セルの色をランダムに設定
        /// </summary>
        public virtual void SetRandomColor()
        {
            Random rand = new Random();
            int r = rand.Next(256);
            int g = rand.Next(256);
            int b = rand.Next(256);
            color = Color.FromArgb(r, g, b);
        }
    }
}