using System;
using System.Collections.Generic;

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
        public bool WCheck = false;
        public bool BCheck = false;
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
        
        /// <summary>
        /// Checks if one is in check
        /// </summary>
        /// <param isW?="isW"></param>
        /// <returns></returns>
        public bool amICheck(bool isW)
        {
            if (isW) { if (WCheck) { return true; } }
            else { if (BCheck) { return true; } }
            return false;
        }

        /// <summary>
        /// [OLD] Update the check values for each king
        /// </summary>
        /*
        public void Checks()
        {
            WCheck = true; BCheck = true;
            foreach (Piece p in Pieces)
            {
                if (p is King)
                {
                    if (p.Player.IsW) { WCheck = false; }
                    else { BCheck = false; }
                }
            }
        }
        */

        /// <summary>
        /// Check if king is in check
        /// </summary>
        public void Checks(bool isW)
        {
            WCheck = false; BCheck = false;
            foreach (Piece p in Pieces)
            {
                //Must be done in this SUPER verbose manner to prevent it registering pieces as threats from behind others (it has to break each loop separately)
                if (p is King && p.Player.IsW == isW)
                {
                    //Orthagonal

                    //left
                    for (int ii = 0; ii <= 7; ii++)
                    {
                        try
                        {
                            if (Pieces[p.PosX + ii, p.PosY].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX + ii, p.PosY] is Pawn) { break; }
                            if (Pieces[p.PosX + ii, p.PosY] is Rook || Pieces[p.PosX + ii, p.PosY] is Queen || Pieces[p.PosX + ii, p.PosY] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        //If it hits the sides, break;
                        catch { break; }
                    }
                    //right
                    for (int ii = 0; ii >= -7; ii--)
                    {
                        try
                        {
                            if (Pieces[p.PosX + ii, p.PosY].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX + ii, p.PosY] is Pawn) { break; }
                            if (Pieces[p.PosX + ii, p.PosY] is Rook || Pieces[p.PosX + ii, p.PosY] is Queen || Pieces[p.PosX + ii, p.PosY] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                    //up
                    for (int ii = 0; ii <= 7; ii++)
                    {
                        try
                        {
                            if (Pieces[p.PosX, p.PosY + ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX, p.PosY + ii] is Pawn) { break; }
                            if (Pieces[p.PosX, p.PosY + ii] is Rook || Pieces[p.PosX, p.PosY + ii] is Queen || Pieces[p.PosX, p.PosY + ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                    //down
                    for (int ii = 0; ii >= -7; ii--)
                    {
                        try
                        {
                            if (Pieces[p.PosX, p.PosY + ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX, p.PosY + ii] is Pawn) { break; }
                            if (Pieces[p.PosX, p.PosY + ii] is Rook || Pieces[p.PosX, p.PosY + ii] is Queen || Pieces[p.PosX, p.PosY + ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }

                    //Diagonal

                    //up left
                    for (int ii = 0; ii <= 7; ii++)
                    {
                        try
                        {
                            if (Pieces[p.PosX + ii, p.PosY + ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX + ii, p.PosY + ii] is Pawn)
                            {
                                if (ii == 1 && !p.Player.IsW) { BCheck = true; break; }
                                else { break; }
                            }
                            if (Pieces[p.PosX + ii, p.PosY + ii] is Bishop || Pieces[p.PosX + ii, p.PosY + ii] is Queen || Pieces[p.PosX + ii, p.PosY + ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                    //up right
                    for (int ii = 0; ii <= 7; ii++)
                    {
                        try
                        {
                            if (Pieces[p.PosX + ii, p.PosY + ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX + ii, p.PosY + ii] is Pawn)
                            {
                                if (ii == 1 && !p.Player.IsW) { BCheck = true; break; }
                                else { break; }
                            }
                            if (Pieces[p.PosX - ii, p.PosY + ii] is Bishop || Pieces[p.PosX - ii, p.PosY + ii] is Queen || Pieces[p.PosX - ii, p.PosY + ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                    //down left
                    for (int ii = 0; ii >= -7; ii--)
                    {
                        try
                        {
                            if (Pieces[p.PosX + ii, p.PosY - ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX + ii, p.PosY - ii] is Pawn)
                            {
                                if (ii == -1 && p.Player.IsW) { WCheck = true; break; }
                                else { break; }
                            }
                            if (Pieces[p.PosX + ii, p.PosY - ii] is Bishop || Pieces[p.PosX + ii, p.PosY - ii] is Queen || Pieces[p.PosX + ii, p.PosY - ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                    //down right
                    for (int ii = 0; ii >= -7; ii--)
                    {
                        try
                        {
                            if (Pieces[p.PosX - ii, p.PosY + ii].Player.IsW == p.Player.IsW) { break; }
                            if (Pieces[p.PosX - ii, p.PosY + ii] is Pawn)
                            {
                                if (ii == -1 && p.Player.IsW) { WCheck = true; break; }
                                else { break; }
                            }
                            if (Pieces[p.PosX - ii, p.PosY + ii] is Bishop || Pieces[p.PosX - ii, p.PosY + ii] is Queen || Pieces[p.PosX - ii, p.PosY + ii] is King)
                            { if (p.Player.IsW) { WCheck = true; } else { BCheck = true; } break; }
                        }
                        catch { break; }
                    }
                }
            }
            if (WCheck) { Console.WriteLine("White is in check"); }
            if (BCheck) { Console.WriteLine("Black is in check"); }
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

