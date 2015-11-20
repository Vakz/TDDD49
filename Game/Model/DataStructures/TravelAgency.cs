using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class TravelAgency : Bank
    {
        public TravelAgency()
            : base(0)
        {
            Type = BlockType.TravelAgency;
        }

        void addMoney(int money)
        {
            Money += money;
        }
    }
}
