using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.Base.Datas
{
    public class LinkedListNode<T>
    {
        public T Value { get; set; }
        public LinkedListNode<T> Next { get; set; }
        public LinkedListNode<T> Last { get; set; }

        public LinkedListNode(T value)
        {
            this.Value = value;
            this.Next = null;
            this.Last = null;
        }
    }

}