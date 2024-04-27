using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace CPMBase.Dependency.Vector
{
    public class VectorExtensions
    {
            // Vector<T>のための拡張メソッドとしてのファクトリーメソッド
        public static Vector<T> CreateVector<T>(params T[] values) where T : struct
        {
            T[] tempArray = new T[Vector<T>.Count];
            int copyCount = Math.Min(values.Length, Vector<T>.Count);

            Array.Copy(values, tempArray, copyCount);
            for (int i = copyCount; i < Vector<T>.Count; i++)
            {
                tempArray[i] = default(T);
            }

            return new Vector<T>(tempArray);
        }
    }
}