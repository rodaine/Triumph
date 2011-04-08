using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class RandomNumber
    {
        private static RandomNumber singleton;
        private long num;

        private RandomNumber()
        {
            Random ran = new Random();
            num = ran.Next(10000);
        }

        public static RandomNumber getInstance()
        {
            if (singleton == null)
            {
                singleton = new RandomNumber();
            }
            return singleton;
        }

        public int getNext(int min, int max)
        {
            num = (num * 1103515245 + 12345) % 4294967296;
            num = num % (max - min + 1);
            return (int)num + min;
        }
    }
}
