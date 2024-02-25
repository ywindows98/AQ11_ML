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
                    break;
                case Temperature.High:
                    return Temperature.NotHigh;
                    break;
                case Temperature.VeryHigh:
                    return Temperature.NotVeryHigh;
                    break;
                case Temperature.NotNormal:
                    return Temperature.Normal;
                    break;
                case Temperature.NotHigh:
                    return Temperature.High;
                    break;
                //case Temperature.NotVeryHigh:
                //    return Temperature.VeryHigh;
                //    break;
                default:
                    return Temperature.VeryHigh;       
            }
        }
    } 
}
