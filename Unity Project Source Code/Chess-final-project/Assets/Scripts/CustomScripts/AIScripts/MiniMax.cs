using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Unity.Jobs;


public class MiniMax
{
    //Integer incremented when MinMax() called.
    private int evaluatedPositions;

    public MiniMax()
    {
        evaluatedPositions = 0;
    }

    #region MiniMax Algo  
    /*
     * Key function for MiniMax algorithm.
     * 
     * Returns a tuple - best move value and the corresponding Move info.
     */
    public (int, Move) MinMax(int depth, bool maximisingPlayer, int alpha = -1000000, int beta = 1000000)
    {
        Move bestMove = null;
        int bestMoveVal;

        evaluatedPositions++;
        //If depth = 0, evaluate current game state.
        if(depth == 0)
        {
            int value = (int)Evaluate();

            return (value, bestMove);
        }

        //All possible moves.
        List<Move> moves = GameManager.instance.GenerateAllMoves(maximisingPlayer ?
            GameManager.instance.black : GameManager.instance.white);

        //Sort moves that were generated (Move Ordering) - store in same list.
        moves = SortMoves(moves, maximisingPlayer);

        //Set default move value, before any move is evaluated.
        bestMoveVal = maximisingPlayer ? -1000000 : 1000000;

        if (maximisingPlayer)
        {
            GameManager.instance.currentPlayer = GameManager.instance.black;
            GameManager.instance.otherPlayer = GameManager.instance.white;
        }
        else
        {
            GameManager.instance.currentPlayer = GameManager.instance.white;
            GameManager.instance.otherPlayer = GameManager.instance.black;
        }

        //Iterate through each possible move...
        foreach (Move move in moves)
        {
            bool pieceCaptured = false;
            GameObject captured = null;
            bool pawnMoved = false;

            #region Make Move
            //Used to remove object from GameManager movedPawns list
            if (move.piece.name.Contains("Pawn") && !GameManager.instance.movedPawns.Contains(move.piece))
            {
                pawnMoved = true;
            }

            if (GameManager.instance.PieceAtGrid(move.dest) == null)
            {
                GameManager.instance.Move(move.piece, move.dest, false);
            }
            else //if(!GameManager.instance.PieceAtGrid(move.dest).name.Contains(search))
            {
                GameManager.instance.CapturePieceAt(move.dest, false, maximisingPlayer ? GameManager.instance.black : GameManager.instance.white);
                GameManager.instance.Move(move.piece, move.dest, false);
                pieceCaptured = true;
            }
            #endregion

            //Traverse further down tree (reducing depth) recursively.
            int value = MinMax(depth - 1, !maximisingPlayer, alpha, beta).Item1;

            //If score is better than current, replace bestMoveScore and bestMove with current move
            if (maximisingPlayer)
            {
                if (value > bestMoveVal)
                {
                    bestMoveVal = value;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, value);
            }
            else
            {
                if (value < bestMoveVal)
                {
                    bestMoveVal = value;
                    bestMove = move;
                }
                beta = Math.Min(beta, value);
            }

            #region Undo Move
            GameManager.instance.Move(move.piece, move.source, false);

            if (pieceCaptured)
            {
                GameManager.instance.UndoDelete(captured, move.dest, maximisingPlayer ? GameManager.instance.black : GameManager.instance.white);
            }

            if (pawnMoved)
            {
                GameManager.instance.movedPawns.Remove(move.piece);
            }
            #endregion

            //Alpha-beta pruning, prevent further traversal down tree, if needed.
            if (beta <= alpha)
            {
                break;
            }
        }

        return (bestMoveVal, bestMove);
    }
    #endregion

    #region Board Evaluation
    /*
     * Function to return a numerical score based on current chess pieces and positions.
     */
    private float Evaluate()
    {
        Dictionary<PieceType, int> pieceScores = new Dictionary<PieceType, int>();
        pieceScores.Add(PieceType.Pawn, 10);
        pieceScores.Add(PieceType.Bishop, 300);
        pieceScores.Add(PieceType.Knight, 300);
        pieceScores.Add(PieceType.Rook, 500);
        pieceScores.Add(PieceType.Queen, 900);
        pieceScores.Add(PieceType.King, 9000);

        float pieceScore = 0;

        for(int i = 0; i < GameManager.instance.pieces.GetLength(0); i++)
        {
            for(int j = 0; j < GameManager.instance.pieces.GetLength(1); j++)
            {
                GameObject piece = GameManager.instance.pieces[j, i];

                if(piece != null)
                {
                    Piece pieceScript = piece.GetComponent<Piece>();

                    if (piece.name.Contains("Black"))
                    {
                        float posScore = PositionValue(true, pieceScript, j, i);
                        pieceScore += pieceScores[pieceScript.type];
                        pieceScore += posScore;
                    }
                    else
                    {
                        float posScore = PositionValue(false, pieceScript, j, i);
                        pieceScore -= pieceScores[pieceScript.type];
                        pieceScore -= posScore;
                    }
                }
            }
        }

        return pieceScore;
    }

