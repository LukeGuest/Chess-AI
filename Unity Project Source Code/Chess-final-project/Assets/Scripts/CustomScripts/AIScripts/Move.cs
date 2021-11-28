using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class used to represent a chess move in the form of an object.
 * 
 * piece - the chess piece being moved.
 * source - the current position of the chess piece.
 * dest - the position the chess piece is moving to.
 */
public class Move
{
    public Vector2Int source;
    public Vector2Int dest;
    public GameObject piece;

    public Move(Vector2Int src, Vector2Int dst, GameObject piece)
    {
        this.source = src;
        this.dest = dst;
        this.piece = piece;
    }
}
