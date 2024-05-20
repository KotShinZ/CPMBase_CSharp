using System.Numerics;

namespace CPMBase;

public class EventListener<T>
{
    public Dictionary<string, Action<T>> events = new();

    public void Add(string eventName, Action<T> action)
    {
        if (events == null)
        {
            events = new Dictionary<string, Action<T>>();
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName] += action;
        }
        else
        {
            events[eventName] = action;
        }
    }

    public void Remove(string eventName, Action<T> action)
    {
        if (events == null)
        {
            return;
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName] -= action;
        }
    }

    public void Invoke(string eventName, T value)
    {
        if (events == null)
        {
            return;
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName](value);
        }
    }
}

public class EventListener
{
    public Dictionary<string, Action> events = new();

    public void Add(string eventName, Action action)
    {
        if (events == null)
        {
            events = new Dictionary<string, Action>();
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName] += action;
        }
        else
        {
            events[eventName] = action;
        }
    }

    public void Remove(string eventName, Action action)
    {
        if (events == null)
        {
            return;
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName] -= action;
        }
    }

    public void Invoke(string eventName)
    {
        if (events == null)
        {
            return;
        }

        if (events.ContainsKey(eventName))
        {
            events[eventName]();
        }
    }
}

public class EventListenerInt : EventListener<int>
{

}

public class EventListenerFloat : EventListener<float>
{

}

public class EventListenerString : EventListener<string>
{

}

public class EventListenerBool : EventListener<bool>
{

}

public class EventListenerVector2 : EventListener<Vector2>
{

}

public class EventListenerVector3 : EventListener<Vector3>
{

}


