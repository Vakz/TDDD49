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
        private bool _active;

        public bool Active
        {
            get
            {
                return _active && Piece.ArrestTurns == 0;
            }
            set { _active = value;}
        }

        public ThiefPlayer(Point startingPosition)
        {
            Piece = new Thief(startingPosition);
        }

        public override bool allowedToMovePiece(Piece p)
        {
            return Active && p.Position == Piece.Position;
        }
    }
}
