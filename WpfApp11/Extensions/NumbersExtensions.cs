using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp11.Extensions
{
    
    public static class NumbersExtensions
    {
        #region https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc

        private static readonly string[] SizeSuffixes =
            { "байт", "КБ", "МБ", "ГБ", "ТБ", "PB", "EB", "ZB", "YB" };
        
        public static string GetSizeSuffix(long length, int decimalPlaces = 2)
        {
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
            }

            if (length < 0)
            {
                return "-" + GetSizeSuffix(-length, decimalPlaces);
            }

            if (length == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} байт", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int) Math.Log(length, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal)length / (1L << (mag * 10));

            // make adjustment when the length is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        #endregion
    }
}
