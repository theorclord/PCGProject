using UnityEngine;
using System.Collections;

public class Board {
    public enum MAP_REF
    {
        UNUSED,
        WALL,
        FLOOR,
        DOOR,
        START,
        END
    }
    public int xsize, ysize;
    public int pieceXSize;
    public int pieceYSize;
	ArrayList rooms = new ArrayList ();
	ArrayList doors = new ArrayList ();
	ArrayList corrs = new ArrayList ();

    Piece[] board = { };
    public MAP_REF [,] RefMap
    {
        get;
        private set;
    }
    int[] map = { };
    public Board(int xs, int ys, int px, int py)
    {
        this.xsize = xs;
        this.ysize = ys;
        this.pieceXSize = px;
        this.pieceYSize = py;
        board = new Piece[xsize * ysize];
        RefMap = new MAP_REF[xsize , ysize];
        setEmptyBoard();
    }

    private void setEmptyBoard()
    {
        for(int i = 0; i < xsize; i++)
        {
            for(int j = 0; j < ysize; j++)
            {
                setCell(i, j, MAP_REF.UNUSED);
            }
        }
    }

    public void placeRoom(Room r)
    {
        makeRoom(r.startX, r.startY, r.xLength, r.yLength, 3, r);
		rooms.Add (r);
        Debug.Log(showDungeon());
    }


    private bool makeRoom(int x, int y, int xlength, int ylength,
            int direction, Room r)
    {
        /*******************************************************************************/

        // define the dimensions of the room, it should be at least 4x4 tiles
        // (2x2 for walking on, the rest is walls)
        int xlen = xlength;//getRand(4, xlength);
        int ylen = ylength;// getRand(4, ylength);

        // the tile type it's going to be filled with
        int floor = 0; // jordgolv..
        int wall = 1; // jordv????gg

		// Needs a door somewhere
		int numDoors = 1;
		int setDoors = 0;
		//Q: How should we place the doors? Randomly on any wall, or set as parameter?

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
                        if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
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
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (xtemp == (x + (xlen - 1) / 2))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == y)
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == (y - ylen + 1))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        // and then fill with the floor
                        else
                            setCell(xtemp, ytemp, MAP_REF.FLOOR);
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
                        if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                            return false;
                    }
                }

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    for (int xtemp = x; xtemp < (x + xlen); xtemp++)
                    {
                        if (xtemp == x)
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (xtemp == (x + xlen - 1))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == (y - ylen / 2))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == (y + (ylen - 1) / 2))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else
                            setCell(xtemp, ytemp, MAP_REF.FLOOR);
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
                        if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                            return false;
                    }
                }

                for (int ytemp = y; ytemp < (y + ylen); ytemp++)
                {
                    for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
                    {
                        if (xtemp == (x - xlen / 2))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (xtemp == (x + (xlen - 1) / 2))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == y)
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else if (ytemp == (y + ylen - 1))
                            setCell(xtemp, ytemp, MAP_REF.WALL);
                        else
                            setCell(xtemp, ytemp, MAP_REF.FLOOR);
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
                        if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                            return false;
                    }
                }
                

                for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
                {
                    for (int xtemp = x; xtemp > (x - xlen); xtemp--)
                    {
                        if (xtemp == x){// right side walls
							setCell(xtemp, ytemp, MAP_REF.WALL);
						}
                        else if (xtemp == (x - xlen + 1)){ // left side walls
							setCell(xtemp, ytemp, MAP_REF.WALL);
						}
                        else if (ytemp == (y - ylen / 2)){ // top
							if(xtemp >= x-xlen + 1 && setDoors != numDoors){
							Debug.Log("Making Door");
								Door d = new Door(new Vector2(xtemp, ytemp));
								d.setRoom(r);
								doors.Add(d);
								setDoors++;
								setCell(xtemp,ytemp, MAP_REF.DOOR);
							}else{
                            	setCell(xtemp, ytemp, MAP_REF.WALL);
							Debug.Log("Non Door");
							}
						}
                        else if (ytemp == (y + (ylen - 1) / 2)) {// bottom 
                            setCell(xtemp, ytemp, MAP_REF.WALL);
						}
                        else{
                            setCell(xtemp, ytemp, MAP_REF.FLOOR);
						}
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
    public void setCell(int x, int y, MAP_REF mr)
    {
        RefMap[x, y] = mr;
    }
    private MAP_REF getCell(int x, int y)
    {
        return RefMap[x, y];
    }
    public void setCell(Vector2 v, MAP_REF ct)
    {
        int x = (int)v.x;
        int y = (int)v.x;
        RefMap[x,y] = ct;
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
                    case MAP_REF.UNUSED:
                        dungeonMap += "-";
                        break;
                    case MAP_REF.FLOOR:
                        dungeonMap += "@";
                        break;
                    case MAP_REF.WALL:
                        dungeonMap += ".";
                        break;
                    //  case tileBLOCKER:
                    //    dungeonMap += "\"";
                    //  break;
                    case MAP_REF.DOOR:
                        dungeonMap += "#";
                        break;
                    case MAP_REF.START:
                        dungeonMap += "S";
                        break;
                    case MAP_REF.END:
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

	// THOUGHT: Should make corridor be the same as make room, but with set width?
	private bool makeCorridor(int x, int y, int length, int direction)
	{
		/*******************************************************************************/
		// define the dimensions of the corridor (er.. only the width and
		// height..)

		int len = Random.Range (2, length);
		int floor = 0;
		int dir = 0;
		if (direction > 0 && direction < 4)
			dir = direction;
		
		int xtemp = 0;
		int ytemp = 0;
		
		// reject corridors that are out of bounds
		if (x < 0 || x > xsize)
			return false;
		if (y < 0 || y > ysize)
			return false;
		
		switch (dir)
		{
			
		case 0: // north
			xtemp = x;
			
			// make sure it's not out of the boundaries
			for (ytemp = y; ytemp > (y - len); ytemp--)
			{
				if (ytemp < 0 || ytemp > ysize)
					return false; // oh boho, it was!
				if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
					return false;
			}
			
			// if we're still here, let's start building
			for (ytemp = y; ytemp > (y - len); ytemp--)
			{
				setCell(xtemp, ytemp, MAP_REF.FLOOR);
			}
			break;
			
		case 1: // east
			ytemp = y;
			
			for (xtemp = x; xtemp < (x + len); xtemp++)
			{
				if (xtemp < 0 || xtemp > xsize)
					return false;
				if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
					return false;
			}
			
			for (xtemp = x; xtemp < (x + len); xtemp++)
			{
				setCell(xtemp, ytemp, MAP_REF.FLOOR);
			}
			break;
			
		case 2: // south
			xtemp = x;
			
			for (ytemp = y; ytemp < (y + len); ytemp++)
			{
				if (ytemp < 0 || ytemp > ysize)
					return false;
				if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
					return false;
			}
			
			for (ytemp = y; ytemp < (y + len); ytemp++)
			{
				setCell(xtemp, ytemp, MAP_REF.FLOOR);
			}
			break;
			
		case 3: // west
			ytemp = y;
			
			for (xtemp = x; xtemp > (x - len); xtemp--)
			{
				if (xtemp < 0 || xtemp > xsize)
					return false;
				if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
					return false;
			}
			
			for (xtemp = x; xtemp > (x - len); xtemp--)
			{
				setCell(xtemp, ytemp, MAP_REF.FLOOR);
			}
			break;
		}
		
		return true;
	}
}
