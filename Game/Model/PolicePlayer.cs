using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model
{
    class PolicePlayer : Player
    {
        List<Piece> _pieces = new List<Piece>();

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

        public PolicePlayer(List<Point> policeSpawnpoints)
        {
            foreach (Point pt in policeSpawnpoints) {
                _pieces.Add(new Piece(pt));
            }
        }

        public override bool allowedToMovePiece(Piece p)
        {
            return p.Type == PieceType.Police && p.Active;
        }
    }
}
