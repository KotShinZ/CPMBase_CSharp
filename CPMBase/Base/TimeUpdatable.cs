using System.Net.NetworkInformation;
using CPMBase.Base;

namespace CPMBase;

public abstract class TimeUpdatable : IUpdatable
{
    public double dt;
    public double nowTime
    {
        get => _nowTime;
        set
        {
            preTime = _nowTime;
            _nowTime = value;
        }
    }

    private double _nowTime;

    public double preTime;

    public double endTime;

    public bool isEnd = false;

    public bool timeNext => MathF.Ceiling((float)nowTime) == MathF.Ceiling((float)preTime); //次の時間に移行したかどうか

    public Action OnEnd;

    public void Init(StepUpdater stepUpdater) { Init(); }

    public void End(StepUpdater stepUpdater) { End(); }

    public void Update(StepUpdater stepUpdater)
    {
        if (nowTime >= endTime)
        {
            if (isEnd == false) OnEnd?.Invoke();
            isEnd = true;
            return;
        }
        nowTime += dt;
        OnUpdate(stepUpdater);

    }

    public virtual void Init() { }

    public virtual void OnUpdate(StepUpdater updater) { }

    public virtual void End() { }

    public void Run()
    {
        Init();
        while (nowTime < endTime)
        {
            OnUpdate(null);
        }
        End();
    }

    public void Reset()
    {
        nowTime = 0;
        isEnd = false;
    }
}
