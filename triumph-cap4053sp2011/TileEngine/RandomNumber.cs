﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public class RandomNumber
    {
        private long num;

        public RandomNumber()
        {
            Random ran = new Random();
            num = ran.Next(10000);
            System.Console.WriteLine(num);
        }

        public int getNext(int min, int max)
        {
            num = (num * 1103515245 + 12345) % 4294967296;
            num = num % (max - min + 1);
            return (int)num + min;
        }
    }
}