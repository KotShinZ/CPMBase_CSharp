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

        public async void Run()
        {
            PreInit();
            Init();
            await Start();
            End();
            Final();
        }
    }
}