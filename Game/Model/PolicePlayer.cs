using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using System.Collections.ObjectModel;

namespace Game.Model
{
    public class PolicePlayer : IPlayer
    {
        public List<Piece> Pieces { get; set; }
        public int Money { get; set; }
        public int ID { get; protected set; }

        // Set whether a police piece is active or not
        public bool this[Point pt]
        {
            get
            {
                return Pieces.First(s => s.Position == pt).Active;
            }
            set
            {
                Pieces.First(s => s.Position == pt).Active = value;
            }
        }

        public PolicePlayer(int id)
        {
            Pieces = new List<Piece>();
        }

        public Piece addPiece(Point pt) {
            Pieces.Add(new Piece(ID + Pieces.Count, pt));
            return Pieces.Last();
        }

        public bool allowedToMovePiece(Piece p)
        {
            return p.Type == PieceType.Police && p.Active;
        }

        public List<Piece> getControlledPieces()
        {
            return Pieces;
        }

        public bool anyInPlay()
        {
            return true; // Police always has at least one piece in play while game is running
        }
    }
}
