using CPMBase;

public class Floats
{
    protected Deque<float> queue;

    public int Count => queue.Count; //キューの要素数
    public float value //デフォルトの値
    {
        get => queue.PeekFront();
        set => queue.SetFront(value);
    }

    public float dF => queue.ElementAt(1) - queue.PeekFront(); //差分 { f(t + Δt) - f(t) }
    public float dFdT(float dt) => dF / dt; //時間差分 (f'(t))
    public float dFdT(Floats dt) => dF / dt.dF; //時間差分 (f'(t))

    public float dFn(int num) => queue.ElementAt(num + 1) - queue.ElementAt(num); //n番目の差分  { f(t + (n+1)Δt) - f(t + nΔt) }

    public float dFdTn(int num, float dt) => dFn(num) / dt; //n番目の時間差分 { f'(t + nΔt) }
    public float dFdTn(int num, Floats dt) => dFn(num) / dt.dFn(num); //n番目の時間差分 { f'(t + nΔt) }
    public float d2FdT2(float dt) => (dFdTn(0, dt) - dFdTn(1, dt)) / dt; //2階の時間差分 { f''(t) }
    public float d2FdT2(Floats dt) => (dFdTn(0, dt) - dFdTn(1, dt)) / dt.dFn(1); //2階の時間差分 { f''(t) }

    public float dnFdTn(int num, float dt) => (dFdTn(num - 2, dt) - dFdTn(num - 1, dt)) / dt; //n番目の時間差分 { f'''...(t) }


    public Floats(int size = 2)
    {
        queue = new Deque<float>(size);
    }

    public float Get(int num)
    {
        return queue.ElementAt(num);
    }

    public void Back()
    {
        queue.PopFront();
    }

    public void Push(float value)
    {
        queue.PushFront(value);
    }

    public void AddPush(float value)
    {
        queue.PushFront(queue.PeekFront() + value);
    }

    public static implicit operator float(Floats f)
    {
        return f.value;
    }

    public static Floats operator +(Floats f, float value)
    {
        f.value += value;
        return f;
    }

    public static Floats operator -(Floats f, float value)
    {
        f.value -= value;
        return f;
    }

    public static Floats operator *(Floats f, float value)
    {
        f.value *= value;
        return f;
    }

    public static Floats operator /(Floats f, float value)
    {
        f.value /= value;
        return f;
    }
}

// 