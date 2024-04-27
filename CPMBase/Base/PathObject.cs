using System.Drawing;
using SkiaSharp;
using System;
using System.Diagnostics;

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

    /// <summary>
    /// 指定されたパスにテキストファイルを出力します。必要なディレクトリが存在しない場合は作成します。
    /// </summary>
    /// <param name="filePath">出力するファイルのフルパス。</param>
    /// <param name="content">ファイルに書き込む内容。</param>
    public void WriteToTextFile(string filePath, string content)
    {
        // ファイルに内容を書き込み
        File.WriteAllText(filePath, content);
    }

    public void WriteToTextFile(string content)
    {
        WriteToTextFile(fullPath, content);
    }

    public void WriteToImgFile(string filePath, Bitmap content)
    {
        // ファイルに内容を書き込み
        content.Save(filePath);
    }

    public void WriteToImgFile(Bitmap content)
    {
        WriteToImgFile(fullPath, content);
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
}
