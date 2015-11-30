using UnityEngine;
using System.Collections;

public class Piece {

    public enum TYPE
    {
        DOOR, ROOM, CORRIDOR
    }

    public TYPE type;

    public Vector2 corner1, corner2, corner3, corner4;

    public int xSize;
    public int ySize;
    public int xStart;
    public int yStart;

    public Piece(int xstart, int ystart, int xsize, int ysize)
    {
        this.xSize = xsize;
        this.xStart = xstart;
        this.ySize = ysize;
        this.yStart = ystart;
    }
    

}
