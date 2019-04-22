using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessNN
{
    /// <summary>
    /// An abstract class from which the pieces are derrived
    /// 
    /// Once the neurons are weighted properly, CVal can be regressed back to posVals 
    /// For now, though, weighting takes the priority
    /// ...
    /// Probably...
    /// It may be encapsulated in the weights... I don't know yet.
    /// </summary>
    [Serializable]
    public abstract class Piece
    {
        public string Name { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int LegalX { get; set; }
        public int LegalY { get; set; }
        public int CVal { get; set; }
        public Player Player { get; set; }
        public abstract Board Move(Board b, int toX, int toY);
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    public class Pawn : Piece
    {
        bool enPass, twoStep;
        public Pawn(Player player, int posX, int posY)
        {
            Name = "Pawn"; PosX = posX; PosY = posY; Player = player; twoStep = true; enPass = false;
            if (player.IsW == true) { LegalX = -1; }
            else { LegalX = 1; }
            CVal = Data.ReadPiece("Pawn");
        }
        public override Board Move(Board b, int toX, int toY)
        {
            Board board = GoDiePointers.DeepClone(b);
            bool move = true;
            if (board.Pieces[toX, toY] is Empty || board.Pieces[toX, toY].Player.IsW != Player.IsW)
            {
                //standard move
                if (toX == PosX + LegalX && toY == PosY && toX <= 7 && toY <= 7 && board.Pieces[toX, toY] is Empty)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    move = false; enPass = false; twoStep = false;
                    board.WTurn = !board.WTurn;
                }
                //capture
                if (!(board.Pieces[toX, toY] is Empty) && move && (toY == PosY + 1 || toY == PosY - 1) && toX == PosX + LegalX && toX <= 7 && toY <= 7 &&
                    board.Pieces[toX, toY].Player.IsW != Player.IsW)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    move = false; enPass = false; twoStep = false;
                    board.WTurn = !board.WTurn;
                }
                //twostep
                if (board.Pieces[toX, toY] is Empty && twoStep == true && toX == PosX + (2 * LegalX) && toY == PosY && toX <= 7 && toY <= 7 && move)
                {
                    if (board.Pieces[PosX + LegalX, PosY] is Empty)
                    {
                        board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                        PosX = toX; PosY = toY;
                        board.Pieces.SetValue(this, new int[] { PosX, PosY });
                        twoStep = false; move = false; enPass = true;
                        board.WTurn = !board.WTurn;
                    }
                }
                //enpass
                if ((toY == PosY + 1 || toY == PosY - 1) && toX == PosX + LegalX && toX <= 7 && toY <= 7 && board.Pieces[toX - LegalX, toY] is Pawn &&
                    ((Pawn)board.Pieces[toX - LegalX, toY]).enPass && move && board.Pieces[toX - LegalX, toY].Player.IsW != Player.IsW && board.Pieces[toX, toY] is Empty)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    board.Pieces.SetValue(new Empty(toX - LegalX, toY), new int[] { toX - LegalX, toY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    move = false; twoStep = false;
                    board.WTurn = !board.WTurn;
                }
                if (move) { throw new Exception("Failure of pawn move"); }
            }
            else { throw new Exception("Failure of pawn move"); }
            //Promotion
            if (PosX == 7 || PosX == 0)
            { board.Pieces.SetValue(new Queen(Player, PosX, PosY), new int[] { PosX, PosY }); }

            return board;
        }
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    class Rook : Piece
    {
        public new int LegalX = 7, LegalY = 7; public bool CanCastle = true;
        public Rook(Player player, int posX, int posY)
        {
            Player = player; PosX = posX; PosY = posY; Name = "Rook";
            CVal = Data.ReadPiece("Rook");
        }
        public override Board Move(Board b, int toX, int toY)
        {
            Board board = GoDiePointers.DeepClone(b);
            if (board.Pieces[toX, toY] is Empty || board.Pieces[toX, toY].Player.IsW != Player.IsW)
            {
                bool throughX = false, throughY = false;
                if (toX != PosX)
                {
                    for (int i = 1; i < Math.Abs(PosX - toX); i = Math.Abs(i) + 1)
                    {
                        if (toX < PosX) { i = i * -1; }
                        if (!(board.Pieces[PosX + i, toY] is Empty)) { throughX = true; break; }
                    }
                }
                if (toY != PosY)
                {
                    for (int i = 1; i < Math.Abs(PosY - toY); i = Math.Abs(i) + 1)
                    {
                        if (toY < PosY) { i = i * -1; }
                        if (!(board.Pieces[toX, PosY + i] is Empty)) { throughY = true; break; }
                    }
                }
                //Shift the legal + to outside of the loop for efficiency?
                if (!throughX && !throughY && ((toX <= LegalX && toY == PosY) || (toY <= LegalY && toX == PosX)))
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { toX, toY });
                    CanCastle = false;
                    board.WTurn = !board.WTurn;
                }
                else { throw new Exception("Failure of rook move"); }
                if (throughX || throughY) { throw new Exception("Rook can't move through pieces"); }
            }
            else { throw new Exception("Rook can't move on own pieces"); }
            return board;
        }
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    class Knight : Piece
    {
        public new int LegalX = 2, LegalY = 2;
        public Knight(Player player, int posX, int posY)
        {
            Player = player; PosX = posX; PosY = posY; Name = "Knight";
            CVal = 2;
        }
        public override Board Move(Board b, int toX, int toY)
        {
            Board board = GoDiePointers.DeepClone(b);
            if (board.Pieces[toX, toY] is Empty || board.Pieces[toX, toY].Player.IsW != Player.IsW)
            {
                bool L = false;
                if (Math.Abs(PosX - toX) + Math.Abs(PosY - toY) == 3) { L = true; }
                if (L && toX <= 7 && toY <= 7)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    board.WTurn = !board.WTurn;
                }
                else { throw new Exception("Failure of knight move"); }
            }
            else { throw new Exception("Knight can't move on own pieces"); }
            return board;
        }
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    class Bishop : Piece
    {
        //Should use legalx and legaly in the initializer to make move easier
        public new int LegalX = 7, LegalY = 7;
        public Bishop(Player player, int posX, int posY)
        {
            Player = player; PosX = posX; PosY = posY; Name = "Bishop";
            CVal = Data.ReadPiece("Bishop");
        }
        public override Board Move(Board b, int toX, int toY)
        {
            //Unecessary?
            for (int i = -7; i <= 7; i++)
            {
                if (PosX + i == toX && PosY + i == toY) { break; }
                if (PosX - i == toX && PosY + i == toY) { break; }
                if (i == 7)
                { throw new Exception("Failure of bishop move"); }
            }
            Board board = GoDiePointers.DeepClone(b);
            if (board.Pieces[toX, toY] is Empty || board.Pieces[toX, toY].Player.IsW != Player.IsW)
            {               
                bool throughPiece = false;
                int xFactor = -1; int yFactor = -1;
                if (PosX < toX) { xFactor = 1; }
                if (PosY < toY) { yFactor = 1; }
                if ((Math.Abs(PosX - toX) + Math.Abs(PosY - toY)) % 2 == 0 && toX <= 7 && toY <= 7)
                {
                    for (int i = 1; i <= ((Math.Abs(PosX - toX) + Math.Abs(PosY - toY)) / 2) - 1; i = Math.Abs(i) + 1)
                    {
                        int ii = GoDiePointers.DeepClone(i);
                        i = i * xFactor;
                        ii = ii * yFactor;
                        if (!(board.Pieces[PosX + i, PosY + ii] is Empty))
                        { throughPiece = true; }
                    }
                }
                else { throughPiece = true; throw new Exception("Failure of bishop move"); }
                if (throughPiece) { throw new Exception("Can't move through pieces"); }
                if ((Math.Abs(PosX - toX) + (PosY - toY)) % 2 == 0 && toX <= 7 && toY <= 7 && !throughPiece)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    board.WTurn = !board.WTurn;
                }
                else { throw new Exception("Failure of bishop move"); }
            }
            else { throw new Exception("Bishop can't move on own pieces"); }
            return board;
        }
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    class Queen : Piece
    {
        public new int LegalX = 7, LegalY = 7;
        public Queen(Player player, int posX, int posY)
        {
            Player = player; PosX = posX; PosY = posY; Name = "Queen";
            CVal = Data.ReadPiece("Queen");
        }
        public override Board Move(Board b, int toX, int toY)
        {
            bool rMove = true; bool bMove = true;
            Board board = GoDiePointers.DeepClone(b);
            try
            {
                Rook Qrook = new Rook(Player, PosX, PosY);
                board = Qrook.Move(board, toX, toY);
            }
            catch
            {
                rMove = false;
                try
                {
                    Bishop Qbish = new Bishop(Player, PosX, PosY);
                    board = Qbish.Move(board, toX, toY);
                }
                catch { bMove = false; }
            }
            finally
            {
                board = GoDiePointers.DeepClone(b);
                if (rMove)
                {
                    board.Pieces[PosX, PosY] = new Empty(PosX, PosY);
                    board.Pieces[toX, toY] = new Queen(Player, toX, toY);
                }
                else
                {
                    if (bMove)
                    {
                        board.Pieces[PosX, PosY] = new Empty(PosX, PosY);
                        board.Pieces[toX, toY] = new Queen(Player, toX, toY);
                    }
                    else { throw new Exception("QMove failure"); }
                }
            }
            board.WTurn = !board.WTurn;
            return board;
        }
    }
    /// <summary>
    /// No bugs known
    /// </summary>
    [Serializable]
    class King : Piece
    {
        public new int LegalX = 1, LegalY = 1; public bool CanCastle = true;
        public King(Player player, int posX, int posY)
        {
            Player = player; PosX = posX; PosY = posY; Name = "king"; LegalX = 1; LegalY = 1; CanCastle = true;
            CVal = 9999;
        }
        public override Board Move(Board b, int toX, int toY)
        {
            Board board = GoDiePointers.DeepClone(b);
            if (board.Pieces[toX, toY] is Empty || board.Pieces[toX, toY].Player.IsW != Player.IsW)
            {
                //Castling
                //Does not work in the traditional way where you can't castle when in danger
                if (CanCastle && toX == PosX && toY == 2 || toY == 6)
                {
                    if (toY == 2)
                    {
                        if (board.Pieces[PosX, toY - 2] is Rook && ((Rook)board.Pieces[PosX, toY - 2]).CanCastle)
                        {
                            if (board.Pieces[PosX, toY + 1] is Empty && board.Pieces[PosX, toY - 1] is Empty && board.Pieces[PosX, toY] is Empty)
                            {
                                board.Pieces.SetValue(new Empty(PosX, toY - 2), new int[] { PosX, toY - 2 }); //Rook
                                board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY }); //King
                                board.Pieces.SetValue(new King(Player, PosX, toY), new int[] { PosX, toY }); //King
                                board.Pieces.SetValue(new Rook(Player, PosX, toY + 1), new int[] { PosX, toY + 1 }); //Rook
                                board.WTurn = !board.WTurn;
                                return board;
                            }
                            else
                            {
                                throw new Exception("Can't move through pieces");
                            }
                        }
                        else { throw new Exception("The rook can't castle"); }
                    }
                    if (toY == 6)
                    {
                        if (board.Pieces[PosX, toY + 1] is Rook && ((Rook)board.Pieces[PosX, toY + 1]).CanCastle)
                        {
                            if (board.Pieces[PosX, toY - 1] is Empty && board.Pieces[PosX, toY] is Empty)
                            {
                                board.Pieces.SetValue(new Empty(PosX, toY + 1), new int[] { PosX, toY + 1 }); //Rook
                                board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY }); //King
                                board.Pieces.SetValue(new King(Player, PosX, toY), new int[] { PosX, toY }); //King
                                board.Pieces.SetValue(new Rook(Player, PosX, toY - 1), new int[] { PosX, toY - 1 }); //Rook
                                board.WTurn = !board.WTurn;
                                return board;
                            }
                            else
                            {
                                throw new Exception("Can't move through pieces");
                            }
                        }
                        else { throw new Exception("The rook can't castle"); }
                    }
                }
                if ((toX == PosX + LegalX || toX == PosX - LegalX || toX == PosX) && (toY == PosY + LegalY || toY == PosY - LegalY || toY == PosY) && toX <= 7 && toY <= 7)
                {
                    board.Pieces.SetValue(new Empty(PosX, PosY), new int[] { PosX, PosY });
                    PosX = toX; PosY = toY;
                    board.Pieces.SetValue(this, new int[] { PosX, PosY });
                    CanCastle = false;
                    board.WTurn = !board.WTurn;
                }
                else { throw new Exception("Failure of king move"); }
            }
            else { throw new Exception("Failure of king move"); }
            return board;
        }
    }
    /// <summary>
    /// Will f*** you up if you forget it DOES NOT HAVE A PLAYER! 
    /// Usually occurs when verifying the isW parameter
    /// </summary>
    [Serializable]
    class Empty : Piece
    {
        public Empty(int posX, int posY)
        {
            PosX = posX; PosY = posY; Name = ".empty";
            CVal = Data.ReadPiece("Empty");
        }
        public override Board Move(Board board, int toX, int toY)
        {
            throw new Exception("Can't move nothing");
        }
    }
}