using System.Numerics;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using CPMBase.Base;

namespace CPMBase;

/// <summary>
/// 四角形の領域
/// パラメータを持つ
/// デフォルトでは1Array1m
/// </summary>

[Serializable]
public class CellArea
{
    public Position position;

	[JsonIgnore]
	public CellAreaArray parent;

	[JsonIgnore]
	public CellArea[] nextAreas;


	public CellArea(Position position = null, CellAreaArray parent = null)
	{
		this.position = position;
		this.parent = parent;
	}

	/// <summary>
	///  隣のエリアを保存
	/// </summary>
	public void SetInitNextAreas()
	{
		nextAreas = new CellArea[6];
		int i = 0;

		parent.InitNextFunc(this, (c, d) =>
		{
			nextAreas[i] = c;
			i++;
			return false;
		}, parent.dim);
	}

	/// <summary>
	///  隣のエリアに対して関数を適用
	/// </summary>
	/// <param name="func"></param>
	/// <param name="dim"></param>
	public void NextFunc(Func<CellArea, Direction, bool> func, Dimention dim)
	{
		for (int i = 0; i < (int)dim * 2; i++)
		{
			if (nextAreas[i] != null)
				if (func(nextAreas[i], (Direction)i)) break;
		}
	}

	/// <summary>
	///  ムーア近傍に対して関数を適用
	/// </summary>
	/// <param name="func"></param>
	/// <param name="dim"></param>
	public void MooreNextFunc(Func<CellArea, Vector3, bool> func, Dimention dim)
	{
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (dim == Dimention._3d)
				{
					for (int z = -1; z < 2; z++)
					{
						if (x == 0 && y == 0 && z == 0) continue;
						var direction = new Vector3(x, y, z);
						if (func(parent.GetCellArea(position), direction)) break;
					}
				}
				else
				{
					if (x == 0 && y == 0) continue;
					var direction = new Vector3(x, y, 0);
					if (func(parent.GetCellArea(position), direction)) break;
				}

			}
		}

	}

	/// <summary>
	///  隣のエリアを取得
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public CellArea GetCellArea(Direction direction)
	{
		return GetNextArea((int)direction);
	}

	public CellArea GetNextArea(int i)
	{
		return nextAreas[i];
	}
}
