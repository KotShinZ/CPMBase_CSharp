using System.Data.Common;
using System.Drawing;
using System.IO.Compression;
using System.Numerics;
using CPMBase.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;

using LinkedList = CPMBase.Base.Datas.LinkedList<CPMBase.CPMArea>;
using Microsoft.Extensions.Caching.Distributed;

namespace CPMBase.CPM
{

	public class CPM_Base : IUpdatable, ITimePathWrite
	{
		public Cell[] cells = new Cell[1000];
		private int cellNum = 0; //現在の細胞の数
		private int maxCellNum => cells.Length;//細胞の最大数

		private Dimention dim => cellAreaArray.dim;
		public CellAreaArray cellAreaArray;

		public Vector3 cellInitSize = new Vector3(10, 10, 1); //細胞の初期サイズ

		public double updateNumPerSecondPerAreaPerMeter2 = 1; //1秒間に何回更新するか

		public RangePosition range; //シミュレーション領域の大きさ

		public float T = 23; //ボルツマン温度

		public int writeResolusion = 512; //書き出す画像の解像度

		public LinkedList areasSortedActivty = new LinkedList();

		public static float sumActivity;

		/// <summary>
		/// CPMを作成
		/// </summary>
		/// <param name="size">シミュレーション領域の大きさ</param>
		/// <param name="dV">シミュレーションの細かさ</param>
		/// <param name="dim">次元</param>
		/*public CPM_Base(RangePosition size, CPMArea area, Dimention dim = Dimention._3d)
		{
			var _area = area ?? new CPMArea(dim);
			cellAreaArray = new CellAreaArray(size, _area, dim);
			range = size;
		}*/

		public CPM_Base(RangePosition size, Func<CPMArea> func = null, Dimention dim = Dimention._3d)
		{
			var _func = func ?? new Func<CPMArea>(() => new CPMArea(dim, cellAreaArray: cellAreaArray));
			cellAreaArray = new CellAreaArray(size, _func, dim);
			range = size;
			cellAreaArray.AllFunc(c =>
				{
					areasSortedActivty.AddFirst((CPMArea)c);
					((CPMArea)c).node = areasSortedActivty.Head;
				}
			);
		}

		/// <summary>
		/// 細胞を一つ追加する
		/// </summary>
		/// <param name="cell">細胞</param>
		/// <param name="center">追加する中心の場所</param>
		/// <param name="initSize">大きさ</param> <summary>
		/// 
		/// </summary>
		public void Add(Cell cell, RangePosition cellarea)
		{
			if (cellNum >= maxCellNum)
			{
				Console.WriteLine("細胞の数が最大数を超えたため追加できません");
				return; //細胞の数が最大数を超えたら追加しない
			}


			cells[cellNum] = cell; //細胞を追加

			cellAreaArray.AreaFunc(cellarea, (c) =>
			{
				((CPMArea)c).cell = cell;
			}); //細胞の領域に細胞を追加


			cellNum++;
		}

		public void Add(Cell cell, Position center, Position size = null)
		{
			var _size = size ?? new Position(cellInitSize);
			Add(cell, new RangePosition(center.arrayPosition.X + _size.arrayPosition.X / 2, center.arrayPosition.X - _size.arrayPosition.X / 2, center.arrayPosition.Y + _size.arrayPosition.Y / 2, center.arrayPosition.Y - _size.arrayPosition.Y / 2, center.arrayPosition.Z + _size.arrayPosition.Z / 2, center.arrayPosition.Z - _size.arrayPosition.Z / 2));
		}

		public void Add(Cell cell, Vector3 center, Vector3 size)
		{
			Add(cell, new Position(center), new Position(size));
		}

		/// <summary>
		/// rangeの範囲のランダムな位置に細胞を追加する
		/// </summary>
		/// <param name="cell">細胞</param>
		/// <param name="range">追加する範囲</param>
		/// <param name="num">追加する数</param>
		public void AddArea(Func<Cell> cell, RangePosition range, int num, Position cellSize = null)
		{
			for (int i = 0; i < num; i++)
			{
				var point = range.GetRandomPosition();
				Add(cell.Invoke(), point, cellSize);
			}
		}

