using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace f14.VisualEase.Helpers
{
    public static class MathHelper
    {
        public static double Clamp(double val, double min, double max)
        {
            return (val < min) ? min : (val > max) ? max : val;
        }
    }
}
