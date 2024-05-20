using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.Base.Datas
{
    public class LinkedList<T> : IEnumerable<T>
    {
        private LinkedListNode<T> _head;
        public LinkedListNode<T> Head
        {
            get => _head;
            set
            {
                _head = value;
                if (_head != null)
                {
                    _head.Last = null;
                }
            }
        }

        private LinkedListNode<T> _tail;
        public LinkedListNode<T> Tail
        {
            get => _tail;
            set
            {
                _tail = value;
                if (_tail != null)
                {
                    _tail.Next = null;
                }
            }
        }
        public int Count { get; private set; }

        public LinkedList()
        {
            Clear();
        }

        public LinkedList(List<T> values) : this()
        {
            foreach (var item in values)
            {
                AddLast(item);
            }
        }

        // リストの先頭に要素を追加
        public void AddFirst(T value)
        {
            AddFirst(new LinkedListNode<T>(value));
        }

        // リストの末尾に要素を追加
        public void AddLast(T value)
        {
            AddLast(new LinkedListNode<T>(value));
        }

        // リストの先頭に要素を追加
        public void AddFirst(LinkedListNode<T> value)
        {
            var newNode = value;
            if (Head == null)
            {
                Head = Tail = newNode;
            }
            else
            {
                newNode.Next = Head;
                if(Head != null)Head.Last = newNode;
                Head = newNode;
            }
            Count++;
        }

        // リストの末尾に要素を追加
        public void AddLast(LinkedListNode<T> value)
        {
            var newNode = value;
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


        public void GetNode(int n)
        {
            var current = Head;
            for (int i = 0; i < n; i++)
            {
                current = current.Next;
            }
            Console.WriteLine(current.Value);
        }

        public List<T> ToList()
        {
            List<T> values = new List<T>();

            foreach (var t in this)
            {
                values.Add(t);
            }

            return values;
        }

        public List<LinkedListNode<T>> ToNodeList()
        {
            List<LinkedListNode<T>> listNodes = new();
            LinkedListNode<T> node = Head;
            while (true)
            {
                if (node == null) break;
                listNodes.Add(node);
                node = node.Next;
            }
            return listNodes;
        }

        public void FromNodeList(List<LinkedListNode<T>> listNodes)
        {
            Clear();
            foreach (var node in listNodes)
            {
                AddFirst(node);
            }
        }

        public void Shuffle()
        {
            var list = ToNodeList();
            list.Shuffle();
            FromNodeList(list);
        }

        // 指定したノードをリストの先頭に移動
        public void ToFirst(LinkedListNode<T> node)
        {
            if (node == null || node == Head) return; // すでに先頭またはnullの場合は何もしない

            Remove(node); // ノードをリストから削除
            AddFirst(node);
        }

        // 指定したノードをリストの末尾に移動
        public void ToLast(LinkedListNode<T> node)
        {
            if (node == null || node == Tail) return; // すでに末尾またはnullの場合は何もしない

            Remove(node); // ノードをリストから削除
            AddLast(node);
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


        /// <summary>
        /// ふたつのノードを入れ替える *テスト済み
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        public void Swap(LinkedListNode<T> node1, LinkedListNode<T> node2)
        {
            if (node1 == null || node2 == null || node1 == node2) return;

            var tempLast1 = node1.Last;
            var tempNext1 = node1.Next;
            var tempLast2 = node2.Last;
            var tempNext2 = node2.Next; //null

            // 隣接していない場合の一時変数を使った交換
            if (node1.Next != node2 && node2.Next != node1)
            {
                node1.Last = tempLast2;
                node1.Next = tempNext2;
                if (tempNext2 != null) tempNext2.Last = node1;
                if (tempLast2 != null) tempLast2.Next = node1;

                node2.Last = tempLast1;
                node2.Next = tempNext1;
                if (tempNext1 != null) tempNext1.Last = node2;
                if (tempLast1 != null) tempLast1.Next = node2;
            }
            else // node1とnode2が隣接している場合
            {
                // この処理は node1 が node2 の直前にある場合に適用されます
                if (node1.Next == node2)
                {
                    // node1 と node2 のポインタを交換
                    node1.Last = node2;
                    node1.Next = tempNext2;
                    node2.Last = tempLast1;
                    node2.Next = node1;

                    // 関連するノードのポインタを更新
                    if (tempNext2 != null) tempNext2.Last = node1;
                    if (tempLast1 != null) tempLast1.Next = node2;
                }
                else if (node2.Next == node1) // node2 が node1 の直前にある場合
                {
                    // node2 と node1 のポインタを交換
                    node2.Last = node1;
                    node2.Next = tempNext1;
                    node1.Last = tempLast2;
                    node1.Next = node2;

                    // 関連するノードのポインタを更新
                    if (tempNext1 != null) tempNext1.Last = node2;
                    if (tempLast2 != null) tempLast2.Next = node1;
                }
            }

            // node1 または node2 が Head または Tail である場合の処理
            if (node1 == Head)
                Head = node2;
            else if (node2 == Head)
                Head = node1;

            if (node1 == Tail)
                Tail = node2;
            else if (node2 == Tail)
                Tail = node1;
        }

        /// <summary>
        /// 与えられたノードの後ろに新しいノードを挿入する
        /// </summary>
        /// <param name="target">既存のノード</param>
        /// <param name="newNode">挿入する新しいノード</param>
        public void Insert(LinkedListNode<T> target, LinkedListNode<T> newNode)
        {
            if (target == null || newNode == null)
            {
                throw new ArgumentNullException("target or newNode cannot be null.");
            }

            // newNodeをnode1の後ろに挿入
            newNode.Next = target.Next;
            newNode.Last = target;
            target.Next = newNode;

            // node1の元々の次のノードのLastを更新
            if (newNode.Next != null)
            {
                newNode.Next.Last = newNode;
            }

            // newNodeがリストの新しい末尾になる場合
            if (target == Tail)
            {
                Tail = newNode;
            }

            Count++;
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
            if (current == null)
            {
                yield break;
            }
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