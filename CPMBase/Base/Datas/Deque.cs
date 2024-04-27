using System.Numerics;

namespace CPMBase;

public class Deque<T> : IEnumerable<T>
{
    private T[] values;
    private int head;
    private int tail;
    private int count;
    public int Count => count;

    public Deque(int capacity)
    {
        values = new T[capacity];
        head = 0;
        tail = 0;
        count = 0;
    }

    public void PushFront(T value)
    {
        if (count == values.Length)
        {
            throw new InvalidOperationException("Deque is full");
        }
        head = (head - 1 + values.Length) % values.Length;
        values[head] = value;
        count++;
    }

    public void PushBack(T value)
    {
        if (count == values.Length)
        {
            throw new InvalidOperationException("Deque is full");
        }
        values[tail] = value;
        tail = (tail + 1) % values.Length;
        count++;
    }

    public T PopFront()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        T value = values[head];
        head = (head + 1) % values.Length;
        count--;
        return value;
    }

    public T PopBack()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        tail = (tail - 1 + values.Length) % values.Length;
        T value = values[tail];
        count--;
        return value;
    }

    public T PeekFront()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        return values[head];
    }

    public T PeekBack()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        return values[(tail - 1 + values.Length) % values.Length];
    }

    public void Clear()
    {
        head = 0;
        tail = 0;
        count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++)
        {
            yield return values[(head + i) % values.Length];
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T ElementAt(int index)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        return values[(head + index) % values.Length];
    }

    public void Set(int index, T value)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        values[(head + index) % values.Length] = value;
    }

    public void SetFront(T value)
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        values[head] = value;
    }

    public void SetBack(T value)
    {
        if (count == 0)
        {
            throw new InvalidOperationException("Deque is empty");
        }
        values[(tail - 1 + values.Length) % values.Length] = value;
    }
}
