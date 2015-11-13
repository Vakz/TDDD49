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

        public Board(int width, int height) {
            _board = new Block[height,width];
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
    }
}
