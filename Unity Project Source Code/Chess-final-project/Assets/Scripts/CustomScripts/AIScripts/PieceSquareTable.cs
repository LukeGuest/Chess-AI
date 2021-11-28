using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class storing values for each chess piece (Black & White), and a score for it's position on the chess board.
 * 
 * Original concept discovered on: https://www.chessprogramming.org/Simplified_Evaluation_Function
 * Modified values used from: https://www.freecodecamp.org/news/simple-chess-ai-step-by-step-1d55a9266977/
 * 
 * Reducing values leads to less impact given on evaluation - leading to main important revolving around capturing pieces.
 */
public static class PieceSquareTable
{

    public static float[,] pawnScoreBlack = new float[8, 8]
    {
        {   0,     0, 0,    0,    0,  0,     0,    0},
        {   5,     5, 5,    5,    5,  5,     5,    5},
        {   1,     1, 2,    3,    3,  2,     1,    1},
        {0.5f,  0.5f, 1, 2.5f, 2.5f,  1,  0.5f, 0.5f},
        {   0,     0, 0,    2,    2,  0,     0,    0},
        {0.5f, -0.5f,-1,    0,    0, -1, -0.5f, 0.5f},
        {0.5f,     1, 1,   -2,   -2,  1,     1, 0.5f},
        {   0,     0, 0,    0,    0,  0,     0,    0}
    };

    public static float[,] pawnScoreWhite = new float[8, 8]
    { 
        {   0,     0,  0,    0,    0,  0,     0,    0},
        {0.5f,     1,  1,   -2,   -2,  1,     1, 0.5f},
        {0.5f, -0.5f, -1,    0,    0, -1, -0.5f, 0.5f},
        {   0,     0,  0,    2,    2,  0,     0,    0},
        {0.5f,  0.5f,  1, 2.5f, 2.5f,  1,  0.5f, 0.5f},
        {   1,     1,  2,    3,    3,  2,     1,    1},
        {   5,     5,  5,    5,    5,  5,     5,    5},
        {   0,     0,  0,    0,    0,  0,     0,    0}
    };

    public static float[,] bishopScoreBlack = new float[8, 8]
    { 
      {-2,   -1,   -1, -1, -1,   -1,   -1, -2},
      {-1,    0,    0,  0,  0,    0,    0, -1},
      {-1,    0, 0.5f,  1,  1, 0.5f,    0, -1},
      {-1, 0.5f, 0.5f,  1,  1, 0.5f, 0.5f, -1},
      {-1,    0,    1,  1,  1,    1,    0, -1},
      {-1, 1.5f,    1,  1,  1,    1,    1, -1},
      {-1, 0.5f,    0,  0,  0,    0, 0.5f, -1},
      {-2,   -1,   -1, -1, -1,   -1,   -1, -2}
    };

    public static float[,] bishopScoreWhite = new float[8, 8] 
    { 
        {-2,   -1,   -1, -1, -1,   -1,   -1, -2},
        {-1, 0.5f,    0,  0,  0,    0, 0.5f, -1},
        {-1, 1.5f,    1,  1,  1,    1,    1, -1},
        {-1,    0,    1,  1,  1,    1,    0, -1},
        {-1, 0.5f, 0.5f,  1,  1, 0.5f, 0.5f, -1},
        {-1,    0, 0.5f,  1,  1, 0.5f,    0, -1},
        {-1,    0,    0,  0,  0,    0,    0, -1},
        {-2,   -1,   -1, -1, -1,   -1,   -1, -2}
    };

    public static float[,] knightScore = new float[8, 8]
    {
        {-5,   -4,   -3,   -3,   -3,   -3,    -4, -5},
        {-4,   -2,    0,    0,    0,    0,    -2, -4},
        {-3,    0,    1, 1.5f, 1.5f,    1,     0, -3},
        {-3, 0.5f, 1.5f,    2,    2, 1.5f,  0.5f, -3},
        {-3,    0, 1.5f,    2,    2, 1.5f,     0, -3},
        {-3, 0.5f,    1, 1.5f, 1.5f,     1, 0.5f, -3},
        {-4,   -2,    0, 0.5f, 0.5f,     0,   -2, -4},
        {-5, -4,     -3,   -3,   -3,    -3,   -4, -5}
    };

    public static float[,] rookScoreBlack = new float[8, 8]
    { 
        {    0, 0, 0,    0,    0, 0, 0,     0},
        { 0.5f, 1, 1,    1,    1, 1, 1,  0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {    0, 0, 0, 0.5f, 0.5f, 0, 0,     0}
    };

    public static float[,] rookScoreWhite = new float[8, 8]
    { 
        {    0, 0, 0, 0.5f, 0.5f, 0, 0,     0},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        {-0.5f, 0, 0,    0,    0, 0, 0, -0.5f},
        { 0.5f, 1, 1,    1,    1, 1, 1,  0.5f},
        {    0, 0, 0,    0,    0, 0, 0,     0}
    };

    public static float[,] queenScore = new float[8, 8] 
    { 
        {   -2,   -1,   -1, -0.5f, -0.5f,   -1, -1,    -2},
        {   -1,    0,    0,     0,     0,    0,  0,    -1},
        {   -1,    0, 0.5f,  0.5f,  0.5f, 0.5f,  0,    -1},
        {-0.5f,    0, 0.5f,  0.5f,  0.5f, 0.5f,  0, -0.5f},
        {    0,    0, 0.5f,  0.5f,  0.5f, 0.5f,  0, -0.5f},
        {   -1, 0.5f, 0.5f,  0.5f,  0.5f, 0.5f,  0,    -1},
        {   -1,    0, 0.5f,     0,     0,    0,  0,    -1},
        {   -2,   -1,   -1, -0.5f, -0.5f,   -1, -1,    -2}
    };

    public static float[,] kingScoreBlack = new float[8, 8] 
    { 
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-2, -3, -3, -4, -4, -3, -3, -2},
        {-1, -2, -2, -2, -2, -2, -2, -1},
        { 2,  2,  0,  0,  0,  0,  2,  2},
        { 2,  3,  1,  0,  0,  1,  3,  2}
    };

    public static float[,] kingScoreWhite = new float[8, 8] 
    { 
        { 2,  3,  1,  0,  0,  1,  3,  2},
        { 2,  2,  0,  0,  0,  0,  2,  2},
        {-1, -2, -2, -2, -2, -2, -2, -1},
        {-2, -3, -3, -4, -4, -3, -3, -2},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3},
        {-3, -4, -4, -5, -5, -4, -4, -3}
    };
}
