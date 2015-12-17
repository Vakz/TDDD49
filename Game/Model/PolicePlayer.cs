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
        List<Piece> _pieces = new List<Piece>();
        public int Money { get; set; }
        public int ID { get; protected set; }

        // Set whether a police piece is active or not
        public bool this[Point pt]
        {
            get
            {
                return _pieces.First(s => s.Position == pt).Active;
            }
            set
            {
                _pieces.First(s => s.Position == pt).Active = value;
            }
        }

        public PolicePlayer(int id, List<Point> policeSpawnpoints)
        {
            foreach (Point pt in policeSpawnpoints) {
                _pieces.Add(new Piece(++id, pt));
            }
        }

        public bool allowedToMovePiece(Piece p)
        {
            return p.Type == PieceType.Police && p.Active;
        }

        public List<Piece> getControlledPieces()
        {
            return _pieces;
        }

        public bool anyInPlay()
        {
            return true; // Police always has at least one piece in play while game is running
        }
    }
}
