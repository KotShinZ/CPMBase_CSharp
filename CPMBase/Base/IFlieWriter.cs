namespace CPMBase;

public interface IFlieWriter
{
    public string path { get; set; }

    public string name { get; set; }

    public string extention { get; set; }

    public string prename { get; set; }

    public string fullPath => path + "/" + prename + name + extention;

    public Action OnWrite { get; set; } //ファイルを書き込む処理を管理するためのデリゲート

    public bool isWrite { get; set; } //ファイルを書き込むかどうか

    public void StartWrite()
    {
        if (isWrite)
        {
            OnWrite?.Invoke();
            Write();
        }
    }

    protected void Write();
}
