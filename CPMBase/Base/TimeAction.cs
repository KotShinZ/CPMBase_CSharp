using System.Xml;

namespace CPMBase;

public class TimeAction
{
    public Action action;
    public float duration; 

    public float now; 

    public TimeAction(Action action, float duration)
    {
        this.action = action;
        this.duration = duration;
    }   

    public void ActionInvoke()
    {
        action();
    }

    public void Invoke(float dt = 1)
    {
        if (now > duration)
        {
            ActionInvoke();
            now = 0;
        }
        now += dt;
    }
}
