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

        public PathFinder(int width, int height){
            this.width  = width;
            this.height = height;
            this.costs = new int[width, height];
            resetCosts();
        }

        // ärv av denna klass för att skapa en callback-funktion till pathfinder
        public interface CanPass
        {
            bool check(Point position);
        }

        // hjälpfunktioner:
        private void resetCosts(){
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    costs[x, y] = int.MaxValue;
                }
            }
        }
        private bool validIndex(int x, int y){
            return (0 <= x && x < width) && (0 <= y && y < height);
        }
        private bool validIndex(Point p){
            return validIndex(p.X, p.Y);
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
            if (!validIndex(pos.X, pos.Y))
                throw new ArgumentException("invalid start position");

            costs[pos.X, pos.Y] = 0;
            List<Point> position_queue = new List<Point>{ pos };

            while (position_queue.Count != 0)
            {
                // ta bort första elementet i kön och lägg det i current
                Point current = position_queue[0];
                position_queue.RemoveAt(0);

                int new_cost = costs[current.X,current.Y] + 1;

                foreach (Point p in getNeighbourPositions(current))
                {
                    // om en granne har en högre kostnad än vad vi vill sätta så ändrar vi den
                    if ( validIndex(p.X,p.Y) && canPass.check(p) && new_cost < costs[p.X,p.Y] )
                    {
                        // ändra kostnad och lägg över nya koordinaten längst bak i kön
                        costs[p.X, p.Y] = new_cost;
                        position_queue.Add(p);
                    }
                }
            }
        }

        public Point getClosestPointOfInterest(Point pos, List<Point> POIs, CanPass canPass){
            if (!validIndex(pos)) throw new ArgumentException("POI is invalid");
            foreach (Point POI in POIs) { if (!validIndex(POI)) throw new ArgumentException("found invalid point in POIs"); }

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
        public List<Point> getPathWithExactCost(Point from, Point to, int exact_cost, CanPass canPass){
            updateCosts(to, canPass);
            // blockera rutan vi står på:
            costs[from.X, from.Y] = int.MaxValue;
            // kolla om någon av grannarna har en väg till målet:
            foreach ( Point neighbour in getNeighbourPositions(from) ){
                if ( validIndex(neighbour) && costs[neighbour.X, neighbour.Y] == exact_cost - 1)
                {
                    List<Point> path = getShortestPath( neighbour, to, canPass, false );
                    // notera att path innehåller en ruta mer än den totala längden på vägen.
                    if (path != null && path.Count == exact_cost){
                        path.Insert(0, from);
                        return path;
                    }
                }
            }
            // om ingen av grannarna har en väg med rätt kostnad så returneras inget:
            return null;
        }

        public List<Point> getPointsWithExactCost(Point from, int cost, CanPass canPass){
            updateCosts(from, canPass);
            List<Point> points = new List<Point>();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (costs[x, y] == cost) {
                        points.Add(new Point(x, y));
                    }
                }
            }
            return points;
        }



        public List<Point> getShortestPath(Point from, Point to, CanPass canPass)
        {
            return getShortestPath(from, to, canPass, true);
        }

        private List<Point> getShortestPath( Point from, Point to, CanPass canPass, bool recalcPaths ){
            if (!validIndex(from) || !validIndex(to)) throw new ArgumentException("invalid position");
            // spelplanen behöver inte alltid räknas om:
            if (recalcPaths){updateCosts(to, canPass);}
            // om rutan inte kan nås så returneras ingen väg:
            if (costs[from.X, from.Y] == int.MaxValue) return null;

            List<Point> path = new List<Point> { from };
            if (from == to) return path;
            Point last_pos = Point.Error;
            while ( last_pos != path[path.Count-1] ) {
                last_pos = path[path.Count - 1];

                foreach ( Point neighbour in getNeighbourPositions(last_pos) ){
                    if ( validIndex(neighbour) && costs[neighbour.X, neighbour.Y] < costs[last_pos.X, last_pos.Y] ){
                        path.Add(neighbour);
                        break;
                    }
                }
            }

            if (path != null && path.Contains(to)){
                return path;
            } else {
                return null;
            }
            
        }
    }
}
