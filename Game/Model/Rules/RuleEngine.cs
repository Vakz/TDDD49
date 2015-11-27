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

        public void robBank(Thief t, Bank b) {
            t.Money += b.rob();
            t.Arrestable = true;
        }

        public int arrest(Thief t, Piece p)
        {
            if (!(p.Type == PieceType.Police)) throw new ArgumentException("Arresting piece must be of type Police");
            int money = t.arrest(Logic.LogicEngine.diceRoll());
            t.Position = _board.getUnoccupiedByBlockType(BlockType.PoliceStation);
            p.Position = _board.getUnoccupiedByBlockType(BlockType.PoliceStation);
            // Police gets 1000 for every started 5000
            // TODO: Fix this
            return t.Money > 0 ? (money / 5000 + 1) * 1000 : 0;
        }

        public bool allThievesArrested()
        {
            return _board.Pieces.Any(s => s.Type == PieceType.Thief && ((Thief)s).ArrestTurns > 0);
        }

        public void stopEscape(Piece p)
        {
            if (p.Type != PieceType.Police) return;
            try
            {
                Piece arrestTarget = _board.Pieces.First(b => b.Type == PieceType.Thief && validEscapeArrest((Thief)b));
                arrest((Thief)arrestTarget, p);
            }
            catch (Exception e)
            {
                return;
            }
        }

        private bool validEscapeArrest(Thief t) {
            return t.Arrestable && (_board[t.Position].Type == BlockType.EscapeAirport || _board[t.Position].Type == BlockType.EscapeCheap);
        }
    }
}
