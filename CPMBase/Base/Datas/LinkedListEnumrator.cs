using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.Base.Datas
{
    public class LinkedListEnumrator<T> : IEnumerator<T>
    {
        private LinkedListNode<T> current;
        private LinkedListNode<T> head;

        public LinkedListEnumrator(LinkedListNode<T> head)
        {
            this.head = head;
            current = null;
        }

        public T Current
        {
            get
            {
                return current.Value;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return current.Value;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (current == null)
            {
                current = head;
            }
            else
            {
                current = current.Next;
            }
            return current != null;
        }

        public void Reset()
        {
            current = null;
        }
    }
}