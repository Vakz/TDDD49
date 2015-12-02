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
        private Thief _piece;
        private bool _active;

        public bool Active
        {
            get
            {
                return _active && _piece.ArrestTurns == 0;
            }
            set;
        }

        public ThiefPlayer(Point startingPosition)
        {
            _piece = new Thief(startingPosition);
        }

        public bool allowedToMovePiece(Piece p)
        {
            return Active && p.Position == _piece.Position;
        }
    }
}
