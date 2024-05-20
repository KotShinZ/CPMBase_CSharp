using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.CPM
{
    public class RandomSelector : CPMSelectorBase
    {
        public RandomSelector(CPMAreaArray area) : base(area)
        {
        }

        public override CPMArea Select()
        {
            return area.GetRandomNextCell();
        }
    }
}