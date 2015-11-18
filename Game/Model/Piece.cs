using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    abstract class Piece
    {
        public Point Position { get; set; }

        public Piece(Point p)
        {
            Position = p;
        }
    }
}
