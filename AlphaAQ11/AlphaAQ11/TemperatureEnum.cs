using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    public enum Temperature
    {
        Normal,
        High,
        VeryHigh,
        NotNormal,
        NotHigh,
        NotVeryHigh
    }

    public static class TemperatureExtentions
    {
        public static Temperature GetOpposite(this Temperature temperature)
        {
            switch (temperature)
            {
                case Temperature.Normal:
                    return Temperature.NotNormal;
                case Temperature.High:
                    return Temperature.NotHigh;
                case Temperature.VeryHigh:
                    return Temperature.NotVeryHigh;
                case Temperature.NotNormal:
                    return Temperature.Normal; 
                case Temperature.NotHigh:
                    return Temperature.High;
                //case Temperature.NotVeryHigh:
                //    return Temperature.VeryHigh;
                default:
                    return Temperature.VeryHigh;       
            }
        }
    } 
}
