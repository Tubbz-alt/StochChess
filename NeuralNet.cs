using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessNN
{
    /// <summary>
    /// A Neural network
    /// 
    /// Weights are set to null in 1 of the layer 0 neurons, and in all of the >layer 0 neurons
    /// </summary>
    [Serializable]
    public class NeuralNet : IDisposable
    {
        public Player Player { get; set; }
        public List<Neuron> Neurons = new List<Neuron>();
        public Neuron Output { get; set; }
        public int depth { get; set; }
        public int count { get; set; }
        //Only works at 2
        public int foresight = 2;
        public void initNN()
        {
            try
            {
                Random r = new Random();
                for (int i = 0; i <= depth; i++)
                {
                    if (i == 0)
                    {
                        for (int ii = 0; ii <= count - 1; ii++)
                        {
                            double[,] temps = new double[,]
                            {
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) },
                            {Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)), Sigmoid.sigmoid(r.Next(-9, 9)) }
                            };
                            Neuron n = new Neuron(this, temps, 0, 0);
                        }
                    }
                    if (i >= 1 && i <= depth - 1)
                    {
                        for (int ii = 0; ii <= count - 1; ii++)
                        {
                            Neuron n = new Neuron(this, new Dictionary<Neuron, double>(), 0, i);
                            n.layWeights.Clear();
                            foreach (Neuron neu in Neurons)
                            {
                                if (neu.layer == n.layer - 1)
                                {
                                    if (!n.layWeights.ContainsKey(neu))
                                    {
                                        n.layWeights.Add(neu, /* Weight for the neuron */ Sigmoid.sigmoid(r.Next(0, 999) / 100));
                                    }
                                }
                            }
                        }
                    }
                    if (i == depth)
                    {
                        Neuron n = new Neuron(this, new Dictionary<Neuron, double>(), 0, i);
                        n.layWeights.Clear();
                        foreach (Neuron neu in Neurons)
                        {
                            if (neu.layer == n.layer - 1)
                            {
                                if (!n.layWeights.ContainsKey(neu))
                                {
                                    n.layWeights.Add(neu, /* Weight for the neuron */ Sigmoid.sigmoid(r.Next(0, 999) / 100));
                                }
                            }
                        }
                        Output = n;
                    }
                }
            }
            catch { initNN(); }
        }
        public NeuralNet(Player p, int d, int c)
        {
            Player = p; depth = d; count = c;
        }
        //Need to feed through neurons
        public double Score(Board board, bool isW)
        {
            Piece[,] values = Board.AdjustFlip(board.Pieces, isW);
            foreach (Piece p in values)
            {
                if (p is Empty) { continue; }
                //backwards
                if (p.Player.IsW != isW) { p.CVal = -Math.Abs(p.CVal); }
                else { p.CVal = Math.Abs(p.CVal); }
            }

            foreach (Neuron n in Neurons)
            {
                n.computeCVal(values);
                if (n.layer == 3) { Output = n; }
            }
            //Make less valuable if you're in check
            if (board.amICheck(isW)) { Output.currentVal = -2 * Math.Abs(Output.currentVal); }
            if (board.Stale) { Output.currentVal = -2 * Math.Abs(Output.currentVal); }
            return Output.currentVal;
        }
        public static void Play(Board b)
        {
            Board b2 = GoDiePointers.DeepClone(b);
            Player PW = new Player(true); NeuralNet NNW = new NeuralNet(PW, 3, 10);
            Data.ReadNs(NNW);
            Player PB = new Player(false); NeuralNet NNB = new NeuralNet(PB, 3, 10);
            Data.ReadNs(NNB);
            foreach (Piece p in b2.Pieces)
            {
                if (p is Empty) { continue; }
                if (p.Player.IsW == true) { p.Player = PW; }
                else { p.Player = PB; }
            }
            List<Neuron> BestNeurons = GoDiePointers.DeepClone(NNW.Neurons);

            Random random = new Random();

            //Amount of weights to change
            int changeCount = 5;
            for (int j = 0; j < changeCount; j++)
            {
                //For neurons
                //It increases/decreases the weight by rand.next(x, y)% [normally]
                //It currently is used as an input for the sigmoid (as a randomizing factor)
                double randomVal = random.Next(-14, 14);
                //For pieces
                double pieceRVal = random.Next(1, 19) / 10.00;
                int randNeuron = random.Next(0, NNW.Neurons.Count);
                int randthing = random.Next(1, 2);
                int X = random.Next(0, 7);
                int Y = random.Next(0, 7);
                try
                {
                    if (randthing == 1)
                    {
                        if (BestNeurons[randNeuron].layer == 0)
                        {
                            NNW.Neurons[randNeuron].weights[X, Y] =
                                Sigmoid.sigmoid(randomVal);
                        }
                        else
                        {
                            KeyValuePair<Neuron, double> kvp = NNW.Neurons[randNeuron].layWeights.ElementAt(random.Next(0, NNW.Neurons[randNeuron].layWeights.Count));
                            NNW.Neurons[randNeuron].layWeights[kvp.Key] =
                                Sigmoid.sigmoid(randomVal);
                        }
                    }
                    if (randthing == 2)
                    {
                        if (BestNeurons[randNeuron].layer == 0)
                        {
                            NNB.Neurons[randNeuron].weights[X, Y] = Sigmoid.sigmoid(randomVal);
                        }
                        else
                        {
                            KeyValuePair<Neuron, double> kvp = NNB.Neurons[randNeuron].layWeights.ElementAt(random.Next(0, NNB.Neurons[randNeuron].layWeights.Count));
                            NNB.Neurons[randNeuron].layWeights[kvp.Key] =
                                 Sigmoid.sigmoid(randomVal);
                        }
                    }
                    /*
                     * Disabled for now 
                     * also, it has a 50% chance of selecting the empty squares with the current x/y randomizer
                    //Changing class values?
                    if (randthing == 3)
                    {
                        b.Pieces[X, Y].CVal = (int)(pieceRVal * (GoDiePointers.DeepClone(b.Pieces[X, Y].CVal)));
                    }
                    //Repeat to equalize chances of neuron vs piece
                    if (randthing == 4)
                    {
                        b2.Pieces[Y, X].CVal = (int)(pieceRVal * (GoDiePointers.DeepClone(b.Pieces[X, Y].CVal)));
                    }
                    */
                }
                catch (Exception ex) { Console.WriteLine(ex); return; }
            }

            //At movecap, end playing, and write whoever had a higher score to the weight list file
            int moveCap = 100;
            int i = 1;
            //While it has not moved too many times, and while no-one has won, play
            //Run in parallel?

            //Using two boards to allow for different piece cvals, unless I want to put that into the NN class?
            while (i <= moveCap && !b.WWin && !b.BWin && !b2.WWin && !b2.BWin && !b.Stale && !b2.Stale)
            {
                if (b.WTurn) { b2.Pieces = NNW.Move(b, true).Pieces; Board.PrintBoard(b2); b2.WTurn = false; i++; }
                if (!b2.WTurn) { b.Pieces = NNB.Move(b2, false).Pieces; Board.PrintBoard(b); b.WTurn = true; i++; }
                else { Console.WriteLine("NN Failure"); break; }
            }
            //Will need to check whether pieces read/write properly in the future

            //If white won, write white's data
            if (b.WWin || b2.WWin) { /*Data.WritePieces(b);*/ Data.WriteNs(NNW); }
            //If black won, write black's data
            if (b.BWin || b2.BWin) { /*Data.WritePieces(b2);*/ Data.WriteNs(NNB); }
            else
            {
                //Whoever has the most pieces/points wins (over both boards) [not the best, but easier for now, and not horribly prone to error]
                int wTotal = 0; int bTotal = 0;
                foreach (Piece p in b.Pieces)
                {
                    try
                    {
                        if (!(p is Empty) && p.Player.IsW) { wTotal += Math.Abs(p.CVal); } else { bTotal += Math.Abs(p.CVal); }
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                }
                foreach (Piece p in b2.Pieces)
                {
                    try
                    {
                        if (!(p is Empty) && p.Player.IsW) { wTotal += Math.Abs(p.CVal); } else { bTotal += Math.Abs(p.CVal); }
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                }
                //If white has more points, write its neurons + pieces, vice versa for black
                if (wTotal > bTotal) {/*Data.WritePieces(b);*/ Data.WriteNs(NNW); }
                if (bTotal > wTotal) { /*Data.WritePieces(b2);*/ Data.WriteNs(NNB); }
                //if they tied (incredibly unlikely), then award the one with the larger score
                if (wTotal == bTotal)
                {
                    //If neither won, write the one with a higher (self-percieved) score
                    //May be encouraging a points arms race; it may be wise to sigmoid the scores?
                    if (NNW.Score(b, NNW.Player.IsW) > NNB.Score(b, NNB.Player.IsW))
                    { /*Data.WritePieces(b);*/ Data.WriteNs(NNW); }
                    else { /*Data.WritePieces(b2);*/ Data.WriteNs(NNB); }
                }
            }


            Console.WriteLine("Done");
            b.Dispose(); b2.Dispose(); NNW.Dispose(); NNB.Dispose();
            b = new Board(PW, PB, new Piece[8, 8], true);
            b.Pieces = Board.initBoard(b);
            Play(b);
        }

        public Board Move(Board b, bool isW)
        {
            bool hasKing = false;
            foreach (Piece piece in b.Pieces)
            {
                if (piece is King && piece.Player.IsW == isW) { hasKing = true; break; }
            }
            if (!hasKing)
            {
                if (b.WTurn == true) { b.BWin = true; Console.WriteLine("Black victory!"); return b; }
                if (b.WTurn == false) { b.WWin = true; Console.WriteLine("White victory!"); return b; }
            }

            NeuralNet notMe = GoDiePointers.DeepClone(this);
            notMe.Player.IsW = !isW;
            Data.ReadNs(notMe);

            List<Board> startBoard = new List<Board>();
            if (isW && b.Checks(isW)) { b.WCheck = true; }
            if (!isW && b.Checks(isW)) { b.BCheck = true; }
            startBoard.Add(b);
            List<double> startVal = new List<double>();
            startVal.Add(-99999999);
            //Could be done in a dictionary?
            List<List<Board>> Boards = new List<List<Board>>();
            Boards.Add(startBoard);
            List<List<double>> Values = new List<List<double>>();
            Values.Add(startVal);
            //Create the lists
            for (int i = 0; i < foresight * foresight; i++)
            {
                try
                {
                    //create the boards (is exponential)
                    for (int ii = 0; ii < Boards[i].Count; ii++)
                    {
                        //Define Boards[i]
                        List<Board> bestBoards = new List<Board>();
                        List<double> bestVals = new List<double>();
                        Boards.Add(bestBoards); Values.Add(bestVals);

                        //Check if Boards[i][ii] exists
                        if (Boards[i].Count == 0) { continue; }
                        List<Board> tempbs;
                        //Create + add them to a list
                        if (Boards[i][ii].WTurn == isW) { tempbs = Moves(Boards[i][ii], isW); }
                        else { tempbs = notMe.Moves(Boards[i][ii], !isW); }
                        //Foreach board generated by moves, add it and its score (by this NN) to temps (the main hub of boards + scores)

                        //Sort and add them
                        foreach (Board b2 in tempbs)
                        {
                            if (!b2.Checks(isW))
                            {
                                //Max
                                if (b2.WTurn != isW)
                                {
                                    if (bestBoards.Count == 0) { bestBoards.Add(b2); bestVals.Add(Score(b2, isW)); continue; }
                                    if (bestBoards.Count < foresight)
                                    {
                                        for (int j = 0; j < bestBoards.Count; j++)
                                        {
                                            double b2Score = Score(b2, isW);
                                            if (bestVals.Count >= j && b2Score > bestVals[j]) { bestBoards.Add(b2); bestVals.Add(Score(b2, isW)); continue; }
                                            else
                                            {
                                                if (j > 1) { bestVals[j - 1] = b2Score; bestBoards[j - 1] = b2; continue; }
                                                else { b2.Dispose(); continue; }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        double b2Score = Score(b2, isW);
                                        if (b2Score < bestVals[0]) { b2.Dispose(); continue; }
                                        else
                                        {
                                            for (int j = 0; j < bestBoards.Count; j++)
                                            {
                                                if (b2Score > bestVals[j]) { bestVals[j] = b2Score; bestBoards[j] = b2; continue; }
                                                else
                                                {
                                                    if (j > 1) { bestVals[j - 1] = b2Score; bestBoards[j - 1] = b2; continue; }
                                                    else { b2.Dispose(); continue; }
                                                }

                                            }
                                        }

                                    }
                                }
                                //Min
                                else
                                {
                                    if (bestBoards.Count == 0) { bestBoards.Add(b2); bestVals.Add(Score(b2, isW)); continue; }
                                    if (bestBoards.Count < foresight)
                                    {
                                        for (int j = 0; j < bestBoards.Count; j++)
                                        {
                                            double b2Score = Score(b2, isW);
                                            if (bestVals.Count >= j && b2Score < bestVals[j]) { bestBoards.Add(b2); bestVals.Add(Score(b2, isW)); continue; }
                                            else
                                            {
                                                if (j > 1) { bestVals[j - 1] = b2Score; bestBoards[j - 1] = b2; continue; }
                                                else { b2.Dispose(); continue; }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        double b2Score = Score(b2, isW);
                                        if (b2Score < bestVals[0]) { b2.Dispose(); continue; }
                                        else
                                        {
                                            for (int j = 0; j < bestBoards.Count; j++)
                                            {
                                                if (b2Score < bestVals[j]) { bestVals[j] = b2Score; bestBoards[j] = b2; continue; }
                                                else
                                                {
                                                    if (j > 1) { bestVals[j - 1] = b2Score; bestBoards[j - 1] = b2; continue; }
                                                    else { b2.Dispose(); continue; }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex); Console.WriteLine("i: " + i.ToString() + " Boards.Count(): " + Boards.Count()); break; }
            }
            notMe.Dispose();

            //If stale
            if (Boards.Count <= 1 && !(b.amICheck(isW))) { b.Stale = true; return b; }
            //If you're in check, include kys as an option
            if (Boards.Count <= 1 && b.amICheck(isW))
            {
                List<Board> tempbs = Moves(Boards[0][0], isW);
                //Foreach board generated by moves, add it and its score (by this NN) to temps (the main hub of boards + scores)
                foreach (Board b2 in tempbs)
                {
                    List<Board> b3 = new List<Board>(); List<double> v = new List<double>();
                    b3.Add(b2); v.Add(Score(b2, isW));
                    Boards.Add(b3); Values.Add(v);
                }
            }

            Board bestboard = GoDiePointers.DeepClone(b);
            double bestVal = -9999999;
            //Choose a board from the last set of options
            for (int i = Boards.Count - foresight; i < Boards.Count; i++)
            {
                //If it is undefined, don't use logic on it
                if (Values[i].Count == 0) { continue; }
                //If better, use it-- the first set of moves (don't need 2 for loops because lists are sorted!)
                try
                {
                    if (Values[i][0] > bestVal) { bestVal = Values[i][0]; bestboard = Boards[1][0]; }
                    else { Boards[i][0].Dispose(); }
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
            //May not work
            if (bestboard.amICheck(isW)) { Console.WriteLine("I'm in check"); }
            return bestboard;
        }

        public List<Board> Moves(Board b, bool isW)
        {
            List<Board> Moves = new List<Board>();
            if (b.WTurn != isW)
            { Console.WriteLine("Not my turn"); return Moves; }
            bool invalid;
            for (int j = 0; j <= 7; j++)
            {
                for (int jj = 0; jj <= 7; jj++)
                {
                    Piece piece = b.Pieces[j, jj];
                    if (piece is Empty) { continue; }
                    if (piece.Player.IsW != isW) { continue; }
                    int iFactor;
                    if (isW) { iFactor = -1; }
                    else { iFactor = 1; }
                    if (piece is Pawn)
                    {
                        //Because it is my turn anyway, I can set my remaining pawns' enpass to false (so long as they don't do it this turn)
                        ((Pawn)piece).enPass = false;
                        //x
                        for (int i = 1 * iFactor; Math.Abs(i) <= Math.Abs(2 * iFactor); i = i + iFactor)
                        {   //y
                            for (int ii = -1; ii <= 1; ii++)
                            {
                                invalid = false;
                                Board trialBoard = GoDiePointers.DeepClone(b);
                                try { trialBoard = ((Pawn)trialBoard.Pieces[j, jj]).Move(trialBoard, j + i, jj + ii); }
                                catch { invalid = true; trialBoard.Dispose(); }
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                            }
                        }
                        continue;
                    }
                    if (piece is Rook)
                    {
                        for (int df = -7; df <= 7; df++)
                        {
                            invalid = false;
                            Board trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Rook)trialBoard.Pieces[j, jj]).Move(trialBoard, j + df, jj); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Rook)trialBoard.Pieces[j, jj]).Move(trialBoard, j, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                        }
                        continue;
                    }
                    if (piece is Knight)
                    {
                        for (int dfx = -1 * iFactor; Math.Abs(dfx) <= Math.Abs(2 * iFactor); dfx = dfx + iFactor)
                        {
                            for (int dfy = -1 * iFactor; Math.Abs(dfy) <= Math.Abs(2 * iFactor); dfy = Math.Abs(dfy) + 1)
                            {
                                invalid = false;
                                Board trialBoard = GoDiePointers.DeepClone(b);
                                try { trialBoard = ((Knight)trialBoard.Pieces[j, jj]).Move(trialBoard, j + dfx, jj + dfy); }
                                catch { invalid = true; trialBoard.Dispose(); }
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                            }
                        }
                        continue;
                    }
                    if (piece is Bishop)
                    {
                        for (int df = -7; df <= 7; df++)
                        {
                            invalid = false;
                            Board trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Bishop)trialBoard.Pieces[j, jj]).Move(trialBoard, j + df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Bishop)trialBoard.Pieces[j, jj]).Move(trialBoard, j - df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                        }
                        continue;
                    }
                    if (piece is Queen)
                    {
                        //fixed?
                        for (int df = -7; df <= 7; df++)
                        {
                            invalid = false;
                            Board trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j + df, jj); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (!trialBoard.amICheck(isW) && trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                        }
                        //Bishop
                        for (int df = -7; df <= 7; df++)
                        {
                            invalid = false;
                            Board trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j + df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j - df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                        }
                        continue;
                    }
                    if (piece is King)
                    {
                        for (int dfx = -1; dfx <= 1; dfx++)
                        {
                            for (int dfy = -3; dfy <= 3; dfy++)
                            {
                                invalid = false;
                                Board trialBoard = GoDiePointers.DeepClone(b);
                                try { trialBoard = ((King)trialBoard.Pieces[j, jj]).Move(trialBoard, j + dfx, jj + dfy); }
                                catch { invalid = true; trialBoard.Dispose(); }
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard); }
                            }
                        }
                        continue;
                    }
                }
            }
            return Moves;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NeuralNet() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    [Serializable]
    public class Neuron
    {
        public NeuralNet NN { get; set; }
        public int layer { get; set; }
        public double[,] weights { get; set; }
        public Dictionary<Neuron, double> layWeights = new Dictionary<Neuron, double>();
        public double currentVal;

        public Neuron(NeuralNet nn, double[,] ws, double cval, int lay)
        {
            currentVal = cval; layer = lay;
            NN = nn; nn.Neurons.Add(this);
            weights = ws;
        }
        /// <summary>
        /// Make sure to specify the wets array later! If not, DO NOT use this factory
        /// </summary>
        /// <param name="nn"></param>
        /// <param name="ws"></param>
        /// <param name="cval"></param>
        /// <param name="lay"></param>
        public Neuron(NeuralNet nn, Dictionary<Neuron, double> vals, double cval, int lay)
        {
            layWeights = vals; currentVal = cval; layer = lay; NN = nn;
            nn.Neurons.Add(this);
        }
        /// <summary>
        /// Make sure to specify the vals/weights dict/array later! If not, DO NOT use this factory.
        /// </summary>
        /// <param neuralnet="nn"></param>
        /// <param current value="cval"></param>
        /// <param layer="lay"></param>
        public Neuron(NeuralNet nn, double cval, int lay)
        {
            NN = nn; currentVal = cval; layer = lay; NN.Neurons.Add(this);
            weights = new double[8, 8]; layWeights = new Dictionary<Neuron, double>();
        }
        public void computeCVal(Piece[,] pieces)
        {
            if (layer == 0)
            {
                currentVal = 0;
                for (int i = 0; i <= 7; i++)
                {
                    for (int ii = 0; ii <= 7; ii++)
                    {
                        currentVal += (pieces[i, ii].CVal * weights[i, ii]);
                    }
                }
            }
            if (layer >= 1)
            {
                currentVal = 0;
                foreach (KeyValuePair<Neuron, double> kvp in layWeights)
                {
                    currentVal += kvp.Key.currentVal * kvp.Value;
                }
            }
        }
    }
    class Sigmoid
    {
        public static double sigmoid(double number)
        {
            return 1 / (1 + Math.Pow(Math.E, -number));
        }
    }
}
