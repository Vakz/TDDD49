using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Board
    {
        private Block[,] _board;
        private List<Piece> _pieces;

        public Board(int width, int height, List<Piece> pieces) {
            _board = new Block[height,width];
            _pieces = new List<Piece>(pieces);
        }
        public Block this[int x, int y]
        {
            get
            {
                return _board[y, x];
            }
            set
            {
                _board[y, x] = value;
            }
        }

        public Block this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
            set
            {
                this[p.X, p.Y] = value;
            }
        }

        public IReadOnlyCollection<Piece> Pieces
        {
            get
            {
                return _pieces.AsReadOnly();
            }
        }

        public Piece getPieceAt(Point p) {
            return _pieces.First(s => s.Position == p);
        }

        public int Height
        {
            get
            {
                return _board.GetLength(0);
            }
        }

        public int Width
        {
            get
            {
                return _board.GetLength(1);
            }
        }
    }
}
