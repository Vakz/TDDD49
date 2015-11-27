using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Bank : Block
    {
        private int _money;
        public int Money {
            get {
                return _money;
            }
            protected set
            {
                if (value < 0) throw new ArgumentException("Must have positive amount of monies");
                _money = value;
            }
        }
        public Bank(int money) : base(BlockType.Bank)
        {
            Money = money;
        }

        public int rob() {
            return Money;
        }
    }
}
