﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    struct Point
    {
        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    

        public static bool operator ==(Point a, Point b) 
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b) 
        {
            return !(a == b);
        }
    }

    
}
