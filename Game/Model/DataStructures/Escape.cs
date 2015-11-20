using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Escape : Block
    {
        private int _cost;
        public int Cost
        {
            get
            {
                return _cost;
            }
        }

        public Escape(int cost, BlockType type)
            : base(type)
        {
            if (cost <= 0) throw new ArgumentException("Must have a cost");
            _cost = cost;
        }
    }
}
