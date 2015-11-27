using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Model.DataStructures;
using Game.Model.Rules;

namespace Game.Model.Logic
{
    class PathFinder
    {
        private int[,]  costs;
        private int     width;
        private int     height;
        private Board   board;

        PathFinder(Board board) {
            this.board  = board;
            this.width  = board.Width;
            this.height = board.Height;

            costs = new int[board.Width, board.Height];
        }

        private void resetCosts() {
            int MAX_COST = width * height + 1;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    costs[x, y] = MAX_COST;
                }
            }
        }

        private void updateCosts( Point pos, int cost=0 ) {
            if (pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height) return;

            if (cost < costs[pos.X, pos.Y]) {
                //sätt kostnad:
                costs[pos.X, pos.Y] = cost;
                //kolla grannar:
                Point[] neighbours = new Point[] {
                    new Point(pos.X+1, pos.Y),
                    new Point(pos.X, pos.Y+1),
                    new Point(pos.X-1, pos.Y),
                    new Point(pos.X, pos.Y-1)
                };

                foreach (Point p in neighbours){
                    //TODO: check neighbours
                }
            }
        }
    }
}
