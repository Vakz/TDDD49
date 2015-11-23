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

        public bool canMoveTo(Piece piece, Point dest)
        {
            if (_board[dest].Type == BlockType.Blocked) { return false; } // Is inaccessible block
            if (_board.isOccupied(dest)) { return false; }
            if (!isAllowedOn(_board[dest].Type, piece)) { return false;  }
            if (_board[piece.Position].Type == BlockType.TrainStop && _board[dest].Type == BlockType.TrainStop)
            {
                if (((TrainStop)_board[piece.Position]).sharesLines((TrainStop)_board[dest])) {
                    return true;
                }
            }
            // TODO: More cases?
            return canReach(piece, dest);
        }

        private bool canReach(Piece piece, Point dest) {
            throw new NotImplementedException();
        }

        private bool canPass(BlockType t, Piece p)
        {
            return (p.Type == PieceType.Police && t == BlockType.Bank) || isAllowedOn(t, p);
        }
        
        private bool isAllowedOn(BlockType t, Piece p) {
            switch(t)
            {
                case BlockType.PoliceStation:
                    return p.Type == PieceType.Police;
                case BlockType.Hideout:
                case BlockType.EscapeAirport:
                case BlockType.EscapeCheap:
                    return p.Type == PieceType.Thief;
                case BlockType.TrainStop:
                case BlockType.Normal:
                case BlockType.Bank:
                case BlockType.Telegraph:
                    return true;
                default:
                    return false;
            }
        }
    }
}