		public void Remove(int index)
		{

		}

		/// <summary>
		///  シミュレーションの開始時に呼び出される
		/// </summary>
		/// <param name="stepUpdater"></param>
		public void Init(StepUpdater stepUpdater)
		{
			cellAreaArray.AllFunc(c => ((CPMArea)c).CullNextSame());  //nextSameを計算
			CullAllCellEnergy(); //各細胞のエネルギー、面積、周長を計算
		}

		/// <summary>
		/// セルとそのセルのエリアを取得
		/// </summary>
		/// <returns></returns>
		public Dictionary<Cell, List<CPMArea>> GetCellAreaList()
		{
			Dictionary<Cell, List<CPMArea>> dict = new();
			// 各細胞のエリアを整理
			cellAreaArray.AllFunc(c =>
				{
					if (dict.ContainsKey(((CPMArea)c).cell))
					{
						dict[((CPMArea)c).cell].Add((CPMArea)c);
					}
					else
					{
						dict[((CPMArea)c).cell] = new List<CPMArea>() { (CPMArea)c };
					}
				}
				);
			return dict;
		}


		/// <summary>
		/// 各細胞のエネルギーを一から計算
		/// </summary>
		public void CullAllCellEnergy()
		{
			var dict = GetCellAreaList();

			foreach (var cell in dict.Keys)
			{
				int L = 0;
				foreach (var area in dict[cell])
				{
					//L += area.nextDiff; //隣接する異なる細胞の数を計算
					L += area.nextDiff > 0 ? 1 : 0;
				}
				int A = dict[cell].Count; //細胞の面積を計算

				cell.nowEnergy = cell.CullEnergy(A, L); //エネルギーを計算
				cell.A = A;
				cell.A = A;
				cell.L = L;
				cell.L = L;
			}
		}

		/// <summary>
		///  ランダムに細胞を選択
		/// </summary>
		/// <param name="stepUpdater"></param>
		/// <returns></returns> <summary>
		public CPMArea GetRandomCellWithActivty(StepUpdater stepUpdater)
		{
			//return (CPMArea)cellAreaArray.GetRandomCell();

			int index = 0;
			while (true)
			{
				var dist = stepUpdater.random.NextDouble() * sumActivity;

			}
		}

		/// <summary>
		/// dt * V * k の数だけcellular potts modelを更新する (kは定数)
		/// </summary>
		/// <param name="stepUpdater"></param>
		public void Update(StepUpdater stepUpdater)
		{
			//var num = (decimal)updateNumPerSecondPerAreaPerMeter2 * (decimal)stepUpdater.dt * range.volume;
			var num = updateNumPerSecondPerAreaPerMeter2;
			for (int i = 0; i < num; i++)
			{
				while (true)
				{
					var denyNum = 0;
					//var area = GetRandomCell(stepUpdater);  //ランダムな細胞を選択
					var area = (CPMArea)cellAreaArray.GetRandomCell();  //ランダムな細胞を選択

					if (!IsNextToOtherCell(area, out CPMArea otherArea, out Direction direction))
					{
						denyNum += 1;
						if (denyNum < 1000000) { continue; }
					}; //更新する細胞がない場合は次の細胞を選択

					var dH = GetDH(area, otherArea, direction); //エネルギー差を計算

					var p = Math.Exp(-dH / T); //ボルツマン因子
					if (dH < 0 || stepUpdater.random.NextDouble() < p) //エネルギー差が負の場合
					{
						otherArea.SwapCell(area.cell, area); //細胞を入れ替える
						if (otherArea.cell is not EmptyCell) otherArea.activity = 1; //活動量を1にする
						AreaToFirstWithActivity(otherArea); //活動量が大きい順に並び替え(要素を一番最初にするだけ)
						break;
					}
					else
					{
						//細胞を入れ替えない
						area.cell.BackPreSwap(); //細胞を入れ替える前の状態に戻す
						otherArea.cell.BackPreSwap(); //細胞を入れ替える前の状態に戻す
						otherArea.NextFunc((c, d) => { c.nextSame = c.preNextSame; return false; }, dim); //細胞を入れ替える前の状態に戻す
					}
					break;
				}
				cellAreaArray.AllFunc(c => ((CPMArea)c).UpdateActivity());  //nextSameを計算
			}
		}

