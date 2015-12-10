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
    public enum DIR
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
    public int xsize, ysize;
    public int pieceXSize;
    public int pieceYSize;
	ArrayList rooms = new ArrayList ();
	ArrayList doors = new ArrayList ();
    //ArrayList corrs = new ArrayList ();

    // Piece[] board = { };
    //int[] map = { };

    private int minLength;
    private int maxLength;
    public MAP_REF [,] RefMap
    {
        get;
        private set;
    }
    public Board(int xs, int ys, int px, int py, int minLength, int maxLength)
    {
        this.xsize = xs;
        this.ysize = ys;
        this.pieceXSize = px;
        this.pieceYSize = py;
        this.minLength = minLength;
        this.maxLength = maxLength;
        //board = new Piece[xsize * ysize];
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
	public bool placeRoom(Room r, int dir, Vector2 ent, ArrayList doors, int numberOfDoors)
	{
        Debug.Log("Setting Room");
        bool res = false;
        // from GameController, vector has +1 or -1 offset in the direction.
        //makeRoom(r.startX, r.startY, r.xLength, r.yLength, dir, r, dir);
        res = makeFitRoom(r.xLength, r.yLength, r, dir, ent, doors, numberOfDoors);
        /*switch (4)
        {
            case 0://north
                res = makeNorthRoom(r.startX, r.startY, r.xLength, r.yLength, r, dir);
                break;
            case 1:
                res = makeEastRoom(r.startX, r.startY, r.xLength, r.yLength, r, dir);
                break;
            case 2:
                res = makeSouthRoom(r.startX, r.startY, r.xLength, r.yLength, r, dir);
                break;
            case 3:
                res = makeWestRoom(r.startX, r.startY, r.xLength, r.yLength, r, dir);
                break;
            case 4:
                
                break;
        }*/
        //makeWestRoom(r.startX, r.startY, r.xLength, r.yLength, r, dir);
		rooms.Add (r);
		Debug.Log(showDungeon());
        return res;
	}

    public void placeRoom(Room r) // Starting room, no particular direction
    {
        // makeRoom(r.startX, r.startY, r.xLength, r.yLength, 0, r, -1);
        makeCenterRoom(r.startX, r.startY, r.xLength, r.yLength,0,  r, -1);
        rooms.Add (r);
        Debug.Log(showDungeon());
    }

    private bool makeFitRoom(int xlength, int ylength, Room r, int incDir, Vector2 ent, ArrayList doors, int numDoors)
    {
        //x and y should be made so it is outside of the door (basically inside the new room)
        /*
        int xPos = 0;
        int xNeg = 0;
        int yPos = 0;
        int yNeg = 0;
        bool xpFound = false;
        bool xnFound = false;
        bool ypFound = false;
        bool ynFound = false;
        */
        Debug.Log("xL = " + xlength + ", yL = " + ylength);

        // NEW TEST
        MAP_REF[,] room = new MAP_REF[xlength, ylength];

        #region north
        //Dir = north
        if (incDir == 2)
        {
            Debug.Log("Making North");
            //room[xlength / 2, 0] = MAP_REF.FLOOR;
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    if (RefMap[(int)ent.x - (xlength / 2) + w, (int)ent.y + l] == MAP_REF.UNUSED)
                    {
                        
                        if (w == 0 || w == xlength - 1 || l == 0 || l == ylength - 1)
                        {
                            if (w == 0 && l == 0)
                                r.cor3 = new Vector2((int)ent.x - (xlength / 2) + w, (int)ent.y);
                            if (w == 0 && l == ylength - 1)
                                r.cor1 = new Vector2((int)ent.x - (xlength / 2) + w, (int)ent.y + l);
                            if (w == xlength - 1 && l == 0)
                                r.cor4 = new Vector2((int)ent.x + (xlength / 2), (int)ent.y);
                            if (w == xlength - 1 && l == ylength - 1)
                                r.cor2 = new Vector2((int)ent.x + (xlength / 2), (int)ent.y + l);
                            //set wall
                            room[w, l] = MAP_REF.WALL;
                        }
                        else
                        {//set floor
                            room[w, l] = MAP_REF.FLOOR;
                        }
                    }
                    else
                    {
                        room[w, l] = RefMap[(int)ent.x - (xlength / 2) + w, (int)ent.y + l];
                    }
                }
            }
            int doorsMade = 0;
            for(int d = 0; d < doors.Count; d++)
            {
                //Debug.Log("Array Content" + doors[d]);
                switch ((int)doors[d])
                {
                    case 0:
                        Debug.Log("Making door N");
                        int[] coord = new int[] { Random.Range(1, xlength - 1), ylength - 1 };
                        if(room[coord[0],coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 1:
                        Debug.Log("Making door W");
                        coord = new int[] { 0, Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 2:
                        Debug.Log("Making door E");
                        coord = new int[] { xlength - 1, Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                }
                if(doorsMade == numDoors)
                {
                    break;
                }
            }
            

            Debug.Log("entrance v: " + ent.x + "," + ent.y + ". cor1="+r.cor1+", c2:"+r.cor2);
            // LINK room to refMap
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    setCell((int)ent.x - (xlength / 2) + w, (int)ent.y + l, room[w, l]);
                    // RefMap[x - (xlength / 2) + w, y + l] = room[w, l];
                }
            }
        }
        #endregion
        //East
        if (incDir == 1)
        {
            Debug.Log("Making East");
            room[xlength-1, ylength/2] = MAP_REF.FLOOR;
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    if (RefMap[(int)ent.x + w, (int)ent.y-ylength/2 + l] == MAP_REF.UNUSED)
                    {
                        
                        if (w == 0 || w == xlength - 1 || l == 0 || l == ylength - 1)
                        {
                            if (w == 0 && l == 0)
                                r.cor1 = new Vector2((int)ent.x + w, (int)ent.y + ylength / 2 );
                            if (w == 0 && l == ylength - 1)
                                r.cor3 = new Vector2((int)ent.x + w, (int)ent.y - ylength / 2);
                            if (w == xlength - 1 && l == 0)
                                r.cor2 = new Vector2((int)ent.x + w, (int)ent.y + ylength / 2 );
                            if (w == xlength - 1 && l == ylength - 1)
                                r.cor4 = new Vector2((int)ent.x + w, (int)ent.y - ylength / 2);
                            //set wall
                            room[w, l] = MAP_REF.WALL;
                        }
                        else
                        {//set floor
                            room[w, l] = MAP_REF.FLOOR;
                        }
                    }
                    else
                    {
                        room[w, l] = RefMap[(int)ent.x + w, (int)ent.y-ylength/2 + l];
                    }
                }
            }
            int doorsMade = 0;
            for (int d = 0; d < doors.Count; d++)
            {
                switch ((int)doors[d])
                {
                    case 0:
                        Debug.Log("Making door E");
                        int[] coord = new int[] { xlength - 1, (int)Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 1:
                        Debug.Log("Making door N");
                        coord = new int[] { (int)Random.Range(1, xlength - 1), ylength - 1 };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 2:
                        Debug.Log("Making door S");
                        coord = new int[] { (int)Random.Range(1, xlength - 1), 0 };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                }
                if (doorsMade == numDoors)
                {
                    break;
                }
            }
                // LINK room to refMap
                for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    setCell((int)ent.x + w, (int)ent.y -ylength/2+ l, room[w, l]);
                    // RefMap[x - (xlength / 2) + w, y + l] = room[w, l];
                }
            }
        }
        //SOUTH
        if (incDir == 0)
        {
            Debug.Log("Making South");
            room[xlength / 2, 0] = MAP_REF.FLOOR;
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    if (RefMap[(int)ent.x - (xlength / 2) + w, (int)ent.y - l] == MAP_REF.UNUSED)
                    {
                        
                        if (w == 0 || w == xlength - 1 || l == 0 || l == ylength - 1)
                        {
                            if (w == 0 && l == 0)
                                r.cor3 = new Vector2((int)ent.x - (xlength / 2), (int)ent.y-ylength+1);
                            if (w == 0 && l == ylength - 1)
                                r.cor1 = new Vector2((int)ent.x - (xlength / 2), (int)ent.y);
                            if (w == xlength - 1 && l == 0)
                                r.cor4 = new Vector2((int)ent.x + (xlength / 2), (int)ent.y-ylength+1);
                            if (w == xlength - 1 && l == ylength - 1)
                                r.cor2 = new Vector2((int)ent.x + (xlength / 2), (int)ent.y);
                            //set wall
                            room[w, l] = MAP_REF.WALL;
                        }
                        else
                        {//set floor
                            room[w, l] = MAP_REF.FLOOR;
                        }
                    }
                    else
                    {
                        room[w, l] = RefMap[(int)ent.x - (xlength / 2) + w, (int)ent.y - l];
                    }
                }
            }
            int doorsMade = 0;
            for (int d = 0; d < doors.Count; d++)
            {
                switch ((int)doors[d])
                {
                    case 0:
                        Debug.Log("Making door S");
                        int[] coord = new int[] { (int)Random.Range(1, xlength - 1), ylength - 1 };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 1:
                        Debug.Log("Making door E");
                        coord = new int[] { xlength - 1, (int)Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 2:
                        Debug.Log("Making door W");
                        coord = new int[] { 0, (int)Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                        room[coord[0], coord[1]] = MAP_REF.DOOR;
                        doorsMade++;
                        }
                        break;
                }
                if(doorsMade == numDoors)
                {
                    break;
                }
            }
            // LINK room to refMap
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    setCell((int)ent.x - (xlength / 2) + w, (int)ent.y - l, room[w, l]);
                    // RefMap[x - (xlength / 2) + w, y + l] = room[w, l];
                }
            }
        }
        //West
        if (incDir == 3)
        {
            Debug.Log("Making West");
            room[0, ylength/2] = MAP_REF.FLOOR;
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    if (RefMap[(int)ent.x - w, (int)ent.y-ylength/2 + l] == MAP_REF.UNUSED)
                    {
                        if (w == 0 && l == 0)
                            r.cor1 = new Vector2((int)ent.x - xlength+1, (int)ent.y + ylength / 2);
                        if (w == 0 && l == ylength - 1)
                            r.cor3 = new Vector2((int)ent.x - xlength+1, (int)ent.y - ylength / 2);
                        if (w == xlength - 1 && l == 0)
                            r.cor2 = new Vector2((int)ent.x, (int)ent.y + ylength / 2);
                        if (w == xlength - 1 && l == ylength - 1)
                            r.cor4 = new Vector2((int)ent.x, (int)ent.y - ylength / 2 );
                        if (w == 0 || w == xlength - 1 || l == 0 || l == ylength - 1)
                        {
                            //set wall
                            room[w, l] = MAP_REF.WALL;
                        }
                        else
                        {//set floor
                            room[w, l] = MAP_REF.FLOOR;
                        }
                    }
                    else
                    {
                        room[w, l] = RefMap[(int)ent.x - w, (int)ent.y -ylength/2 + l];
                    }
                }
            }
            int doorsMade = 0;
            for (int d = 0; d < doors.Count; d++)
            {
                //Debug.Log("Array Content" + doors[d]);
                switch ((int)doors[d])
                {
                    case 0:
                        Debug.Log("Making door W");
                        int[] coord = new int[] { xlength-1, Random.Range(1, ylength - 1) };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 1:
                        Debug.Log("Making door S");
                        coord = new int[] { (int)Random.Range(1, xlength - 1), 0 };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                    case 2:
                        Debug.Log("Making door N");
                        coord = new int[] { (int)Random.Range(1, xlength - 1), ylength - 1 };
                        if (room[coord[0], coord[1]] == MAP_REF.WALL)
                        {
                            room[coord[0], coord[1]] = MAP_REF.DOOR;
                            doorsMade++;
                        }
                        break;
                }
                if(doorsMade == numDoors)
                {
                    break;
                }
            }
            // LINK room to refMap
            for (int w = 0; w < xlength; w++)
            {
                for (int l = 0; l < ylength; l++)
                {
                    setCell((int)ent.x - w, (int)ent.y -ylength/2+ l, room[w, l]);
                    // RefMap[x - (xlength / 2) + w, y + l] = room[w, l];
                }
            }
        }
        #region oldDoors
        // SET DOORS!
        //makeRandomDoorInRoom(r, (int)r.cor1.x, (int)r.cor1.y, 0);
        //makeRandomDoorInRoom(r, (int)r.cor2.x, (int)r.cor2.y, 0);
        //makeRandomDoorInRoom(r, (int)r.cor3.x, (int)r.cor3.y, 0);
        //makeRandomDoorInRoom(r, (int)r.cor4.x, (int)r.cor4.y, 0);
        /*for (int a = 0; a < numDoors; a++)
        {
            switch ((int)doors[a])
            {
                case 0:
                    //north door - (c1x+c2) /2, c1.y
                    makeRandomDoorInRoom(r,(int)r.cor1.x+(xlength/2),  (int)r.cor1.y, 0 );//(int)(r.cor3.x + r.cor4.x) / 2, (int)r.cor3.y, 0);
                    break;
                case 1:
                    //east door
                    makeRandomDoorInRoom(r, (int)r.cor2.x, (int)r.cor2.y-(ylength/2), 0);
                    break;
                case 2:
                    //south door
                    makeRandomDoorInRoom(r, (int)r.cor3.x + (xlength / 2), (int)r.cor3.y, 0);
                    break;
                case 3:
                    //west door
                    makeRandomDoorInRoom(r, (int)r.cor3.x, (int)r.cor3.y + (ylength / 2), 0);
                    break;
            }
        }*/

        // new test for splitting room in accurate sizes:
        /*
            for(int i = 0; i < xlength/2; i++)
            {
                if(getCell(x+i, y) == MAP_REF.UNUSED && !xpFound && x+i != 0 && x+i != xsize)
                {
                    xPos++;
                }
                else
                {
                    xpFound = true;
                }
                if (getCell(x - i, y) == MAP_REF.UNUSED && !xnFound && x - i != 0 && x - i != xsize)
                {
                    xNeg++;
                }
                else
                {
                    xnFound = true;
                }
            }
            for (int j = 0; j < ylength/2; j++)
            {

                if (getCell(x, y - j) == MAP_REF.UNUSED && !ynFound && y - j != 0 && y - j != ysize)
                {
                    yNeg++;
                }
                else
                {
                    ynFound = true;
                }
                if (getCell(x, y + j) == MAP_REF.UNUSED && !ypFound && y + j != 0 && y + j != ysize)
                {
                    yPos++;
                }
                else
                {
                    ypFound = true;
                }
            }
            // X values
            if(!(xNeg+xPos == xlength))
            {
                while(xNeg<xlength && getCell(xNeg, y)==MAP_REF.UNUSED && xNeg + xPos < xlength)
                {
                    if(getCell(xNeg+1, y) == MAP_REF.UNUSED)
                    {
                        xNeg++;
                    }
                    else { break; }
                }
                while (xPos < xlength && getCell(xPos, y) == MAP_REF.UNUSED && xNeg + xPos < xlength)
                {
                    if (getCell(xPos + 1, y) == MAP_REF.UNUSED)
                    {
                        xPos++;
                    }
                    else { break; }
                }
            }
            // Y values
            if (!(yNeg + yPos == ylength))
            {
                while (yNeg < ylength && getCell(x, yNeg) == MAP_REF.UNUSED && yNeg + yPos < ylength)
                {
                    if (getCell(x, yNeg+1) == MAP_REF.UNUSED)
                    {
                        yNeg++;
                    }
                    else { break; }
                }
                while (yPos < ylength && getCell(x, yPos) == MAP_REF.UNUSED && yNeg + yPos < ylength)
                {
                    if (getCell(x, yPos+1) == MAP_REF.UNUSED)
                    {
                        yPos++;
                    }
                    else { break; }
                }
            }*/

        /////// End ///////

        /*   for (int i = 0; i < xlength; i++)
           {
               //Debug.Log("x = " + i + ", y = " + y);
               //Debug.Log(getCell(x + i, y));
               if (getCell(x+i, y) == MAP_REF.UNUSED && !xpFound && x+i != 0 && x+i != xsize)
               {
                   xPos++;
               }
               else
               {
                   xpFound = true;
               }
               if (getCell(x-i, y) == MAP_REF.UNUSED && !xnFound && x - i != 0 && x- i != xsize)
               {
                   xNeg++;
               }
               else
               {
                   xnFound = true;
               }
           }
           for (int j = 0; j < ylength; j++)
           {

               if (getCell(x, y-j) == MAP_REF.UNUSED && !ynFound && y-j != 0 && y-j != ysize)
               {
                   yNeg++;
               }
               else
               {
                   ynFound = true;
               }
               if (getCell(x, y+j) == MAP_REF.UNUSED && !ypFound && y + j != 0 && y + j != ysize)
               {
                   yPos++;
               }
               else
               {
                   ypFound = true;
               }
           }*/
        //Debug.Log("xPos = " + xPos + ", xNeg = " + xNeg + ", yPos = " + yPos + ", yNeg = " + yNeg);
        //MAKE SURE THE TOTAL LENGTH IS NOT BIGGER THAT MAX!
        /* if (xPos == xNeg)
         {
             xPos /= 2;
             xNeg /= 2;
         }
         if(yPos == yNeg)
         {
             yPos /= 2;
             yNeg /= 2;
         }*/
        /* for(int w = x-xNeg; w < x+xPos; w++)
         {
             for(int l = y-yNeg; l < y+yPos; l++)
             {
                 if(w==x-xNeg || w == x + xPos-1 || l == y - yNeg || l == y + yPos-1)//lower and higher end
                 {
                     if (getCell(w, l) != MAP_REF.WALL || getCell(w, l) == MAP_REF.UNUSED)
                     {
                         if(getCell(w, l) != MAP_REF.DOOR)
                         {
                             setCell(w, l, MAP_REF.WALL);
                         }
                     }
                 }else
                 {
                     setCell(w, l, MAP_REF.FLOOR);
                 }
             }
         }*/

        //Debug.Log(doorPlace + " = doorplace");
        /*
        int xVal = 0;
        int yVal = 0;
        if (xPos > xNeg)
        {
            xVal = xPos;
        }
        else
        {
            xVal = xNeg;
        }
        if (yPos > yNeg)
        {
            yVal = yPos;
        }
        else
        {
            yVal = yNeg;
        }
        */
        /*
        x+xVal = Right
        y+yVal = Up
        x-xneg = left
        x
        */
        /*ArrayList doors = new ArrayList();
        //int nod = Random.Range(1, 4);
        for(int a = 0; a < numberOfDoors; a++)
        {
            Debug.Log("Adding " + numberOfDoors + "doors to list");
            bool placed = false;
            while (!placed)
            {
                int d = Random.Range(0, 3);
                if (!doors.Contains(d))
                {
                    doors.Add(d);
                    placed = true;
                }
            }
        }*/
        //Debug.Log("Done adding doors to list");
        /* while (numDoors > 0)
         {
             int doorPlace = (int)doors[0];//Random.Range(0, 3);
             switch (incDir)
             {
                 case 0://NorthFacing room
                        //North, east and west
                     if (doorPlace == 0)//North
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y - yVal, 0);
                     }
                     else if (doorPlace == 1)//east
                     {
                         makeRandomDoorInRoom(r, x + xPos - 1, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     else
                     {
                         makeRandomDoorInRoom(r, x - xNeg, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     break;
                 case 1://Eastfacing
                        //East, north and south
                     if (doorPlace == 0)
                     {
                         makeRandomDoorInRoom(r, x + xPos - 1, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     else if (doorPlace == 1)
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y - yVal, 0);
                     }
                     else
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y + yVal - 1, 0);
                     }
                     break;
                 case 2://southfacing
                        //south, east and west
                     if (doorPlace == 0)
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y + yVal - 1, 0);
                     }
                     else if (doorPlace == 1)
                     {
                         makeRandomDoorInRoom(r, x + xPos - 1, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     else
                     {
                         makeRandomDoorInRoom(r, x - xNeg, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     break;
                 case 3://westfacing
                        //west, south and north
                     if (doorPlace == 0)
                     {
                         makeRandomDoorInRoom(r, x - xNeg, ((y - yNeg) + (y + yPos - 1)) / 2, 0);
                     }
                     else if (doorPlace == 1)
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y + yVal - 1, 0);
                     }
                     else
                     {
                         makeRandomDoorInRoom(r, ((x - xNeg) + (x + xPos - 1)) / 2, y - yVal, 0);
                     }
                     break;
             }
             doors.RemoveAt(0);
             numDoors--;
         }*/
        #endregion
        setCell(r.startX, r.startY, MAP_REF.FLOOR);
        setCell((int)ent.x, (int)ent.y, MAP_REF.FLOOR);
        /*setCell((int)ent.x, (int)ent.y+1, MAP_REF.FLOOR);
        setCell((int)ent.x, (int)ent.y-1, MAP_REF.FLOOR);
        setCell((int)ent.x+1, (int)ent.y, MAP_REF.FLOOR);
        setCell((int)ent.x-1, (int)ent.y, MAP_REF.FLOOR);*/


        return true;
    }

    private bool makeNorthRoom(int x, int y, int xlength, int ylength, Room r, int doorDir)
    {
        int xlen = xlength;
        int ylen = ylength;
        int doorX = x;
        int doorY = y;
        bool doorEntrance = false;
        int tmpChange = 0;
        int doorWall = Random.Range(0,3);

        Debug.Log("North Facing room");
        // Check if there's enough space left for it
        if (doorDir != -1)
            tmpChange += 1; // means we are going from a set point already occupied.
        for (int ytemp = doorY; ytemp > (doorY - ylen + tmpChange); ytemp--) // added +1 to account for the placement of the joint walls in the south.
        {
            if (ytemp < 0 || ytemp > ysize)
                return false;
            for (int xtemp = (doorX - xlen / 2); xtemp < (doorX + (xlen + 1) / 2); xtemp++)
            {
                if (xtemp < 0 || xtemp > xsize)
                    return false;
                if (getCell(xtemp, ytemp) != MAP_REF.UNUSED && getCell(xtemp, ytemp) != MAP_REF.WALL)
                    return false; // no space left...
            }
        }
        // we're still here, build
        if (doorDir != -1)
        {// set y+=1 to compensate the offset.  //REMEMBER TO SET THE DOOR BACK AFTERWARDS
            doorY += 1;
            doorEntrance = true;
            Debug.Log("DOOR SHOULD BE MADE dir: " + doorWall);
        }
        for (int ytemp = doorY; ytemp > (doorY - ylen); ytemp--)
        {
            for (int xtemp = (doorX - xlen / 2); xtemp < (doorX + (xlen + 1) / 2); xtemp++)
            {

                // start with the walls
                if (xtemp == (doorX - xlen / 2))//West
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (xtemp == (doorX + (xlen - 1) / 2))//East
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == doorY)//south
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY - ylen + 1))// North
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                // and then fill with the floor
                else
                    setCell(xtemp, ytemp, MAP_REF.FLOOR);

                switch (doorWall)
                {
                    case 0:
                        //North
                        if (xtemp == doorX && ytemp == doorY - ylen + 1)
                        {
                            Debug.Log("Creating N");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 1://east
                        Debug.Log("xtemp: "+xtemp +"xtemp simily: " + (doorX + (xlen) / 2));
                        Debug.Log("ytemp: " + ytemp + "ytemp simily: " + (doorY - ylen / 2));
                        if (xtemp == doorX + (xlen-1) / 2 && ytemp == doorY - ylen / 2)
                        {
                            Debug.Log("Creating E");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    
                    case 2://west
                        if (xtemp == doorX - xlen / 2 && ytemp == doorY - ylen / 2)
                        {
                            Debug.Log("Creating W");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 3:
                        Debug.Log("3Inclusive");
                        break;
                }
            }
        }
        if (doorEntrance)
        {
            setCell(doorX, doorY, MAP_REF.DOOR);
        }

        return true;
    }

    private bool makeEastRoom(int x, int y, int xlength, int ylength, Room r, int doorDir)
    {
        int xlen = xlength;
        int ylen = ylength;
        int doorX = x;
        int doorY = y;
        bool doorEntrance = false;
        int tmpChange = 0;
        int doorWall = Random.Range(0, 3);

        if (doorDir != -1)
            tmpChange += 1; // means we are going from a set point already occupied.

        Debug.Log("East Facing room");
        for (int ytemp = (doorY - ylen / 2); ytemp < (doorY + (ylen + 1) / 2); ytemp++)
        {
            if (ytemp < 0 || ytemp > ysize)
                return false;
            for (int xtemp = doorX; xtemp < (doorX + xlen - tmpChange); xtemp++)
            {
                if (xtemp < 0 || xtemp > xsize)
                    return false;
                if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                    return false;
            }
        }
        if (doorDir != -1)
        {// set y+=1 to compensate the offset.  //REMEMBER TO SET THE DOOR BACK AFTERWARDS
            doorX -= 1;
            doorEntrance = true;
            Debug.Log("DOOR SHOULD BE MADE dir: "+doorWall);
        }

        for (int ytemp = (doorY - ylen / 2); ytemp < (doorY + (ylen + 1) / 2); ytemp++)
        {
            for (int xtemp = doorX; xtemp < (doorX + xlen); xtemp++)
            {
                if (xtemp == doorX)//west
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (xtemp == (doorX + xlen - 1))//east
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY - ylen / 2))
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY + (ylen - 1) / 2))
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else
                    setCell(xtemp, ytemp, MAP_REF.FLOOR);

                switch (doorWall)
                {
                    case 0:
                        //North
                        if (xtemp == doorX + xlen / 2 && ytemp == (doorY + (ylen + 1) / 2) - 1)
                        {
                            Debug.Log("Creating N");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 1://east
                        if (xtemp == doorX + (xlen - 1) && ytemp == doorY)
                        {
                            Debug.Log("Creating E");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 2://south
                        if (xtemp == doorX + xlen / 2 && ytemp == (doorY - ylen / 2))
                        {
                            Debug.Log("Creating S");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 3:
                        Debug.Log("3Inclusive");
                        break;
                }
            }
        }
        if (doorEntrance)
        {
            setCell(doorX, doorY, MAP_REF.DOOR);
        }
        return true;
    }

    private bool makeSouthRoom(int x, int y, int xlength, int ylength, Room r, int doorDir)
    {
        int xlen = xlength;
        int ylen = ylength;
        int doorX = x;
        int doorY = y;
        bool doorEntrance = false;
        int tmpChange = 0;
        int doorWall = Random.Range(0, 3);

        if (doorDir != -1)
            tmpChange += 1; // means we are going from a set point already occupied.
        Debug.Log("South Facing room");
        for (int ytemp = y; ytemp < (y + ylen - tmpChange); ytemp++)
        {
            if (ytemp < 0 || ytemp > ysize)
                return false;
            for (int xtemp = (doorX - xlen / 2); xtemp < (doorX + (xlen + 1) / 2); xtemp++)
            {
                if (xtemp < 0 || xtemp > xsize)
                    return false;
                if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                    return false;
            }
        }

        if (doorDir != -1)
        {// set y+=1 to compensate the offset.  //REMEMBER TO SET THE DOOR BACK AFTERWARDS
            doorY -= 1;
            doorEntrance = true;
            Debug.Log("DOOR SHOULD BE MADE dir: "+ doorWall);
        }

        for (int ytemp = doorY; ytemp < (doorY + ylen); ytemp++)
        {
            for (int xtemp = (doorX - xlen / 2); xtemp < (doorX + (xlen + 1) / 2); xtemp++)
            {
                if (xtemp == (doorX - xlen / 2))
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (xtemp == (doorX + (xlen - 1) / 2))
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == doorY)
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY + ylen - 1))
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else
                    setCell(xtemp, ytemp, MAP_REF.FLOOR);

                switch (doorWall)
                {
                   
                    case 0://east
                        if (xtemp == doorX + (xlen-1) / 2 && ytemp == doorY + ylen / 2)
                        {
                            Debug.Log("Creating E");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 1://south
                        if (xtemp == doorX && ytemp == (doorY + ylen - 1))
                        {
                            Debug.Log("Creating S");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 2://west
                        if (xtemp == doorX - xlen / 2 && ytemp == doorY + ylen / 2)
                        {
                            Debug.Log("Creating W");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 3:
                        Debug.Log("3Inclusive");
                        break;
                }
            }
        }
        if (doorEntrance)
        {
            setCell(doorX, doorY, MAP_REF.DOOR);
        }

        return true;
    }

    private bool makeWestRoom(int x, int y, int xlength, int ylength, Room r, int doorDir)
    {
        int xlen = xlength;
        int ylen = ylength;
        int doorX = x;
        int doorY = y;
        bool doorEntrance = false;
        int tmpChange = 0;
        int doorWall = Random.Range(0, 3);

        if (doorDir != -1)
            tmpChange += 1; // means we are going from a set point already occupied.

        Debug.Log("West Facing room");
        for (int ytemp = (doorY - ylen / 2); ytemp < (doorY + (ylen + 1) / 2); ytemp++)
        {
            if (ytemp < 0 || ytemp > ysize)
                return false;
            for (int xtemp = doorX; xtemp > (doorX - xlen+tmpChange); xtemp--)
            {
                if (xtemp < 0 || xtemp > xsize)
                    return false;
                if (getCell(xtemp, ytemp) != MAP_REF.UNUSED)
                    return false;
            }
        }
        if (doorDir != -1)
        {
            doorX += 1;
            doorEntrance = true;
            Debug.Log("DOOR SHOULD BE MADE dir: " + doorWall);
        }

        for (int ytemp = (doorY - ylen / 2); ytemp < (doorY + (ylen + 1) / 2); ytemp++)
        {
            for (int xtemp = doorX; xtemp > (doorX - xlen); xtemp--)
            {
                if (xtemp == doorX)// right side walls
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (xtemp == (doorX - xlen + 1))// left side walls
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY - ylen / 2)) // top
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (doorY + (ylen - 1) / 2))// bottom 
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else
                    setCell(xtemp, ytemp, MAP_REF.FLOOR);

                switch (doorWall)
                {
                    case 0:
                        //North
                        if (xtemp == doorX - xlen / 2 && ytemp == (doorY - ylen / 2))
                        {
                            Debug.Log("Creating N");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 1://south
                        if (xtemp == doorX - xlen / 2 && ytemp == (doorY + (ylen + 1) / 2) - 1)
                        {
                            Debug.Log("Creating S");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 2://west
                        if (xtemp == doorX - xlen + 1 && ytemp == doorY)
                        {
                            Debug.Log("Creating W");
                            makeRandomDoorInRoom(r, xtemp, ytemp, 0);
                        }
                        break;
                    case 3:
                        Debug.Log("3Inclusive");
                        break;
                }
            }
        }
        if (doorEntrance)
        {
            setCell(doorX, doorY, MAP_REF.DOOR);
        }
        return true;
    }

    private bool makeCenterRoom(int x, int y, int xlength, int ylength,
            int direction, Room r, int doorDir)
    {
        /*******************************************************************************/

        // define the dimensions of the room, it should be at least 4x4 tiles
        // (2x2 for walking on, the rest is walls)
        int xlen = xlength;//getRand(4, xlength);
        int ylen = ylength;// getRand(4, ylength);
		//int ddir = doorDir;
        //int doorWall = Random.Range(0, 4);
        // choose the way it's pointing at
        /*
        int dir = 0;
        if (direction > 0 && direction < 4) // not north, north by default, and less than 4
            dir = direction;
        */
        for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
        {
            if (ytemp < 0 || ytemp > ysize)
                return false;
            for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
            {
                if (xtemp < 0 || xtemp > xsize)
                    return false;
                if (getCell(xtemp, ytemp) != MAP_REF.UNUSED && getCell(xtemp, ytemp) != MAP_REF.WALL)
                    return false; // no space left...
            }
        }
        for (int ytemp = (y - ylen / 2); ytemp < (y + (ylen + 1) / 2); ytemp++)
        {
            for (int xtemp = (x - xlen / 2); xtemp < (x + (xlen + 1) / 2); xtemp++)
            {
                if (xtemp == (x - xlen / 2))//West
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (xtemp == (x + (xlen ) / 2))//East  /// Shouldnt it be +1?
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (y-ylen/2))//south
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else if (ytemp == (y + (ylen)/2))// North
                    setCell(xtemp, ytemp, MAP_REF.WALL);
                else
                    setCell(xtemp, ytemp, MAP_REF.FLOOR);
            }
        }
        makeRandomDoorInRoom(r, x, y + (ylen) / 2, 0);
        makeRandomDoorInRoom(r, x + xlen / 2, y, 0);
        makeRandomDoorInRoom(r, x, y - (ylen) / 2, 0);
        makeRandomDoorInRoom(r, x - xlen / 2, y, 0);
        /*switch (doorWall)
        {
            case 0://north
                Debug.Log("Creating N-C");
                makeRandomDoorInRoom(r, x, y + (ylen) / 2, 0);
                break;
            case 1://east
                    Debug.Log("Creating E-C");
                makeRandomDoorInRoom(r, x+xlen/2, y, 0);
                break;
            case 2://south
                    Debug.Log("Creating S-C");
                makeRandomDoorInRoom(r, x, y - (ylen) / 2, 0);
                break;
            case 3://west
                    Debug.Log("Creating W-C");
                makeRandomDoorInRoom(r, x - xlen / 2, y, 0);
                
                break;
            }*/
    return true;

    
    }

    public void makeRandomDoorInRoom(Room r, int xc, int yc, int direction)
    {
        Debug.Log("Making door");
        /*int dir = Random.Range(0, 3);
        Debug.Log("dir = " + dir);
        int height = r.yLength;
        int width = r.xLength;
        int x = r.startX;
        int y = r.startY;
        Debug.Log("Chords = " + x + "," + y + ". Item = " + getCell(x, y));
        int posX = Random.Range((x - 1), (x - width + 1));
        int posY = Random.Range((y - 1), (y - height + 1));*/

        Door d = new Door(new Vector2(xc, yc));
        d.setRoom(r);
        doors.Add(d);
        setCell(new Vector2(xc, yc), MAP_REF.DOOR);
       /* switch (dir)
        {
            case 0://NORTH
                d = new Door(new Vector2(x+1, y));
                d.setRoom(r);
                doors.Add(d);
                setCell(x+1, y, MAP_REF.DOOR);
                break;
            case 1://EAST
                d = new Door(new Vector2(x+width, y+1));
                d.setRoom(r);
                doors.Add(d);
                setCell(x+width, y+1, MAP_REF.DOOR);
                break;
            case 2://SOUTH
                Debug.Log(getCell(x, y + height));
                d = new Door(new Vector2(x+1, y+height));
                d.setRoom(r);
                doors.Add(d);
                setCell(x+1, y+height, MAP_REF.DOOR);
                break;
            case 3://WEST
                d = new Door(new Vector2(x, y+1));
                d.setRoom(r);
                doors.Add(d);
                setCell(x, y+1, MAP_REF.DOOR);
                break;
        }*/
    }

    /*
    public void placePiece(Piece p)
    {
        int psx = p.xStart;
        int psy = p.yStart;


    }
    */
    public void setCell(int x, int y, MAP_REF mr)
    {
        if(mr == MAP_REF.DOOR)
        {
            if(x+minLength >= RefMap.GetLength(0) || y+minLength >= RefMap.GetLength(1) ||
                x-minLength <= 0 || y-minLength <= 0)
            {
                mr = MAP_REF.WALL;
            }
        }
        RefMap[x, y] = mr;
    }
    private MAP_REF getCell(int x, int y)
    {
        return RefMap[x, y];
    }
    public void setCell(Vector2 v, MAP_REF ct)
    {
        int x = (int)v.x;
        int y = (int)v.y;
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
    /*
	private bool makeCorridor(int x, int y, int length, int direction)
	{
		/*******************************************************************************/
		// define the dimensions of the corridor (er.. only the width and
		// height..)
        /*
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
    */

}
