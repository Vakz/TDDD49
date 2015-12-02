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
            int MAX_COST = width * height + 1;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    costs[x, y] = MAX_COST;
                }
            }
        }

        private void updateCosts( Piece p, Point pos, int cost=0 ){
            // ingen får gå utanför kartan:
            if (pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height) return;

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
            updateCosts(p, goal);
            List<Point> path = new List<Point> { p.Position };
            
            Point[] neighbour_offset = new Point[] {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            Point pos = p.Position;
            /* om det inte fungerar helt så kolla *
             * gärna om i ska börja på 1 eller 0  */
            for ( int i = 1; i < costs[p.Position.X,p.Position.Y]; i++ ){
                Point min_pos = pos + neighbour_offset[0];
                for ( int n = 1; n < 4; n++ ){
                    Point neighbour = pos + neighbour_offset[n];
                    if ( costs[neighbour.X, neighbour.Y] < costs[min_pos.X, min_pos.Y] ){
                        min_pos = neighbour;
                    }
                }
                path.Add( min_pos );
            }
            return path;
        }
    }
}
