using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model
{
    class ThiefPlayer : IPlayer
    {
        public Thief Piece { get; private set; }

        public int Money
        {
            get
            {
                return Piece.Money;
            }
        }

        public ThiefPlayer(Point startingPosition)
        {
            Piece = new Thief(startingPosition);
        }

        public bool allowedToMovePiece(Piece p)
        {
            return Piece.Active && p == Piece;
        }

        public List<Piece> getControlledPieces()
        {
            return new List<Piece>() { Piece };
        }

        public bool anyInPlay()
        {
            return Piece.Alive;
        }
    }
}
