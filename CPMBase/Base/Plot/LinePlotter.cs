namespace CPMBase;

using Microsoft.VisualBasic;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Numerics;

public class LinePlotter
{
    public List<double> xData;
    public List<double> yData;
    private Plot plt;

    public PathObject pathObject;

    public LinePlotter()
    {
        xData = new List<double>();
        yData = new List<double>();
        plt = new Plot();
    }

    public void AddData(double value1, double value2)
    {
        xData.Add(value1);
        yData.Add(value2);
    }

    public void Plot(PathObject pathObject, string title = "Line Plot", string xLabel = "Index", string yLabel = "Values")
    {
        this.pathObject = pathObject;

        if (xData.Count == 0)
        {
            for (int i = 0; i < yData.Count; i++)
            {
                xData.Add(i);
            }
        }

        plt.Clear();
        plt.Add.Scatter(xData.ToArray(), yData.ToArray());
        plt.Title(title);
        plt.XLabel(xLabel);
        plt.YLabel(yLabel);

        pathObject.Write(plt);
        Console.WriteLine("Plot updated and saved as plot.png");
    }
}
