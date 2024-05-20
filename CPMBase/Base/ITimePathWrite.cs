using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CPMBase.Base
{
    /// <summary>
    ///  時間経過によってファイルを書き込むクラス
    /// </summary>
    public interface ITimePathWrite
    {
        public void Write(float time, int step, PathObject pathObject);
        public void WriteImage(float time, int step, PathObject pathObject, Vector2 resolution);
    }

    public interface ITimePathWrite<T>
    {
        public void Write(float time, int step, PathObject pathObject, T t);
    }

    public interface ITimePathWrite<T1, T2>
    {
        public void Write(float time, int step, PathObject pathObject, T1 t, T2 t2);
    }

    public interface ITimePathWrite<T1, T2, T3>
    {
        public void Write(float time, int step, PathObject pathObject, T1 t, T2 t2, T3 t3);
    }
}