using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessNN
{
    class Program
    {
        static Player player1 = new Player(true);
        static Player player2 = new Player(false);
        public static Board ActiveBoard = new Board(player1, player2, new Piece[8, 8], true);
        static void Main(string[] args)
        {
            ActiveBoard.Pieces = Board.initBoard(ActiveBoard);
            Board.PrintBoard(ActiveBoard);
            activeUI();
        }

        public static void activeUI()
        {
            Console.WriteLine("Command?");
            string command = Console.ReadLine();
            try
            {
                if (command[0] == 'm')
                {
                    bool x = int.TryParse(command[1].ToString(), out int result);
                    bool y = int.TryParse(command[2].ToString(), out int result2);
                    bool z = int.TryParse(command[3].ToString(), out int result3);
                    bool q = int.TryParse(command[4].ToString(), out int result4);
                    if (x && y && z && q)
                    {
                        ActiveBoard = (ActiveBoard.Pieces[result, result2]).Move(ActiveBoard, result3, result4);
                    }
                }
                if (command.ToLower() == "learn")
                {
                    NeuralNet.Play(ActiveBoard);
                }
                if (command[0] == 'p' || command[0] == 'P')
                {
                    if (ActiveBoard.WTurn)
                    {
                        NeuralNet NN = new NeuralNet(player1, 3, 10);
                        Data.ReadNs(NN);
                        ActiveBoard = NN.Move(ActiveBoard, NN.Player.IsW);
                    }
                    else
                    {
                        NeuralNet NN = new NeuralNet(player2, 3, 10);
                        Data.ReadNs(NN);
                        ActiveBoard = NN.Move(ActiveBoard, NN.Player.IsW);
                    }
                }
                if (command == "initialize")
                {
                    NeuralNet NN = new NeuralNet(player2, 3, 10);
                    NN.initNN();
                    Data.WriteNs(NN);
                }
                if (command == "load")
                {
                    NeuralNet NN = new NeuralNet(player2, 3, 10);
                    Data.ReadNs(NN);
                    Data.WriteNs(NN);
                }
                //'s' to stop learning
                if (command.Length >= 2 && command[0] == 'p' && command[1] == 'm')
                {
                    bool x = int.TryParse(command[2].ToString(), out int result);
                    bool y = int.TryParse(command[3].ToString(), out int result2);
                    bool z = int.TryParse(command[4].ToString(), out int result3);
                    bool q = int.TryParse(command[5].ToString(), out int result4);
                    if (x && y && z && q)
                    {
                        ActiveBoard.Pieces[result, result2].Move(ActiveBoard, result3, result4);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Failure"); Console.WriteLine(ex); }
            finally
            {
                Board.PrintBoard(ActiveBoard);
                activeUI();
            }
        }
    }

}
