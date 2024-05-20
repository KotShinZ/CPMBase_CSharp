using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPMBase.CPM
{
    public class ActSelector : CPMSelectorBase
    {
        public static LinkedList<CPMArea> ActSortedList = new LinkedList<CPMArea>();
        public static float sumAct = 0;

        public ActSelector(CPMAreaArray area) : base(area)
        {
        }

        public override CPMArea Select()
        {
            //return (CPMArea)cellAreaArray.GetRandomCell();
            float sum = 0;
            var rand = Randomizer.NextFloat() * sumAct;
            foreach (var area in ActSortedList)
            {
                sum += area.activity;
                if (sum >= rand) return area;
            }
            return null;
        }
    }
}