		public void AreaToFirstWithActivity(CPMArea area)
		{
			areasSortedActivty.ToFirst(area.node);
		}

		public void SetActivity(CPMArea area, int num, float mul = 1)
		{
			if (area.cell is not EmptyCell) area.activity = mul; //活動量を1にする
			if (num > 0)
			{
				area.NextFunc((c, d) =>
				{
					if (c.cell is not EmptyCell) SetActivity(c, num - 1, mul * mul);
					return false;
				}, dim); //nextSameを計算
			}
		}

		/// <summary>
		/// dHを計算
		/// </summary>
		/// <param name="area"></param>
		/// <param name="otherArea"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public float GetDH(CPMArea area, CPMArea otherArea, Direction direction)
		{
			var cell = area.cell; //細胞 (浸食する側)
			var otherCell = otherArea.cell; //隣の細胞　(浸食される側）

			var pre = cell.nowEnergy + otherCell.nowEnergy; //変更前の細胞の合計のエネルギー(sum)

			var now = cell.CullEnergy(add: otherArea) + otherCell.CullEnergy(remove: otherArea); //変更後の細胞の合計のエネルギー(sum)

			var dH = now - pre; //細胞のエネルギー差

			if (cell.kAdhesion != 0)
				dH += cell.CullAdhesionFactor((CPMArea)area); //細胞のエネルギー差に接着因子を加える

			if (cell.constantPower != Vector3.Zero)
				dH += Vector3.Dot(cell.constantPower * -1, DirectionHelper.GetVector(direction)); //定数の力を加える

			dH += -1 * cell.lact * (area.GetNextActivity() - otherArea.GetNextActivity());

			/*Console.WriteLine("pre : " + pre);
			Console.WriteLine("now : " + now);
			Console.WriteLine("A : " + cell.A + "  L : " + cell.L + "   otherA : " + otherCell.A + "  otherL : " + otherCell.L);
			Console.WriteLine("preA : " + cell.preA + "  preL : " + cell.preL + "   preotherA : " + otherCell.preA + "  preotherL : " + otherCell.preL);
			Console.WriteLine("\n");*/

			return dH;
		}

		/// <summary>
		/// 隣に違う細胞があるかどうか
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		public virtual bool IsNextToOtherCell(CPMArea cell, out CPMArea otherCell, out Direction direction)
		{
			List<CPMArea> list = new List<CPMArea>();

			direction = default;
			Direction direct = default;

			cell.NextFunc((other, dir) =>
			{
				var next = ((CPMArea)other).cell != cell.cell; //違う細胞が隣にあるかどうか
				if (next)
				{
					list.Add((CPMArea)other);
					direct = dir;
					return true;
				}
				return false;
			}, dim);

			direction = direct; //違う細胞がある方向

			if (list.Count > 0) { otherCell = list.GetRandom(); }
			else { otherCell = null; }
			return list.Count > 0;
		}

		///Write
		/// <summary>
		///  CPMの画像をビットマップに書き込む
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="z"></param>
		public void WriteImage(Bitmap bitmap, int z = 0)
		{
			// 各ピクセルに対してランダムな色を割り当て
			var range = cellAreaArray.size;
			cellAreaArray.AreaFunc(range, (x, y, z, c) => bitmap.SetPixel(x, (int)(range.arrayRange.y.Length - y), ((CPMArea)c).cell.color), z2d: z);
		}

		public void WriteImage(SKBitmap bitmap, int z = 0)
		{
			// 各ピクセルに対してランダムな色を割り当て
			var range = cellAreaArray.size;

			cellAreaArray.AreaFunc(range, (x, y, z, c) =>
			{
				bitmap.SetPixel(x, (int)(range.arrayRange.y.Length - y - 1), ConvertToSKColor(((CPMArea)c).cell.color));
			}, z2d: z);
		}

