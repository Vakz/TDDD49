using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model
{
    public class ThiefPlayer : IPlayer
    {
        public Thief Piece { get; private set; }
        public int ID { get; protected set; }

        public int Money
        {
            get
            {
                return Piece.Money;
            }
        }

        public ThiefPlayer(int id, Point startingPosition)
        {
            ID = id;
            Piece = new Thief(id, startingPosition);
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
