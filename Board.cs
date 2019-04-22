using System;

namespace ChessNN
{
    [Serializable]
    public class Board : IDisposable
    {
        public Player P1 { get; set; }
        public Player P2 { get; set; }
        public Piece[,] Pieces { get; set; }
        public bool WTurn = true;
        public bool WWin = false;
        public bool BWin = false;
        public Board(Player p1, Player p2, Piece[,] pieces, bool wturn)
        {
            P1 = p1; P2 = p2; Pieces = pieces; WTurn = wturn;
        }
        public static void PrintBoard(Board board)
        {
            string prints = string.Empty;
            string prints2 = string.Empty;
            for (int i = 0; i <= 8; i++)
            {
                if (i != 0) { prints += (i - 1) + " "; }
                else { prints += "  "; }
                for (int ii = 0; ii <= 7; ii++)
                {
                    if (i == 0) { prints += ii + " "; }
                    else
                    {
                        prints += ConvertLetter(board.Pieces[i - 1, ii]) + " ";
                        prints2 += board.Pieces[i - 1, ii].CVal.ToString() + " ";
                    }
                }
                prints += "\n"; prints2 += "\n";
            }
            Console.WriteLine(prints); /* Console.WriteLine(prints2); */
        }
        public static char ConvertLetter(Piece piece)
        {
            if (piece is Empty) { return piece.Name[0]; }
            if (piece.Player.IsW) { if (piece is Knight) { return char.ToUpper(piece.Name[1]); } else { return char.ToUpper(piece.Name[0]); } }
            if (!piece.Player.IsW) { if (piece is Knight) { return char.ToLower(piece.Name[1]); } else { return char.ToLower(piece.Name[0]); } }
            else { return '|'; }
        }
        public static Piece[,] initBoard(Board board)
        {
            Player p1 = board.P1; Player p2 = board.P2;
            Piece[,] tempPieces = new Piece[8, 8]
            {
                { new Rook(p2, 0, 0), new Knight(p2, 0, 1), new Bishop(p2, 0, 2), new Queen(p2, 0, 3), new King(p2, 0, 4), new Bishop(p2, 0, 5), new Knight(p2, 0, 6), new Rook(p2, 0, 7) },
                { new Pawn(p2, 1, 0), new Pawn(p2, 1, 1), new Pawn(p2, 1, 2), new Pawn(p2, 1, 3), new Pawn(p2, 1, 4), new Pawn(p2, 1, 5), new Pawn(p2, 1, 6), new Pawn(p2, 1, 7) },
                { new Empty(2, 0), new Empty(2, 1), new Empty(2, 2), new Empty(2, 3), new Empty(2, 4), new Empty(2, 5), new Empty(2, 6), new Empty(2, 7) },
                { new Empty(3, 0), new Empty(3, 1), new Empty(3, 2), new Empty(3, 3), new Empty(3, 4), new Empty(3, 5), new Empty(3, 6), new Empty(3, 7) },
                { new Empty(4, 0), new Empty(4, 1), new Empty(4, 2), new Empty(4, 3), new Empty(4, 4), new Empty(4, 5), new Empty(4, 6), new Empty(4, 7) },
                { new Empty(5, 0), new Empty(5, 1), new Empty(5, 2), new Empty(5, 3), new Empty(5, 4), new Empty(5, 5), new Empty(5, 6), new Empty(5, 7) },
                { new Pawn(p1, 6, 0), new Pawn(p1, 6, 1), new Pawn(p1, 6, 2), new Pawn(p1, 6, 3), new Pawn(p1, 6, 4), new Pawn(p1, 6, 5), new Pawn(p1, 6, 6), new Pawn(p1, 6, 7) },
                { new Rook(p1, 7, 0), new Knight(p1, 7, 1), new Bishop(p1, 7, 2), new Queen(p1, 7, 3), new King(p1, 7, 4), new Bishop(p1, 7, 5), new Knight(p1, 7, 6), new Rook(p1, 7, 7) }
            };
            return tempPieces;
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
        // ~Board() {
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
}