		public void WriteImageActivty(SKBitmap bitmap, int z = 0)
		{
			// 各ピクセルに対してランダムな色を割り当て
			var range = cellAreaArray.size;

			cellAreaArray.AreaFunc(range, (x, y, z, c) =>
			{
				var cellarea = ((CPMArea)c);
				SKColor color = cellarea.activity > 0 ? new SKColor((byte)(MathF.Min(cellarea.activity, 1) * 255), 0, 0) : ConvertToSKColor(((CPMArea)c).cell.color);
				bitmap.SetPixel(x, (int)(range.arrayRange.y.Length - y - 1), color);
			}, z2d: z);
		}

		/// <summary>
		/// CPMの画像をビットマップに書き込む(補完あり、解像度固定)
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="z"></param>
		public void WriteImageInterpolate(SKBitmap bitmap, int z = 0)
		{
			// 各ピクセルに対してランダムな色を割り当て
			var range = cellAreaArray.size;

			var height = bitmap.Height;
			var width = bitmap.Width;

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var x2 = (int)(x * range.arrayRange.x.Length / width); //補完するための座標
					var y2 = (int)(y * range.arrayRange.y.Length / height); //補完するための座標
					var cellarea = (((CPMArea)cellAreaArray.GetCellArea(x2, y2, z)));
					SKColor color = ConvertToSKColor(cellarea.cell.color);
					bitmap.SetPixel(x, y, color);
				}
			}
		}

		public void WriteImageInterpolateActivty(SKBitmap bitmap, int z = 0)
		{
			// 各ピクセルに対してランダムな色を割り当て
			var range = cellAreaArray.size;

			var height = bitmap.Height;
			var width = bitmap.Width;

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var x2 = (int)(x * range.arrayRange.x.Length / width); //補完するための座標
					var y2 = (int)(y * range.arrayRange.y.Length / height); //補完するための座標
					var cellarea = (((CPMArea)cellAreaArray.GetCellArea(x2, y2, z)));
					SKColor color = new SKColor((byte)(MathF.Min(cellarea.activity, 1) * 255), 0, 0);
					bitmap.SetPixel(x, y, color);
				}
			}
		}

		public SKColor ConvertToSKColor(Color color)
		{
			return new SKColor(color.R, color.G, color.B, color.A);
		}

		public void Write(float time, int step, PathObject path)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var bitmap = new Bitmap((int)range.arrayRange.x.Length, (int)range.arrayRange.y.Length);
				WriteImage(bitmap); // CPMの画像をビットマップに書き込む
				path.WriteToImgFile(bitmap); //ビットマップを書き出す
			}
			else
			{
				WriteSK(time, step, path);
			}

		}

		public void WriteSK(float time, int step, PathObject path, Vector2 writeResolusion = default)
		{
			//var resolusion = writeResolusion == default ? new Vector2((float)range.arrayRange.x.Length, (float)range.arrayRange.y.Length) : writeResolusion;
			var resolusion = new Vector2(256, 256);
			var bitmap = new SKBitmap((int)resolusion.X, (int)resolusion.Y);
			WriteImageInterpolate(bitmap); // CPMの画像をビットマップに書き込む
			path.WriteToImgFile(bitmap); //ビットマップを書き出す
		}

		public void WriteSKActivty(float time, int step, PathObject path, Vector2 writeResolusion = default)
		{
			//var resolusion = writeResolusion == default ? new Vector2((float)range.arrayRange.x.Length, (float)range.arrayRange.y.Length) : writeResolusion;
			var resolusion = new Vector2(256, 256);
			var bitmap = new SKBitmap((int)resolusion.X, (int)resolusion.Y);
			WriteImageInterpolateActivty(bitmap); // CPMの画像をビットマップに書き込む
			path.WriteToImgFile(bitmap); //ビットマップを書き出す
		}

		public void WriteAsJson(StepUpdater stepUpdater, PathObject path, string name = "JsonData")
		{
			path.WriteToTextFile(path.path + stepUpdater.stepNum + name + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
		}
	}
}
