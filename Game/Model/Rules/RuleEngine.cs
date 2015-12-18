using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model.Logic;
using Game.Exceptions;

namespace Game.Model.Rules
{
    class RuleEngine
    {
        private Board _board;
        private PathFinder pathfinder;
        public const int MAX_ARRESTS = 4;
        public const int MAX_TURNS_BY_TRAIN = 2;

        public RuleEngine(Board board)
        {
            _board = board;
            pathfinder = new PathFinder( _board.Width, _board.Height );
        }

        public bool canMoveTo(Piece piece, Point dest, int cost = -1)
        {
            if (!LogicEngine.containedIn(dest, _board.Height, _board.Width)) return false;
            if (_board[dest].Type == BlockType.Blocked) return false;
            if (_board.isOccupied(dest) && !canArrestAt(piece, dest)) return false;
            if (!isAllowedOn(dest, piece)) return false;
            if (_board[piece.Position].Type == BlockType.TrainStop && _board[dest].Type == BlockType.TrainStop && piece.TrainMovementStreak <= MAX_TURNS_BY_TRAIN)
            {
                if (((TrainStop)_board[piece.Position]).sharesLines((TrainStop)_board[dest])) {
                    return true;
                }
            }
            return canReach(piece, dest, cost);
        }

        public bool isThiefSurrounded(PathFinder pf, List<Point> hideouts, Thief t)
        {
            PieceCanPassPoint canPass = new PieceCanPassPoint(t, _board, this);
            return pf.getClosestPointOfInterest(t.Position, hideouts, canPass) == Point.Error;
        }

        private bool canReach(Piece piece, Point dest, int cost) {
            PieceCanPassPoint canPass = new PieceCanPassPoint(piece, _board, this);
            if ( cost == -1 ) {
                return pathfinder.getShortestPath(piece.Position, dest, canPass).Count != 0;
            } else if (_board[dest].Type == BlockType.EscapeAirport) {
                return pathfinder.getShortestPath(piece.Position, dest, canPass).Count <= cost;
            } else {
                List<Point> path = pathfinder.getPathWithExactCost(piece.Position, dest, cost, canPass);
                return path != null && (path.Count - 1) == cost;
            }
        }

        private class PieceCanPassPoint : PathFinder.CanPass {
            private Piece piece;
            private Board board;
            private RuleEngine rule_engine;
            public PieceCanPassPoint(Piece piece, Board board, RuleEngine rule_engine){
                this.piece = piece;
                this.rule_engine = rule_engine;
                this.board = board;
            }
            public bool check(Point pt){
                if (piece.Type == PieceType.Police && board[pt].Type == BlockType.Bank) return true;

                Piece p = board.getPieceAt(pt);
                if (piece.Type == PieceType.Thief && ((Thief)piece).Arrestable && p != null && p.Type == PieceType.Police) return false;
                return rule_engine.isAllowedOn(pt, piece);
            }
        }

        public bool canArrestAt(Piece p, Point pt)
        {
            if (p.Type != PieceType.Police) return false;
            Piece arrestTarget = _board.getPieceAt(pt);
            if (arrestTarget == null) return false;
            if (arrestTarget.Type != PieceType.Thief) return false;
            return (((Thief)arrestTarget).Arrestable);
        }
        
        public bool isAllowedOn(Point pt, Piece p) {
            switch(_board[pt].Type)
            {
                case BlockType.PoliceStation:
                    return p.Type == PieceType.Police;
                case BlockType.Hideout:
                case BlockType.EscapeAirport:
                case BlockType.EscapeCheap:
                case BlockType.Bank:
                    return p.Type == PieceType.Thief;
                case BlockType.TrainStop:
                case BlockType.Normal:
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

        /// <summary>
        /// Checks if it
        /// </summary>
        /// <param name="t"></param>
        public void release(Thief t)
        {
            t.Arrestable = false;
            t.Active = true;
            // Find a police to activate, as guarding is done
            _board.Pieces.First(s => s.Alive && !s.Active).Active = true;
        }

        public int arrest(Thief t, Piece p)
        {
            if (!(p.Type == PieceType.Police)) throw new ArgumentException("Arresting piece must be of type Police");
            int money = t.arrest(Logic.LogicEngine.diceRoll());
            t.Position = _board.getUnoccupiedByBlockType(BlockType.PoliceStation);
            p.Position = _board.getUnoccupiedByBlockType(BlockType.PoliceStation);
            p.Active = false;
            // Police gets 1000 for every started 5000
            return calcArrestMoney(money);
        }

        private int calcArrestMoney(int money) {
            return ((money - 1) / 5000 + 1) * 1000;
        }

        public bool allThievesArrested()
        {
            return !(_board.Pieces.Any(s => s.Type == PieceType.Thief && ((Thief)s).ArrestTurns == 0));
        }

        public void stopEscape(Piece p)
        {
            if (p.Type != PieceType.Police) return;
            try
            {
                Piece arrestTarget = _board.Pieces.First(b => b.Type == PieceType.Thief && validEscapeArrest((Thief)b));
                arrest((Thief)arrestTarget, p);
            }
            catch (Exception)
            {
                return;
            }
        }

        private bool validEscapeArrest(Thief t) {
            return t.Arrestable && 
                   (_board[t.Position].Type == BlockType.EscapeAirport || _board[t.Position].Type == BlockType.EscapeCheap);
        }

        public bool isAllowedToSkipTurn(Piece p) {
            if (p.Type == PieceType.Thief) return ((Thief)p).ArrestTurns > 0;
            if (p.TurnsOnCurrentPosition < 2) return true;
            if (_board[p.Position].Type == BlockType.TravelAgency) return false;
            return !nextToEscape(p.Position);
        }

        public void removePieceFromGame(Piece p)
        {
            p.Position = Point.Error;
            p.Active = false;
            p.Alive = false;
        }

        public void removePieceFromGame(Point pt)
        {
            removePieceFromGame(_board.getPieceAt(pt));
        }

        private bool nextToEscape(Point p)
        {
            return _board.isEscape(p + new Point(0, 1)) ||
                   _board.isEscape(p - new Point(0, 1)) ||
                   _board.isEscape(p + new Point(1, 0)) ||
                   _board.isEscape(p - new Point(1, 0));
        }
    }
}
