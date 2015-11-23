using UnityEngine;
using System.Collections;

public class Board {

    public int xsize, ysize;
    public int pieceXSize;
    public int pieceYSize;

    Piece[] board = { };
    int[] map = { };
    public Board(int xs, int ys, int px, int py)
    {
        this.xsize = xs;
        this.ysize = ys;
        this.pieceXSize = px;
        this.pieceYSize = py;
        board = new Piece[xsize * ysize];
        map = new int[xsize * ysize];
        setEmptyBoard();
    }

    private void setEmptyBoard()
    {
        for(int i = 0; i < xsize; i++)
        {
            for(int j = 0; j < ysize; j++)
            {
                setCell(i, j, -1);
            }
        }
    }

    public void placeRoom(Room r)
    {
        makeRoom(r.startX, r.startY, r.xLength, r.yLength, 1);
        Debug.Log(showDungeon());
    }


    private bool makeRoom(int x, int y, int xlength, int ylength,
            int direction)
    {
        /*******************************************************************************/

        // define the dimensions of the room, it should be at least 4x4 tiles
        // (2x2 for walking on, the rest is walls)
        int xlen = xlength;//getRand(4, xlength);
        int ylen = ylength;// getRand(4, ylength);

        // the tile type it's going to be filled with
        int floor = 0; // jordgolv..
        int wall = 1; // jordv????gg

        // choose the way it's pointing at
        int dir = 0;
        if (direction > 0 && direction < 4)
            dir = direction;

        switch (dir)
        {

            case 0: // north

                // Check if there's enough space left for it
                for (int ytemp = y; ytemp > (y - ylen); ytemp--)
                {
                    if (ytemp < 0 || ytemp > ysize)
                        return false;
                    for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
                    {
                        if (xtemp < 0 || xtemp > xsize)
                            return false;
                        if (getCell(xtemp, ytemp) != -1)
                            return false; // no space left...
                    }
                }

                // we're still here, build
                for (int ytemp = y; ytemp > (y - ylen); ytemp--)
                {
                    for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
                    {
                        // start with the walls
                        if (xtemp == (x - xlen / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (xtemp == (x + (xlen - 1) / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == y)
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y - ylen + 1))
                            setCell(xtemp, ytemp, wall);
                        // and then fill with the floor
                        else
                            setCell(xtemp, ytemp, floor);
                    }
                }

                break;

            case 1: // east

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    if (ytemp < 0 || ytemp > ysize)
                        return false;
                    for (int xtemp = x; xtemp < (x + xlen); xtemp++)
                    {
                        if (xtemp < 0 || xtemp > xsize)
                            return false;
                        if (getCell(xtemp, ytemp) != -1)
                            return false;
                    }
                }

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    for (int xtemp = x; xtemp < (x + xlen); xtemp++)
                    {
                        if (xtemp == x)
                            setCell(xtemp, ytemp, wall);
                        else if (xtemp == (x + xlen - 1))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y - ylen / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y + (ylen - 1) / 2))
                            setCell(xtemp, ytemp, wall);
                        else
                            setCell(xtemp, ytemp, floor);
                    }
                }

                break;

            case 2: // south

                for (int ytemp = y; ytemp < (y + ylen); ytemp++)
                {
                    if (ytemp < 0 || ytemp > ysize)
                        return false;
                    for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
                    {
                        if (xtemp < 0 || xtemp > xsize)
                            return false;
                        if (getCell(xtemp, ytemp) != -1)
                            return false;
                    }
                }

                for (int ytemp = y; ytemp < (y + ylen); ytemp++)
                {
                    for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
                    {
                        if (xtemp == (x - xlen / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (xtemp == (x + (xlen - 1) / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == y)
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y + ylen - 1))
                            setCell(xtemp, ytemp, wall);
                        else
                            setCell(xtemp, ytemp, floor);
                    }
                }

                break;

            case 3: // west

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    if (ytemp < 0 || ytemp > ysize)
                        return false;
                    for (int xtemp = x; xtemp > (x - xlen); xtemp--)
                    {
                        if (xtemp < 0 || xtemp > xsize)
                            return false;
                        if (getCell(xtemp, ytemp) != -1)
                            return false;
                    }
                }
                

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    for (int xtemp = x; xtemp > (x - xlen); xtemp--)
                    {
                        if (xtemp == x)
                            setCell(xtemp, ytemp, wall);
                        else if (xtemp == (x - xlen + 1))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y - ylen / 2))
                            setCell(xtemp, ytemp, wall);
                        else if (ytemp == (y + (ylen - 1) / 2))
                            setCell(xtemp, ytemp, wall);
                        else
                            setCell(xtemp, ytemp, floor);
                    }
                }

                break;
        }

        return true;
    }

    public void placePiece(Piece p)
    {
        int psx = p.xStart;
        int psy = p.yStart;


    }
    public void setCell(int x, int y, int celltype)
    {
        map[x + xsize * y] = celltype;
    }
    private int getCell(int x, int y)
    {
        return map[x + xsize * y];
    }
    public void setCell(Vector2 v, int ct)
    {
        int x = (int)v.x;
        int y = (int)v.x;
        map[x + xsize * y] = ct;
    }

    string showDungeon()
    {
        /*******************************************************************************/
        // used to print the map on the screen
        string dungeonMap = "";
        for (int y = 0; y < ysize; y++)
        {
            for (int x = 0; x < xsize; x++)
            {
                switch (getCell(x, y))
                {
                    case -1:
                        dungeonMap += "-";
                        break;
                    case 0:
                        dungeonMap += "@";
                        break;
                    case 1:
                        dungeonMap += ".";
                        break;
                    //  case tileBLOCKER:
                    //    dungeonMap += "\"";
                    //  break;
                    case 2:
                        dungeonMap += "#";
                        break;
                    case 3:
                        dungeonMap += ".";
                        break;
                    case 4:
                        dungeonMap += "S";
                        break;
                    case 5:
                        dungeonMap += "E";
                        break;
                        // case tileChest:
                        //   dungeonMap += "*";
                        // break;
                }
            }
            dungeonMap += "\n";
        }
        return dungeonMap;
    }
}
