using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.Base.Datas
{
    public class LinkedList<T> : IEnumerable<T>
    {
        public LinkedListNode<T> Head { get; private set; }
        private LinkedListNode<T> Tail { get; set; }
        public int Count { get; private set; }

        public LinkedList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        // リストの先頭に要素を追加
        public void AddFirst(T value)
        {
            var newNode = new LinkedListNode<T>(value);
            if (Head == null)
            {
                Head = Tail = newNode;
            }
            else
            {
                newNode.Next = Head;
                Head.Last = newNode;
                Head = newNode;
            }
            Count++;
        }

        public void GetNode(int n)
        {
            var current = Head;
            for (int i = 0; i < n; i++)
            {
                current = current.Next;
            }
            Console.WriteLine(current.Value);
        }

        // リストの末尾に要素を追加
        public void AddLast(T value)
        {
            var newNode = new LinkedListNode<T>(value);
            if (Head == null)
            {
                Head = Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                newNode.Last = Tail;
                Tail = newNode;
            }
            Count++;
        }

        // 指定したノードをリストの先頭に移動
        public void ToFirst(LinkedListNode<T> node)
        {
            if (node == null || node == Head) return; // すでに先頭またはnullの場合は何もしない

            Remove(node); // ノードをリストから削除
            node.Next = Head; // ノードを先頭に挿入
            node.Last = null;
            if (Head != null)
            {
                Head.Last = node;
            }
            Head = node;
            if (Tail == null) // リストが空だった場合
            {
                Tail = Head;
            }
            Count++;
        }

        // 指定したノードをリストの末尾に移動
        public void ToLast(LinkedListNode<T> node)
        {
            if (node == null || node == Tail) return; // すでに末尾またはnullの場合は何もしない

            Remove(node); // ノードをリストから削除
            if (Tail != null)
            {
                Tail.Next = node;
                node.Last = Tail;
            }
            Tail = node;
            node.Next = null;
            if (Head == null) // リストが空だった場合
            {
                Head = Tail;
            }
            Count++;
        }

        // リストから指定したノードを削除
        public bool Remove(LinkedListNode<T> node)
        {
            if (node == null)
            {
                return false;
            }

            if (node == Head)
            {
                Head = node.Next;
                if (Head != null)
                {
                    Head.Last = null;
                }
                else
                {
                    Tail = null;
                }
            }
            else
            {
                if (node.Last != null)
                {
                    node.Last.Next = node.Next;
                }
                if (node.Next != null)
                {
                    node.Next.Last = node.Last;
                }
                else
                {
                    Tail = node.Last;
                }
            }

            node.Next = null;
            node.Last = null;
            Count--;
            return true;
        }

        // リストをクリア
        public void Clear()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        // 値がリストに存在するか確認
        public bool Contains(T value)
        {
            return Find(value) != null;
        }

        // 値を検索し、最初のノードを返す
        public LinkedListNode<T> Find(T value)
        {
            var current = Head;
            while (current != null)
            {
                if (current.Value.Equals(value))
                {
                    return current;
                }
                current = current.Next;
            }
            return null;
        }

        // リストをコンソールに表示
        public void PrintAll()
        {
            var current = Head;
            while (current != null)
            {
                Console.Write(current.Value + " -> ");
                current = current.Next;
            }
            Console.WriteLine("null");
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = Head;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}