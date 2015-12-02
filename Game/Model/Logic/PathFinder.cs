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
        private int[,]      costs;
        private int         width;
        private int         height;
        private Board       board;
        private RuleEngine  rules;

        PathFinder(Board board, RuleEngine rules) {
            this.board  = board;
            this.rules = rules;
            this.width  = board.Width;
            this.height = board.Height;

            costs = new int[board.Width, board.Height];
        }

        private void resetCosts() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    costs[x, y] = int.MaxValue;
                }
            }
        }

        private void updateCosts( Piece p, Point pos, int cost=0 ){
            // ingen får gå utanför kartan:
            if (pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height) return;
            if (costs[pos.X, pos.Y] <= cost) return;

            if ( !rules.canPass( board[pos].Type, p ) ) return;

            if (cost < costs[pos.X, pos.Y]) {
                //sätt kostnad:
                costs[pos.X, pos.Y] = cost;
                //kolla grannar:
                Point[] neighbours = new Point[] {
                    pos + new Point( 1, 0 ), pos + new Point( 0, 1 ),
                    pos + new Point(-1, 0 ), pos + new Point( 0,-1 ),
                };

                foreach (Point pt in neighbours){
                    updateCosts(p, pt, cost + 1);
                }
            }
        }

        public List<Point> getShortestPath( Piece p, Point goal ){
            List<Point> path = new List<Point>();

            resetCosts();
            updateCosts(p, goal);

            if (costs[p.Position.X, p.Position.Y] == int.MaxValue) return path;
            if (p.Position == goal) return path;

            Point[] neighbour_offset = new Point[] {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            /* om det inte fungerar helt så kolla *
             * gärna om i ska börja på 1 eller 0  */
            Point min_pos = p.Position;
            do{
                for ( int n = 0; n < 4; n++ ){
                    Point neighbour = min_pos + neighbour_offset[n];
                    if ( costs[neighbour.X, neighbour.Y] < costs[min_pos.X, min_pos.Y] ){
                        min_pos = neighbour;
                    }
                }
                path.Add( min_pos );
            } while ( min_pos != goal );
            return path;
        }
    }
}
