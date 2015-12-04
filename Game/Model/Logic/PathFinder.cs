using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Model.DataStructures;

namespace Game.Model.Logic
{
    class PathFinder
    {
        private int[,]      costs;
        private int         width;
        private int         height;
        private Board       board;

        PathFinder(Board board) {
            this.board  = board;
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

        public delegate bool canPassCheck( Point pt, Piece p );

        private void updateCosts( Piece p, Point pos, canPassCheck canPass, int cost=0 ){
            // ingen spelare får gå utanför kartan:
            if (pos.X < 0 || pos.X >= width || pos.Y < 0 || pos.Y >= height) return;
            if (costs[pos.X, pos.Y] <= cost) return;

            if ( !canPass( pos, p ) ) return;

            if (cost < costs[pos.X, pos.Y]) { 
                //sätt kostnad:
                costs[pos.X, pos.Y] = cost;
                //kolla grannar:
                Point[] neighbours = new Point[] {
                    pos + new Point( 1, 0 ), pos + new Point( 0, 1 ),
                    pos + new Point(-1, 0 ), pos + new Point( 0,-1 ),
                };

                foreach (Point pt in neighbours){
                    updateCosts(p, pt,canPass, cost + 1);
                }
            }
        }

        public Point getClosestPointOfInterest(Piece p, List<Point> POIs, canPassCheck canPass){
            resetCosts();
            updateCosts(p, p.Position, canPass);

            Point closest_point = Point.error();
            foreach ( Point POI in POIs ){
                try {
                    if (costs[POI.X,POI.Y] < costs[closest_point.X,closest_point.Y]){
                        closest_point = POI;
                    }
                } catch (IndexOutOfRangeException) { /* Ignore error_point */}
            }
            return closest_point;
        
        }
        public List<Point> getPathWithExactCost(Piece p, Point goal, int exact_cost, canPassCheck canPass){
            List<Point> path = new List<Point>();
            resetCosts();
            updateCosts(p, goal, canPass);

            Point[] neighbour_offset = new Point[] {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            foreach ( Point pt in neighbour_offset ){
                Point pos = p.Position+pt;
                if (costs[pos.X, pos.Y] == exact_cost - 1) {
                    // blockera rutan vi stod på:
                    costs[p.Position.X, p.Position.Y] = int.MaxValue;   // istället för detta kan man alternativt använda en ändrad callbackfuktion
                    // hitta kortaste väg från den nya punkten:
                    path = getShortestPath(p, goal, canPass, false);
                    if (path.Count != 0) {
                        path.Insert(0, pos);
                    }
                    return path;
                }
            }

            return new List<Point>();
        }


        public List<Point> getShortestPath( Piece p, Point goal, canPassCheck canPass, bool recalcPaths=true ){
            List<Point> path = new List<Point>();

            if (costs[p.Position.X, p.Position.Y] == int.MaxValue) return path;
            if (p.Position == goal) return path;

            // spelplanen behöver inte alltid räknas om:
            if (recalcPaths){
                resetCosts();
                updateCosts(p, goal, canPass);
            }

            Point[] neighbour_offsets = new Point[] {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            /* om det inte fungerar helt så kolla *
             * gärna om i ska börja på 1 eller 0  */
            Point min_pos = p.Position;
            do{
                Point last_point = min_pos;
                foreach ( Point neighbour_offset in neighbour_offsets ){
                    Point neighbour = min_pos + neighbour_offset;
                    if ( costs[neighbour.X, neighbour.Y] < costs[min_pos.X, min_pos.Y] ){
                        min_pos = neighbour;
                    }
                }
                // ifall koden inte kan hitta vägen ger vi endast en tom väg
                if (min_pos == last_point)
                {
                    return new List<Point>();
                } else {
                    path.Add(min_pos);
                }
            } while ( min_pos != goal );
            return path;
        }
    }
}
