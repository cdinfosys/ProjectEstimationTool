using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.Utilities
{
    public struct IntDescendingSortComparer : IComparer<Int32>
    {
        public Int32 Compare(Int32 x, Int32 y)
        {
            if (x > y) return -1;
            else if (x < y) return 1;
            else return 0;
        }
    }
}
