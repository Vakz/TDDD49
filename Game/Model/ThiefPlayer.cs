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

        public bool Active
        {
            get
            {
                return _active && _piece.ArrestTurns == 0;
            }
            set
            {
                _active = value;
            }
        }

        public ThiefPlayer(Point startingPosition)
        {
            _piece = new Thief(startingPosition);
        }
    }
}
