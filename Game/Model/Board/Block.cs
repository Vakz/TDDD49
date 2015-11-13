using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.Board
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
    Blocked,
    Normal,
    Bank,
    Hideout,
    Telegraph,
    EscapeAirport,
    EscapeCheap,
    TravelAgency,
    PoliceStation,
    TrainStop
}
