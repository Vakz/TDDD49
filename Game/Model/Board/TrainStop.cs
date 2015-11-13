using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.Board
{
    class TrainStop : Block
    {
        private List<int> _lines;

        bool isOnLine(int line)
        {
            return _lines.Any(i => i == line);
        }

        public TrainStop(List<int> lines) : base(BlockType.TrainStop)
        {
            _lines = new List<int>(lines);
        }
    }
}
