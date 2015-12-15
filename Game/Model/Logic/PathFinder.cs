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

        public PathFinder(int width, int height){
            this.width  = width;
            this.height = height;
            this.costs = new int[width, height];
            resetCosts();
        }

        private void resetCosts(){
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    costs[x, y] = int.MaxValue;
                }
            }
        }

        private bool validIndex(int x, int y)
        {
            return (0 <= x && x < width) && (0 <= y && y < height);
        }


        public class CanPass {
            protected CanPass() {}
            public virtual bool check(Point position) { throw new NotImplementedException("override this method"); }
        }

        private List<Point> getNeighbourPositions( Point pos ){
            return new List<Point>
            {
                pos + new Point(  1, 0 ),
                pos + new Point( -1, 0 ),
                pos + new Point(  0, 1 ),
                pos + new Point(  0,-1 )
            };
        }

        private void updateCosts( Point pos, CanPass canPass ){
            resetCosts();
            // ingen spelare får gå utanför kartan:
            if (!validIndex(pos.X, pos.Y)) return;

            costs[pos.X, pos.Y] = 0;
            List<Point> position_queue = new List<Point>{ pos };

            while (position_queue.Count != 0)
            {
                Point current = position_queue[0];
                position_queue.RemoveAt(0);
                int new_cost = costs[current.X,current.Y] + 1;
                foreach (Point p in getNeighbourPositions(current))
                {
                    if ( validIndex(p.X,p.Y) && canPass.check(p) && new_cost < costs[p.X,p.Y] )
                    {
                        costs[p.X, p.Y] = new_cost;
                        position_queue.Insert(position_queue.Count - 1, p);
                    }
                }
            }
        }

        public Point getClosestPointOfInterest(Point pos, List<Point> POIs, CanPass canPass){
            updateCosts(pos, canPass);

            Point closest_point = Point.Error;
            int   closest_cost  = int.MaxValue;

            foreach ( Point POI in POIs ){
                int POI_cost = costs[POI.X, POI.Y];

                if ( POI_cost < closest_cost ){
                    closest_point = POI;
                    closest_cost  = POI_cost;
                }
            }
            return closest_point;
        
        }
        public List<Point> getPathWithExactCost(Point current_pos, Point goal, int exact_cost, CanPass canPass){
            List<Point> path = new List<Point>();
            resetCosts();
            updateCosts(goal, canPass);

            Point[] neighbour_offset = new Point[]
            {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            foreach ( Point pt in neighbour_offset ){
                Point new_pos = current_pos + pt;
                if (costs[new_pos.X, new_pos.Y] == exact_cost - 1)
                {
                    // blockera rutan vi stod på:
                    costs[current_pos.X, current_pos.Y] = int.MaxValue;   // istället för detta kan man alternativt använda en ändrad callbackfuktion
                    // hitta kortaste väg från den nya punkten:
                    path = getShortestPath( current_pos, goal, canPass, false );
                    if (path.Count != 0){
                        path.Insert(0, new_pos);
                    }
                    return path;
                }
            }

            return new List<Point>();
        }

        public List<Point> getShortestPath(Point current_pos, Point goal, CanPass canPass)
        {
            return getShortestPath(current_pos, goal, canPass, true);
        }

        private List<Point> getShortestPath( Point current_pos, Point goal, CanPass canPass, bool recalcPaths ){
            List<Point> path = new List<Point>();

            if (costs[current_pos.X, current_pos.Y] == int.MaxValue) return path;
            if (current_pos == goal) return path;

            // spelplanen behöver inte alltid räknas om:
            if (recalcPaths){
                resetCosts();
                updateCosts(goal, canPass);
            }

            Point[] neighbour_offsets = new Point[]
            {
                new Point( 1, 0 ), new Point( 0, 1 ),
                new Point(-1, 0 ), new Point( 0,-1 ),
            };

            /* om det inte fungerar helt så kolla *
             * gärna om i ska börja på 1 eller 0  */
            Point min_pos = current_pos;
            do {
                Point last_point = min_pos;
                foreach ( Point neighbour_offset in neighbour_offsets ){
                    Point neighbour = min_pos + neighbour_offset;
                    if ( costs[neighbour.X, neighbour.Y] < costs[min_pos.X, min_pos.Y] ){
                        min_pos = neighbour;
                    }
                }
                // ifall koden inte kan hitta vägen ger vi endast en tom väg
                if (min_pos == last_point){
                    return new List<Point>();
                } else {
                    path.Add(min_pos);
                }
            } while ( min_pos != goal );
            return path;
        }
    }
}
