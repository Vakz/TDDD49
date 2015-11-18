using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    struct Point
    {
        Point(int x, int y) 
        {
            this.x = x;
            this.y = y;
        }
        int x;
        int y;
    

        public static bool operator ==(Point a, Point b) 
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Point a, Point b) 
        {
            return !(a == b);
        }
    }

    
}
