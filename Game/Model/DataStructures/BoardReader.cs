using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    static class BoardReader
    {
        public static Board readBoard(string file)
        {
            List<Piece> pieces = new List<Piece>();
            List<List<string>> boardStrings = readFile(file);
            Board board = new Board(boardStrings[0].Count, boardStrings.Count);
            for (int y = 0; y < boardStrings.Count; ++y)
            {
                for (int x = 0; x < boardStrings[y].Count; ++x)
                {
                    board[x, y] = parseBlock(boardStrings[y][x]);
                }
            }

            return board;
        }

        private static List<List<string>> readFile(string file) {
            List<List<string>> board = new List<List<string>>();
            string line;
            System.IO.StreamReader f = new System.IO.StreamReader(file);
            while ((line = f.ReadLine()) != null)
            {
                board.Add(Regex.Split(line, @"\s+").ToList<string>());
            }
            return board;
        }

        private static Block parseBlock(string block)
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
