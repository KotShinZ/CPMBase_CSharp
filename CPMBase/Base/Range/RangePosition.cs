using System.Numerics;
using SkiaSharp;

namespace CPMBase;

/// <summary>
///   配列の位置と実際の位置を持つ範囲
/// </summary>
public class RangePosition
{
    public decimal volume;

    public Range3 arrayRange;
    public Range3 realRange;

    public decimal surface;

    public double arraySurface;
    public double realSurface;

    public Vector3 dV;

    public double arrayVolume;
    public double realVolume;

    public RangePosition(Range3 arrayRange, Range3 realRange)
    {
        this.arrayRange = arrayRange;
        this.realRange = realRange;
        this.dV = new Vector3([
            (float)(realRange.x.Length / arrayRange.x.Length),
            (float)(realRange.y.Length / arrayRange.y.Length),
            (float)(realRange.z.Length / arrayRange.z.Length)
        ]);
        Init();
    }

    public RangePosition(Range3 range, Vector3 dV, bool isArrayOrReal)
    {
        if (isArrayOrReal)
        {
            this.arrayRange = range;
            this.realRange = range * dV;
            this.dV = dV;
        }
        else
        {
            this.realRange = range;
            this.arrayRange = new Range3(
                range.x.min / dV[0],
                range.x.max / dV[0],
                range.y.min / dV[1],
                range.y.max / dV[1],
                range.z.min / dV[2],
                range.z.max / dV[2]
            );
            this.dV = dV;
        }
    }

    public RangePosition(double arrayMaxX, double arrayMaxY, double arrayMaxZ, bool isCenter = false)
    {
        this.arrayRange = new Range3(arrayMaxX, arrayMaxY, arrayMaxZ, isCenter);
        this.realRange = new Range3(arrayMaxX, arrayMaxY, arrayMaxZ, isCenter);
        Init();
    }

    public RangePosition(double arrayMaxX = 1, double arrayMinX = 0, double arrayMaxY = 1, double arrayMinY = 0, double arrayMaxZ = 1, double arrayMinZ = 0)
    {
        this.arrayRange = new Range3(arrayMaxX, arrayMinX, arrayMaxY, arrayMinY, arrayMaxZ, arrayMinZ);
        this.realRange = new Range3(arrayMaxX, arrayMinX, arrayMaxY, arrayMinY, arrayMaxZ, arrayMinZ);
        Init();
    }

    public RangePosition(Vector3 arrayRange, Vector3 realRange, bool isCenter)
    {
        if (isCenter)
        {
            this.arrayRange = new Range3(arrayRange);
            this.realRange = new Range3(arrayRange * realRange - arrayRange * realRange / 2);
            this.dV = new Vector3([
                    (float)(realRange.X / arrayRange.X),
                    (float)(realRange.Y / arrayRange.Y),
                    (float)(realRange.Z / arrayRange.Z)
                ]);
        }
        else
        {
            this.arrayRange = new Range3(arrayRange);
            this.realRange = new Range3(realRange);
            this.dV = new Vector3([
                    (float)(realRange.X / arrayRange.X),
                    (float)(realRange.Y / arrayRange.Y),
                    (float)(realRange.Z / arrayRange.Z)
                ]);
        }
        Init();
    }

    public RangePosition(Vector3 arrayRange, bool isCenter) : this(arrayRange, arrayRange, isCenter) { }

    /// <summary>
    ///  配列の大きさを指定して作成
    ///  配列の大きさと実際の大きさが同じ
    ///  実際の大きさは1要素の1辺が1m
    /// </summary>
    /// <param name="arrayRange"></param>
    public RangePosition(Vector3 arrayRange) : this(arrayRange, arrayRange, Vector3.One) { }

    public RangePosition(Vector3 arrayRange, Vector3 realRange, Vector3 dV)
    {
        this.arrayRange = new Range3(arrayRange);
        this.realRange = new Range3(realRange);
        this.dV = dV;
        Init();
    }

    public virtual void Init()
    {
        arrayVolume = arrayRange.x.Length * arrayRange.y.Length * arrayRange.z.Length;
        realVolume = realRange.x.Length * realRange.y.Length * realRange.z.Length;
        volume = (decimal)arrayRange.x.Length * (decimal)arrayRange.y.Length * (decimal)arrayRange.z.Length;

        surface = (decimal)arrayRange.x.Length * (decimal)arrayRange.y.Length;
        arraySurface = arrayRange.x.Length * arrayRange.y.Length;
        realSurface = realRange.x.Length * realRange.y.Length;
    }

    public RangePosition AddArray(Vector3 value)
    {
        arrayRange.x.Add(value.X);
        arrayRange.y.Add(value.Y);
        arrayRange.z.Add(value.Z);
        realRange.x.Add(value.X * dV[0]);
        realRange.y.Add(value.Y * dV[1]);
        realRange.z.Add(value.Z * dV[2]);

        return this;
    }

    public RangePosition AddReal(Vector3 value)
    {
        realRange.x.Add(value.X);
        realRange.y.Add(value.Y);
        realRange.z.Add(value.Z);
        arrayRange.x.Add(value.X / dV[0]);
        arrayRange.y.Add(value.Y / dV[1]);
        arrayRange.z.Add(value.Z / dV[2]);

        return this;
    }

