using UnityEngine;
using System.Collections;

public class Piece {

    public enum TYPE
    {
        DOOR, ROOM, CORRIDOR
    }

    TYPE t;

    int xSize;
    int ySize;
    int xStart;
    int yStart;

    public Piece(int xstart, int ystart, int xsize, int ysize)
    {
        this.xSize = xsize;
        this.xStart = xstart;
        this.ySize = ysize;
        this.yStart = ystart;
    }
    

}