    private float PositionValue(bool isBlack, Piece piece, int row, int col)
    {
        if (isBlack)
        {
            if(piece.type == PieceType.Pawn)
            {
                return PieceSquareTable.pawnScoreBlack[row, col];
            }
            else if(piece.type == PieceType.Bishop)
            {
                return PieceSquareTable.bishopScoreBlack[row, col];
            }
            else if(piece.type == PieceType.Knight)
            {
                return PieceSquareTable.knightScore[row, col];
            }
            else if(piece.type == PieceType.Rook)
            {
                return PieceSquareTable.rookScoreBlack[row, col];
            }
            else if (piece.type == PieceType.Queen)
            {
                return PieceSquareTable.queenScore[row, col];
            }
            else if (piece.type == PieceType.King)
            {
                return PieceSquareTable.kingScoreBlack[row, col];
            }
        }
        else
        {
            if (piece.type == PieceType.Pawn)
            {
                return PieceSquareTable.pawnScoreWhite[row, col];
            }
            else if (piece.type == PieceType.Bishop)
            {
                return PieceSquareTable.bishopScoreWhite[row, col];
            }
            else if (piece.type == PieceType.Knight)
            {
                return PieceSquareTable.knightScore[row, col];
            }
            else if (piece.type == PieceType.Rook)
            {
                return PieceSquareTable.rookScoreWhite[row, col];
            }
            else if (piece.type == PieceType.Queen)
            {
                return PieceSquareTable.queenScore[row, col];
            }
            else if (piece.type == PieceType.King)
            {
                return PieceSquareTable.kingScoreWhite[row, col];
            }
        }

        //Never reached - function only called when piece matches one of return types above.
        return -1000;
    }
    #endregion

    /*
     * Function used to sort moves (best score moves ordered first)
     * Benefits AB Pruning.
     * 
     * movesToSort - list of Moves generated, to be evaluted.
     */
    private List<Move> SortMoves(List<Move> movesToSort, bool maximisingPlayer)
    {
        int[] scores = new int[movesToSort.Count];

        foreach(Move move in movesToSort)
        {
            #region Make Move
            bool pieceCaptured;
            GameObject captured = null;
            bool pawnMoved = false;

            //Used to remove object from GameManager movedPawns list
            if (move.piece.name.Contains("Pawn") && !GameManager.instance.movedPawns.Contains(move.piece))
            {
                pawnMoved = true;
            }

            if (GameManager.instance.PieceAtGrid(move.dest) == null)
            {
                GameManager.instance.Move(move.piece, move.dest, false);
                pieceCaptured = false;
            }
            else
            {
                GameManager.instance.CapturePieceAt(move.dest, false,GameManager.instance.black);
                GameManager.instance.Move(move.piece, move.dest, false);
                pieceCaptured = true;
            }
            #endregion

            //Store Score
            scores[movesToSort.IndexOf(move)] = (int)Evaluate();

            #region Undo Move
            GameManager.instance.Move(move.piece, move.source, false);

            if (pieceCaptured)
            {
                GameManager.instance.UndoDelete(captured, move.dest, GameManager.instance.black);
            }

            if (pawnMoved)
            {
                GameManager.instance.movedPawns.Remove(move.piece);
            }
            #endregion
        }

        List<Move> newListA = new List<Move>();
        List<Move> newListB = movesToSort;

        //Either search through first 6 items, or whatever the size of the list is (if < 6).
        for(int i = 0; i < Mathf.Min(6, movesToSort.Count); i++)
        {
            int max = -1000000;
            int maxLocation = 0;

            for(int j = 0; j < movesToSort.Count; j++)
            {
                //If new best move has been discovered
                if (scores[j] > max)
                {
                    max = scores[j];
                    maxLocation = j;
                }
            }

            //Now move has been discovered, it does not want to be picked again.
            scores[maxLocation] = -1000000;
            
            newListA.Add(movesToSort.ElementAt(maxLocation));
            newListB.RemoveAt(maxLocation);
        }

        //Append values from newListB to newListA - most promising values located at front.
        newListA.AddRange(newListB);
        return newListA;
    }

    /*
     * Small helper function to output the total number of evaluated calls.
     * Once called, will reset number, ready for next MiniMax call.
     */
    public void EvaluatedMovesCount()
    {
        Debug.Log("Number of Moves: " + evaluatedPositions);
        evaluatedPositions = 0;
    }
}