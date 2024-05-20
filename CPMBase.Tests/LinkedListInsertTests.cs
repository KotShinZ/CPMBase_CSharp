using NUnit.Framework;
using System;
using CPMBase.Base.Datas;

using LinkedList = CPMBase.Base.Datas.LinkedList<int>;
using LinkedListNode = CPMBase.Base.Datas.LinkedListNode<int>;

[TestFixture]
public class LinkedListInsertTests
{
    private LinkedList list;

    [SetUp]
    public void SetUp()
    {
        list = new LinkedList();
        // テスト用リストに初期データを追加
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
    }

    [Test]
    public void Insert_NewNodeAfterFirstNode_ShouldBeInCorrectPosition()
    {
        var node1 = list.Find(1);
        var newNode = new LinkedListNode(4);

        list.Insert(node1, newNode);

        Assert.AreEqual(4, node1.Next.Value);
        Assert.AreEqual(2, newNode.Next.Value);
    }

    [Test]
    public void Insert_NewNodeAtListTail_ShouldUpdateTail()
    {
        var nodeTail = list.Tail;
        var newNode = new LinkedListNode(5);

        list.Insert(nodeTail, newNode);

        Assert.AreEqual(5, list.Tail.Value);
        Assert.IsNull(newNode.Next);
    }

    [Test]
    public void Insert_NewNode_ShouldIncreaseListCount()
    {
        var node1 = list.Find(1);
        var newNode = new LinkedListNode(6);

        list.Insert(node1, newNode);

        Assert.AreEqual(4, list.Count);
    }

    [Test]
    public void Insert_NullNodeAsNewNode_ShouldThrowArgumentNullException()
    {
        var node1 = list.Find(1);

        var ex = Assert.Throws<ArgumentNullException>(() => list.Insert(node1, null));
        StringAssert.Contains("newNode cannot be null", ex.Message);
    }
}