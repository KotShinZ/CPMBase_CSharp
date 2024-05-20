using System.Drawing;
using SkiaSharp;
using System;
using System.Diagnostics;
using ScottPlot;
using System.Numerics;

namespace CPMBase;

public class PathObject
{
    /// <summary>
    ///  ファイルのパス 例) "C:/Users/username/Documents" 例(相対パス) "/Users/username/Documents"
    /// </summary>
    public string path;

    /// <summary>
    ///  ファイルの名前 例) "test.txt"
    /// </summary>//  
    public string name;
    public string prename = "";
    public string extention = "";

    public string fullPath => path + "/" + prename + name + extention;

    public bool directoryCreated = false;

    public Vector2 resolution = new Vector2(512, 256);

    /// <summary>
    /// 例　new PathObject("/workspaces/CPMBase_CSharp/Output/MicroExample", "image", extention: ".png");
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="prename"></param>
    /// <param name="extention"></param>
    public PathObject(string path, string name, string prename = "", string extention = "")
    {
        this.path = path;
        this.name = name;
        this.prename = prename;
        this.extention = extention;
        CreateDirectory(path);
    }

    public void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            directoryCreated = true;
        }
    }

    public void Write(object obj, string path = null)
    {
        path = path ?? fullPath;

        switch (obj)
        {
            case Plot plt:
                Plot(plt);
                break;
            case string str:
                WriteToTextFile(str);
                break;
            case Bitmap bmp:
                WriteToImgFile(bmp);
                break;
            case SKBitmap skbmp:
                WriteToImgFile(skbmp);
                break;
            default:
                throw new NotImplementedException("Object type not supported");
        }
    }

    /// <summary>
    /// 指定されたパスにテキストファイルを出力します。必要なディレクトリが存在しない場合は作成します。
    /// </summary>
    /// <param name="filePath">出力するファイルのフルパス。</param>
    /// <param name="content">ファイルに書き込む内容。</param>
    public void WriteToTextFile(string content)
    {
        //Console.WriteLine(content);
        //Console.WriteLine(fullPath);
        // ファイルに内容を書き込み
        File.WriteAllText(fullPath, content);
    }

    public void WriteToImgFile(Bitmap content)
    {
        // ファイルに内容を書き込み
        content.Save(fullPath);
    }

    public void WriteToImgFile(SKBitmap content)
    {
        using (var image = SKImage.FromBitmap(content))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        using (var stream = File.OpenWrite(fullPath))
        {
            //Console.WriteLine("Writing to " + fullPath);
            data.SaveTo(stream);
        }
    }

    public void Plot(Plot plot)
    {
        plot.SavePng(fullPath, (int)resolution.X, (int)resolution.Y);
    }

}
