using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class BoardReader
    {
        private List<Point> policeStation;
        private Point travelAgency;

        public Board readBoard(string file, int nrOfPlayers)
        {
            List<Piece> pieces = new List<Piece>();
            List<List<string>> boardStrings = readFile(file);
            Board board = new Board(boardStrings[0].Count, boardStrings.Count, pieces);
            for (int y = 0; y < boardStrings.Count; ++y)
            {
                for (int x = 0; x < boardStrings[y].Count; ++x)
                {
                    board[x, y] = parseBlock(boardStrings[y][x]);
                    if (board[x, y].Type == BlockType.TravelAgency) travelAgency = new Point(x, y);
                    if (board[x, y].Type == BlockType.PoliceStation) policeStation.Add(new Point(x, y));
                }
            }

            return board;
        }

        private List<List<string>> readFile(string file) {
            List<List<string>> board = new List<List<string>>();
            string line;
            System.IO.StreamReader f = new System.IO.StreamReader(file);
            while ((line = f.ReadLine()) != null)
            {
                board.Add(line.Split().ToList<string>());
            }
            return board;
        }

        private Block parseBlock(string block)
        {
            switch (block[0])
            {
                case 'l':
                    List<int> stations = new List<int>();
                    for (int i = 1; i < block.Length; ++i)
                    {
                        stations.Add(int.Parse(block[i].ToString()));
                    }
                    return new TrainStop(stations);
                case 'b':
                    return new Bank(int.Parse(block.Substring(1)) * 1000);
                case '7':
                    return new TravelAgency();
                case '5':
                    return new Escape(3000, BlockType.EscapeAirport);
                case '6':
                    return new Escape(1000, BlockType.EscapeCheap);
                default:
                    return new Block((BlockType)int.Parse(block));
            }
        }
    }
}
