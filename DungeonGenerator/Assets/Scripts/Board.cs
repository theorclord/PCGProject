using UnityEngine;
using System.Collections;

public class Board {

    int xSize, ySize;
    int pieceXSize;
    int pieceYSize;

    Piece[][] board;
    public Board(int xs, int ys, int px, int py)
    {
        this.xSize = xs;
        this.ySize = ys;
        this.pieceXSize = px;
        this.pieceYSize = py;
    }

}
