using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.Board
{
    class TrainStation
    {
        private List<int> _lines;

        bool isOnLine(int line)
        {
            return _lines.Any(i => i == line);
        }

        public TrainStation(List<int> lines)
        {
            _lines = new List<int>(lines);
        }
    }
}
