using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Block
    {
        private BlockType _type;
        public BlockType Type {
            get
            {
                return _type;
            }
            protected set {
                _type = value;
            }
        }

        public Block(BlockType type) {
            Type = type;
        }
    }
}
enum BlockType {
    Blocked = 0,
    Normal = 1,
    Bank = 2,
    Hideout = 3,
    Telegraph = 4,
    EscapeAirport = 5,
    EscapeCheap = 6,
    TravelAgency = 7,
    PoliceStation = 8,
    TrainStop = 9
}
