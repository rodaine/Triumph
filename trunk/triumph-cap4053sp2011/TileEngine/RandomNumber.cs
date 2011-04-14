using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    /// <summary>
    /// Gets random numbers! how exciting
    /// </summary>
    public class RandomNumber
    {
        private static RandomNumber singleton;
        private long num;

        private RandomNumber()
        {
            Random ran = new Random();
            num = ran.Next(10000);
        }

        /// <summary>
        /// gets the singleton of randomnumber
        /// </summary>
        /// <returns></returns>
        public static RandomNumber getInstance()
        {
            if (singleton == null)
            {
                singleton = new RandomNumber();
            }
            return singleton;
        }

        /// <summary>
        /// Gets a random number between the specified bounds
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int getNext(int min, int max)
        {
            num = (num * 1103515245 + 12345) % 4294967296;
            long ret = (num / 65536) % 32768;
            ret = ret % (max - min + 1);
            return (int)ret + min;
        }
    }
}
