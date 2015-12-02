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
        Dictionary<Piece, bool> _pieces = new Dictionary<Piece, bool>();

        // Set whether a police piece is active or not
        public bool this[Piece p]
        {
            get
            {
                return _pieces[p];
            }
            set
            {
                _pieces[p] = value;
            }
        }

        public PolicePlayer(List<Point> policeSpawnpoints)
        {
            foreach (Point pt in policeSpawnpoints) {
                _pieces.Add(new Piece(pt), true);
            }
        }

        public bool allowedToMovePiece(Piece p)
        {
            return _pieces.ContainsKey(p) && _pieces[p];
        }
    }
}
