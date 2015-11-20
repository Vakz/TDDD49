using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Model.Rules
{
    class RuleEngine
    {
        private Board _board;

        RuleEngine(Board board)
        {
            _board = board;
        }

        public bool move(Piece piece, Point dest)
        {
            if (_board[dest].Type == BlockType.Blocked) { return false; } // Is inaccessible block
            if (_board.Pieces.Any(s => s.Position == dest)) { return false; }

            return true;
        }
        /*
        private bool isAllowedOn(BlockType t, Piece p) {
            if (p.Type == PieceType.Police)
            {
                switch (t)
                {
                    case 
                }
            }
        }*/
    }
}
