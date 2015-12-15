using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model
{
    class ThiefPlayer : Player
    {
        public Thief Piece { get; private set; }

        public ThiefPlayer(Point startingPosition)
        {
            Piece = new Thief(startingPosition);
        }

        public override bool allowedToMovePiece(Piece p)
        {
            return Piece.Active && p == Piece;
        }

        public override List<Piece> getControlledPieces()
        {
            return new List<Piece>() { Piece };
        }

        public override bool anyInPlay()
        {
            return Piece.Alive;
        }
    }
}
