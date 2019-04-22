using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessNN
{
    [Serializable]
    public class Player
    {
        public bool AI { get; set; }
        public bool IsW { get; set; }
        public NeuralNet NN { get; set; }
        public Player(bool isw)
        {
            IsW = isw;
        }
    }
}
