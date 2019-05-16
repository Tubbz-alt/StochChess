using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessNN
{
    public class Data
    {
        private static string Path = @"H:\Desktop\Testing\Pieces.txt";
        private static string NPath = @"H:\Desktop\Testing\Wets.txt";
        //Might convert to a diff return type?
        public static int ReadPiece(string pName)
        {
            //May read them in the wrong order
            FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs);
            int cval = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] splitLine = line.Split(' ');
                if (splitLine[0] == pName)
                {
                    line = sr.ReadLine();
                    splitLine = line.Split(' ');
                    int.TryParse(splitLine[0], out int result);
                    cval = result;
                    break;
                }
            }
            sr.Close(); fs.Close();
            return cval;
        }
        public static void WritePieces(Board board)
        {
            Board b = GoDiePointers.DeepClone(board);
            Board.initBoard(b);
            //Might change fs name?
            FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            int cval = 0;
            for (int i = 1; i <= 6; i++)
            {
                switch (i)
                {
                    case 1: cval = GoDiePointers.DeepClone(b.Pieces[0, 3].CVal); sw.WriteLine("Queen"); break; //Queen
                    case 2: cval = GoDiePointers.DeepClone(b.Pieces[0, 0].CVal); sw.WriteLine("Rook"); break; //Rook
                    case 3: cval = GoDiePointers.DeepClone(b.Pieces[4, 4].CVal); sw.WriteLine("Empty"); break; //Empty
                    case 4: cval = GoDiePointers.DeepClone(b.Pieces[0, 2].CVal); sw.WriteLine("Bishop"); break; //Bishop
                    case 5: cval = GoDiePointers.DeepClone(b.Pieces[1, 3].CVal); sw.WriteLine("Pawn"); break; //Pawn
                    case 6: cval = GoDiePointers.DeepClone(b.Pieces[0, 1].CVal); sw.WriteLine("Knight"); break; //Knight
                }
                sw.Write(Math.Abs(cval).ToString() + ' ');
                sw.WriteLine();
            }
            sw.Close(); fs.Close(); b.Dispose();
        }
        public static void ReadNs(NeuralNet NN)
        {
            NN.Neurons = new List<Neuron>();
            FileStream fs = new FileStream(NPath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] splitLine = line.Split(' ');

                if (splitLine[0] == "Neuron" && splitLine[1] == "0")
                {
                    //fs.Position++;
                    int.TryParse(splitLine[1], out int result);
                    Neuron n = new Neuron(NN, 0, result);
                    //Flip it if it's black to match board perspective
                    for (int i = 0; i <= 7; i++)
                    {
                        line = sr.ReadLine();
                        splitLine = line.Split(' ');
                        for (int ii = 0; ii <= 7; ii++)
                        {
                            try
                            {
                                double.TryParse(splitLine[ii], out double result2);
                                n.weights[i, ii] = result2;
                            }
                            catch (Exception ex) { Console.WriteLine(ex); }
                        }
                    }
                }
                if (splitLine[0] == "Neuron" && splitLine[1] != "0")
                {
                    Dictionary<Neuron, double> layerwets = new Dictionary<Neuron, double>();
                    int.TryParse(splitLine[1], out int result);
                    line = sr.ReadLine();
                    splitLine = line.Split(' ');

                    //MAGIC CODE, DO NOT TOUCH

                    for (int i = (splitLine.Count() * (result - 1)); i <= (splitLine.Count() * result) - 1; i++)
                    {
                        if (!layerwets.ContainsKey(NN.Neurons[i]))
                        {
                            if (result == NN.Neurons[i].layer + 1)
                            {
                                double.TryParse(splitLine[i - (splitLine.Count() * (result - 1))], out double result2);
                                layerwets.Add(NN.Neurons[i], result2);
                            }
                        }
                        else { Console.WriteLine("Failure to copy neuron " + NN.Neurons[i].ToString() + " " + NN.Neurons[i].layer.ToString()); }
                    }
                    Neuron n = new Neuron(NN, layerwets, 0, result);
                    if (result == 3) { NN.Output = n; }
                }
            }
            sr.Close(); fs.Close();
        }
        public static void WriteNs(NeuralNet NN)
        {
            FileStream fs = new FileStream(NPath, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);
            foreach (Neuron n in NN.Neurons)
            {
                sw.WriteLine("Neuron" + ' ' + n.layer);
                if (n.layer == 0)
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        for (int ii = 0; ii <= 7; ii++)
                        {
                            sw.Write(Math.Abs(n.weights[i, ii]).ToString());
                            if (ii < 7) { sw.Write(" "); }
                        }
                        sw.WriteLine();
                    }
                }
                else
                {
                    int count = 1;
                    foreach (KeyValuePair<Neuron, double> kvp in n.layWeights)
                    {
                        sw.Write(Math.Abs(kvp.Value).ToString());
                        if (count < n.layWeights.Count()) { sw.Write(" "); }
                        count++;
                    }
                    sw.WriteLine();
                }
            }
            sw.Close(); fs.Close();
        }
    }
}
