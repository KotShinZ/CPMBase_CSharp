using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.Base
{
    public class TimeFunc
    {
        public float duration;

        public Action func;

        public TimeFunc(float duration, Action func)
        {
            this.duration = duration;
            this.func = func;
        }
    }
}