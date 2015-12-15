using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Piece
    {
        public PieceType Type { get; protected set; }
       
        public Point Position { get; set; }

        public bool Alive { get; set; }

        public bool Active { get; set; }

        public int TurnsOnCurrentPosition { get; set; }

        public int TrainMovementStreak { get; set; }

        public Piece(Point p)
        {
            Type = PieceType.Police;
            Position = p;
            Active = true;
            Alive = true;
        }
    }

    public enum PieceType
    {
        Police,
        Thief
    }
}
