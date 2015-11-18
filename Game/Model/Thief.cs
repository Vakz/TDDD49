using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    class Thief : Piece
    {
        public int Money { get; set; }

        private int _arrestCount = 0;
        public int ArrestCount
        {
            get
            {
                return _arrestCount;
            }
            private set
            {
                _arrestCount = value;
            }
        }

        public bool Arrestable { get; set; }
        public int ArrestTurns { get; set; }

        public int arrest(int turns) 
        {
            int temp = Money;
            Money = 0;
            _arrestCount++;
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
