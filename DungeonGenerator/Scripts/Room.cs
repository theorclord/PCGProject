using UnityEngine;
using System.Collections;

public class Room {

    public Vector2 cor1, cor2, cor3, cor4;
    public int startX, startY, xLength, yLength;

    public Piece.TYPE type = Piece.TYPE.ROOM;

	public Room(int startx, int starty, int xLength, int yLength)
    {
        
        this.startX = startx;
        this.startY = starty;
        this.xLength = xLength;
        this.yLength = yLength;
        //dungeon_map[x + xsize * y];
        cor1 = new Vector2(startX, startY); // topLeft
        cor2 = new Vector2(startX + xLength, startY); // topRight
        cor3 = new Vector2(startX, startY + yLength); // bottomLeft
        cor4 = new Vector2(startX + xLength, startY + yLength); // bottomRight


    }

    public Room(Vector2 cor1, Vector2 cor2, Vector2 cor3, Vector2 cor4)
    {
        this.cor1 = cor1;
        this.cor2 = cor2;
        this.cor3 = cor3;
        this.cor4 = cor4;
    }
}
