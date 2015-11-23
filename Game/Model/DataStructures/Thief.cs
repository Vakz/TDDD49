using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Thief : Piece
    {
        public int Money { get; set; }

        public bool Arrestable { get; set; }
        public int ArrestTurns { get; set; }
        public int ArrestCount { get; private set; }

        public Thief(Point p) : base(p)
        {
            Type = PieceType.Thief;
        }

        public int arrest(int turns) 
        {
            int temp = Money;
            Money = 0;
            ArrestCount++;
            Arrestable = false;
            ArrestTurns = turns;
            return temp;
        }

        private Dictionary<Point, int> _hiddenMoney;

        public void hideMoney()
        {
            if (!_hiddenMoney.ContainsKey(Position)) _hiddenMoney[Position] = 0;
            _hiddenMoney[Position] += Money;
            Money = 0;
        }

        public void fetchHiddenMoney() {
            if (_hiddenMoney.ContainsKey(Position))
            {
                Money += _hiddenMoney[Position];
                _hiddenMoney[Position] = 0;
            }
        }
    }
}
