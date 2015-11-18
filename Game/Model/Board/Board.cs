using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.Board
{
    class Board
    {
        private Block[,] _board;
        private List<Piece> _pieces;

        public Board(int width, int height, List<Piece> pieces) {
            _board = new Block[height,width];
            _pieces = new List<Piece>(pieces);
        }

        public void setBlock(int x, int y, Block block)
        {
            _board[y, x] = block;
        }

        public Block this[int x, int y]
        {
            get
            {
                return _board[y, x];
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
