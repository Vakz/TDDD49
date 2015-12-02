using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model
{
    class Player
    {        
        protected bool _active; // Whether the player is allowed to move
        public abstract bool Active;
        private bool _alive; // Whether the player is still in the game
        public bool Alive
        {
            get
            {
                return _alive;
            }
            set
            {
                Active = value; // A player which is out of the game is also not Active
                _alive = value;
            }
        }
        private int _money;
        public int Money
        {
            get
            {
                return _money;
            }
            set
            {
                if (value < 0) throw new ArgumentException("Cannot set negative money");
            }
        }

        public abstract bool allowedToMovePiece(Piece p);
    }


}
