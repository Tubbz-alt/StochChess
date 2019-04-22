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
    /// Currently non-disposable, should be fixed.
    /// </summary>
    [Serializable]
    public class NeuralNet : IDisposable
    {
        public Player Player { get; set; }
        public List<Neuron> Neurons = new List<Neuron>();
        public Neuron Output { get; set; }
        public int depth { get; set; }
        public int count { get; set; }
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
            Piece[,] values = GoDiePointers.DeepClone(board.Pieces);
            foreach (Piece p in values)
            {
                if (p is Empty) { continue; }
                //backwards
                if (p.Player.IsW != isW) { p.CVal = -Math.Abs(p.CVal); }
                else { p.CVal = Math.Abs(p.CVal); }
            }
            double score = 0;
            foreach (Neuron n in Neurons)
            {
                n.computeCVal(values);
                if (n.layer == 3) { Output = n; }
            }
            score = Output.currentVal;
            return score;
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
            List<Neuron> BestNeurons = new List<Neuron>();
            BestNeurons = GoDiePointers.DeepClone(NNW.Neurons);

            Random random = new Random();
           
            //Amount of weights to change
            int changeCount = 5;
            for (int j = 0; j <= changeCount; j++)
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
            int moveCap = 30;
            int i = 1;
            //While it has not moved too many times, and while no-one has won, play
            //Run in parallel?
            while (i <= moveCap && !b.WWin && !b.BWin && !b2.WWin && !b2.BWin)
            {
                if (b.WTurn) { b2.Pieces = NNW.Move(b, NNW.Player.IsW).Pieces; Board.PrintBoard(b2); b2.WTurn = GoDiePointers.DeepClone(!b.WTurn); i++; }
                if (!b2.WTurn) { b.Pieces = NNB.Move(b2, NNB.Player.IsW).Pieces; Board.PrintBoard(b); b.WTurn = GoDiePointers.DeepClone(!b2.WTurn); i++; }
                else { Console.WriteLine("NN Failure"); break; }
            }

            //Will need to check whether pieces write properly in the future

            //If white won, write white's data
            if (b.WWin || b2.WWin) { /*Data.WritePieces(b);*/ Data.WriteNs(NNW); }
            else
            {
                //Elif black won, write black's data
                if (b.BWin || b2.BWin) { /*Data.WritePieces(b2);*/ Data.WriteNs(NNB); }
                else
                {
                    //If neither won, write the one with a higher (self-percieved) score
                    //May be encouraging a points arms race, it may be wise to sigmoid the scores?
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

            List<Board> Boards = new List<Board>();
            List<double> Vals = new List<double>();
            NeuralNet notMe = GoDiePointers.DeepClone(this);
            notMe.Player.IsW = !isW;
            Dictionary<Board, double> moves = new Dictionary<Board, double>();

            List<Board> starterBoards = new List<Board>();
            Dictionary<Board, double> starterMoves = refineMoves(Moves(b, isW), foresight);
            moves = GoDiePointers.DeepClone(starterMoves);

            foreach (KeyValuePair<Board, double> kvp in moves)
            {
                Boards.Add(kvp.Key); Vals.Add(kvp.Value);
                starterBoards.Add(kvp.Key);
            }

            for (int i = 0; i < Vals.Count; i++)
            {
                for (int ii = 0; ii < Vals.Count; ii++)
                {
                    try
                    {
                        if (Boards[ii].WTurn == Player.IsW) { moves = refineMoves(Moves(Boards[ii], isW), foresight); }
                        else { moves = refineMoves(notMe.Moves(Boards[ii], notMe.Player.IsW), foresight); }
                    }
                    catch (ArgumentOutOfRangeException argsEx) { Console.WriteLine(argsEx); }
                    Vals[ii] = -9999;
                    foreach (KeyValuePair<Board, double> kvp in moves)
                    {
                        if (Vals[ii] <= kvp.Value) { Vals[ii] = kvp.Value; Boards[ii] = kvp.Key; }
                        else { kvp.Key.Dispose(); }
                    }
                }
            }

            for (int i = 0; i < Vals.Count; i++)
            {
                if (Vals[i] > Vals[0]) { Vals[0] = Vals[i]; Boards[i] = starterBoards[i]; }
                else { Boards[i].Dispose(); }
            }
            notMe.Dispose(); return starterBoards[0]; 
        }
        public Dictionary<Board, double> refineMoves(Dictionary<Board, double> Moves, int Depth)
        {
            Dictionary<Board, double> kvps = new Dictionary<Board, double>();
            List<Board> boards = new List<Board>();
            List<double> values = new List<double>();
            foreach (KeyValuePair<Board, double> Move in Moves)
            {
                if (values.Count() <= Depth) { values.Add(Move.Value); boards.Add(Move.Key); }
                else
                {
                    for (int i = 0; i < values.Count() - 1; i++)
                    {
                        if (Move.Value > values[i]) { values[i] = Move.Value; boards[i] = Move.Key; }
                        else { Move.Key.Dispose(); }
                    }
                }
            }

            for (int i = 0; i < boards.Count(); i++)
            {
                if (!kvps.ContainsKey(boards[i]))
                { kvps.Add(boards[i], values[i]); }
            }
            return kvps;
        }
        public Dictionary<Board, double> Moves(Board b, bool isW)
        {
            Dictionary<Board, double> Moves = new Dictionary<Board, double>();
            if (b.WTurn != isW) { Console.WriteLine("Not my turn"); return Moves; }
            Board bestBoard = GoDiePointers.DeepClone(b);
            bool invalid = false;
            for (int j = 0; j <= 7; j++)
            {
                for (int jj = 0; jj <= 7; jj++)
                {
                    Piece piece = b.Pieces[j, jj];
                    if (piece is Empty) { continue; }
                    else { if (piece.Player.IsW != isW) { continue; } }
                    int iFactor;
                    if (isW) { iFactor = -1; }
                    else { iFactor = 1; }
                    if (piece is Pawn)
                    {   //x
                        for (int i = 1 * iFactor; Math.Abs(i) <= Math.Abs(2 * iFactor); i = i + iFactor)
                        {   //y
                            for (int ii = -1; ii <= 1; ii++)
                            {
                                invalid = false;
                                Board trialBoard = GoDiePointers.DeepClone(b);
                                try { trialBoard = ((Pawn)trialBoard.Pieces[j, jj]).Move(trialBoard, j + i, jj + ii); }
                                catch { invalid = true; trialBoard.Dispose(); }
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Rook)trialBoard.Pieces[j, jj]).Move(trialBoard, j, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Bishop)trialBoard.Pieces[j, jj]).Move(trialBoard, j - df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
                        }
                        //Bishop
                        for (int df = -7; df <= 7; df++)
                        {
                            invalid = false;
                            Board trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j + df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }

                            invalid = false;
                            trialBoard = GoDiePointers.DeepClone(b);
                            try { trialBoard = ((Queen)trialBoard.Pieces[j, jj]).Move(trialBoard, j - df, jj + df); }
                            catch { invalid = true; trialBoard.Dispose(); }
                            if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
                                if (trialBoard.Pieces != b.Pieces && !invalid) { Moves.Add(trialBoard, Score(trialBoard, isW)); }
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
