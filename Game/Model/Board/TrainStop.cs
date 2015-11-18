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
        
        /// <summary>
        /// Checks whether two TrainStop instances are on the same line
        /// </summary>
        /// <param name="s">Stop to compare to</param>
        /// <returns>True if the instances share lines</returns>
        bool sharesLines(TrainStop s)
        {
            return _lines.Intersect(s._lines).Any();
        }

        public TrainStop(List<int> lines) : base(BlockType.TrainStop)
        {
            _lines = new List<int>(lines);
        }
    }
}
