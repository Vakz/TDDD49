using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    public class Piece
    {
        public PieceType Type { get; set; }
       
        public Point Position { get; set; }

        public bool Alive { get; set; }

        public bool Active { get; set; }

        public int TurnsOnCurrentPosition { get; set; }

        public int TrainMovementStreak { get; set; }

        public int ID { get; set; }

        public Piece(int id, Point p)
        {
            Type = PieceType.Police;
            Position = p;
            Active = true;
            Alive = true;
            ID = id;
        }

        public static Piece Error
        {
            get
            {
                return new Piece(int.MaxValue, Point.Error);
            }
        }
    }

    public enum PieceType
    {
        Police,
        Thief
    }
}
