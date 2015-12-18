using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    public class Thief : Piece
    {
        public int Money { get; set; }

        public bool Arrestable { get; set; }
        public int ArrestTurns { get; set; }
        public int ArrestCount { get; set; }


        public Thief(int id, Point p) : base(id, p)
        {
            Type = PieceType.Thief;
            HiddenMoney = new Dictionary<Point, int>();
        }

        public int arrest(int turns) 
        {
            int temp = Money;
            Money = 0;
            ArrestCount++;
            Arrestable = false;
            ArrestTurns = turns;
            Active = false;
            return temp;
        }

        public Dictionary<Point, int> HiddenMoney { get; set; }

        public void hideMoney()
        {
            if (!HiddenMoney.ContainsKey(Position)) HiddenMoney[Position] = 0;
            HiddenMoney[Position] += Money;
            Money = 0;
        }

        public void fetchHiddenMoney() {
            if (HiddenMoney.ContainsKey(Position))
            {
                Money += HiddenMoney[Position];
                HiddenMoney[Position] = 0;
            }
        }
    }
}