    /// <summary>
    ///    配列の位置を実際の位置に変換
    /// </summary>
    /// <param name="arrayPosition"></param>
    /// <returns></returns>
    /*public Vector3 GetRealPosition(Vector3 arrayPosition)
    {
        return new Vector3(
            arrayPosition.X * dV[0] + realRange.x.min,
            arrayPosition.Y * dV[1] + realRange.y.min,
            arrayPosition.Z * dV[2] + realRange.z.min
        );
    }

    /// <summary>
    ///  実際の位置を配列の位置に変換
    /// </summary>
    /// <param name="realPosition"></param>
    /// <returns></returns>
    public Vector3 GetArrayPosition(Vector3 realPosition)
    {
        return new Vector3(
            (float)((realPosition.X - realRange.x.min) / dV.X),
            (float)((realPosition.Y - realRange.y.min) / dV.Y),
            (float)((realPosition.Z - realRange.z.min) / dV.Z)
        );
    }*/



    /// <summary>
    ///    配列の位置を実際の位置に変換
    /// </summary>
    /// <param name="arrayPosition"></param>
    /// <returns></returns>
    public Vector3 GetRealPosition(Vector3 arrayPosition)
    {
        return new Vector3(
            (float)(realRange.x.min + dV[0] * arrayPosition.X),
            (float)(realRange.y.min + dV[1] * arrayPosition.Y),
            (float)(realRange.z.min + dV[2] * arrayPosition.Z)
        );
    }

    /// <summary>
    ///  実際の位置を配列の位置に変換
    /// </summary>
    /// <param name="realPosition"></param>
    /// <returns></returns>
    public Vector3 GetArrayPosition(Vector3 realPosition)
    {
        return new Vector3(
            (float)((realPosition.X - realRange.x.min) / dV[0]),
            (float)((realPosition.Y - realRange.y.min) / dV[1]),
            (float)((realPosition.Z - realRange.z.min) / dV[2])
        );
    }

    public Position GetRandomPosition()
    {
        return new Position(GetRandomArray(), new Vector3((float)dV.X, (float)dV.Y, (float)dV.Z), new Range3(realRange.x.max, realRange.x.min, realRange.y.max, realRange.y.min, realRange.z.max, realRange.z.min));
    }

    public Vector3 GetRandomArray()
    {
        return new Vector3((float)arrayRange.x.GetRandom(), (float)arrayRange.y.GetRandom(), (float)arrayRange.z.GetRandom());
    }

    public Vector3 GetRandomReal()
    {
        return new Vector3((float)realRange.x.GetRandom(), (float)realRange.y.GetRandom(), (float)realRange.z.GetRandom());
    }

    public bool isContain(Vector3 value, bool equal = false)
    {
        return arrayRange.Contains(value);
    }

    public bool isContain(RangePosition range, bool equal = false)
    {
        return arrayRange.Contains(range.arrayRange);
    }

    public override string ToString()
    {
        return "ArrayRange: " + arrayRange + "\nRealRange: " + realRange;
    }

    public void DrawAxis(SKCanvas canvas, int width, int height)
    {
        // 軸と目盛りの描画設定
        var axisPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 2,
            IsAntialias = true
        };

        var tickMarkPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 1,
            IsAntialias = true
        };

        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            TextSize = 14
        };

        // x軸とy軸を描画
        int xAxisYPosition = height - 50; // x軸のy位置を下部に設定
        int yAxisXPosition = 50; // y軸のx位置を左側に設定
        canvas.DrawLine(0, xAxisYPosition, width, xAxisYPosition, axisPaint); // x軸
        canvas.DrawLine(yAxisXPosition, 0, yAxisXPosition, height, axisPaint); // y軸

        // 目盛りの数
        int tickCount = 10;

        // x軸の目盛りとラベル
        float xInterval = (width - yAxisXPosition) / (float)tickCount;
        for (int i = 0; i <= tickCount; i++)
        {
            float xPos = yAxisXPosition + i * xInterval;
            canvas.DrawLine(xPos, xAxisYPosition, xPos, xAxisYPosition + 10, tickMarkPaint);
            var label = ((realRange.x.max - realRange.x.min) / tickCount * i + realRange.x.min).ToString("0");
            canvas.DrawText(label, xPos - textPaint.TextSize / 2, xAxisYPosition + 25, textPaint);
        }

        // y軸の目盛りとラベル
        float yInterval = (xAxisYPosition) / (float)tickCount;
        for (int i = 0; i <= tickCount; i++)
        {
            float yPos = xAxisYPosition - i * yInterval;
            canvas.DrawLine(yAxisXPosition - 10, yPos, yAxisXPosition, yPos, tickMarkPaint);
            var label = ((realRange.y.max - realRange.y.min) / tickCount * i + realRange.y.min).ToString("0");
            canvas.DrawText(label, yAxisXPosition - 35, yPos + textPaint.TextSize / 3, textPaint);
        }
    }
}