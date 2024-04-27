namespace CPMBase.Base;

/// <summary>
///     毎時間更新される処理を管理するためのインターフェース
/// </summary>
public interface IUpdatable
{
    /// <summary>
    ///     毎時間更新される処理
    /// </summary>
    /// <param name="stepUpdater"></param>

    /// <summary>
    /// シミュレーション開始時に１度だけ呼び出される
    /// </summary>
    /// <param name="stepUpdater"></param>
    public void Init(StepUpdater stepUpdater);


    /// <summary>
    /// 毎時間更新される処理
    /// </summary>
    public void Update(StepUpdater stepUpdater);
}
