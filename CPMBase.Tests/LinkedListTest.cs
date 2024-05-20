namespace CPMBase.Tests;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using _LinkedList = CPMBase.Base.Datas.LinkedList<int>;

[TestFixture]
public class LinkedListTest
{
    private _LinkedList list;

    [SetUp]
    public void SetUp()
    {
        // テストごとに新しいリストを初期化
        list = new();
    }

    private void AddNodes(params int[] values)
    {
        foreach (var value in values)
        {
            list.AddLast(value);
        }
    }

    private List<int> ListValues()
    {
        return list.ToList();
    }

    [Test]
    public void Test_Swap_Adjacent_Nodes()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Find(2);
        var node2 = list.Find(3);

        list.Swap(node1, node2);

        Assert.AreEqual(new List<int> { 1, 3, 2, 4 }, ListValues());
    }

    [Test]
    public void Test_Swap_Adjacent_NodesInvert()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Find(2);
        var node2 = list.Find(3);

        list.Swap(node2, node1);

        Assert.AreEqual(new List<int> { 1, 3, 2, 4 }, ListValues());
    }

    [Test]
    public void Test_Swap_Nodes_At_Head_And_Tail()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Head;
        var node2 = list.Tail;

        list.Swap(node1, node2);

        Assert.AreEqual(new List<int> { 4, 2, 3, 1 }, ListValues());
    }

    [Test]
    public void Test_Swap_Nodes_At_WithHead()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Head;
        var node2 = list.Find(2);

        list.Swap(node1, node2);

        Assert.AreEqual(new List<int> { 2, 1, 3, 4 }, ListValues());
    }

    [Test]
    public void Test_Swap_Nodes_At_WithTail()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Find(3);
        var node2 = list.Find(4);

        list.Swap(node1, node2);

        Assert.AreEqual(new List<int> { 1, 2, 4, 3 }, ListValues());
        Assert.AreEqual(list.Tail, node1);
        Assert.AreEqual(node1.Next, null);
        Assert.AreEqual(node1.Last, node2);
        Assert.AreEqual(node2.Next, node1);
    }

    [Test]
    public void Test_Swap_Same_Node()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Find(2);

        list.Swap(node1, node1);

        Assert.AreEqual(new List<int> { 1, 2, 3, 4 }, ListValues());
    }

    [Test]
    public void Test_Swap_With_Null()
    {
        AddNodes(1, 2, 3, 4);
        var node1 = list.Find(2);

        list.Swap(node1, null);

        Assert.AreEqual(new List<int> { 1, 2, 3, 4 }, ListValues());
    }

    [Test]
    public void Test_for()
    {
        var node = new CPMBase.Base.Datas.LinkedListNode<int>(-1);

        Assert.Null(list.Head);
        list.Insert(list.Head, node);
        Assert.AreEqual(new List<int> { 1, -1, 2, 3, 4 }, ListValues());

        list.Swap(node, node.Next);
        Assert.AreEqual(new List<int> { 1, 2, -1, 3, 4 }, ListValues());

        list.Swap(node, node.Next);
        Assert.AreEqual(new List<int> { 1, 2, 3, -1, 4 }, ListValues());

        list.Swap(node, node.Next);
        Assert.AreEqual(new List<int> { 1, 2, 3, 4, -1 }, ListValues());

        list.Remove(node);
        list.Insert(list.Head, node);
        Assert.AreEqual(new List<int> { 1, -1, 2, 3, 4 }, ListValues());

    }
}
