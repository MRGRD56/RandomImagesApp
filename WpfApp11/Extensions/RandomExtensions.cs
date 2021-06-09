using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp11.Extensions
{
    public static class RandomExtensions
    {
        public static int NextOf(this Random random, params int[] possibleNumbers) => 
            possibleNumbers[random.Next(0, possibleNumbers.Length)];
    }
}
