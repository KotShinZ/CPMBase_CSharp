namespace CPMBase.Tests;

public class LinkedListTest
{
    [Fact]
    public void Test1()
    {
        var list = new LinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);
        list.AddLast(3);
        foreach (var item in list)
        {
            Assert.Equal(2, item);
        }
    }
}
