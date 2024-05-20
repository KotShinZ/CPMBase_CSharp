using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.ExSimrations
{
    public interface ISimration
    {
        public void PreInit();

        public void Init();

        public Task Start();

        public void End();

        public void Final();

        public virtual async Task Run()
        {
            AllSim();
        }

        public async Task AllSim()
        {
            PreInit();
            Init();
            await Start();
            End();
            Final();
        }
    }
}