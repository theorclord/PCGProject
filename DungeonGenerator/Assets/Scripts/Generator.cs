using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {

    /*
    
        Create a room at start in the center.
        Create at least 1 door from that room
        from that door, create corridor to another room.
        
        */
    private int[] dungeon_map = { };
    private const int tileUnused = 0;
    private const int tileCorrH = 1; // not in use
    private const int tileCorrW = 2;
    private const int tileCorrNSE = 3;
    private const int tileCorrNSW = 4;
    private const int tileCorrWNE = 5;
    private const int tileCorrWSE = 6;
    private const int tileCornerNW = 7;
    private const int tileCornerNE = 8;
    private const int tileCornerSE = 9;
    private const int tileCornerSW = 10;
    private const int tileDoor = 11;
    private const int tileBLOCKER = 12;
    private const int tileFloor = 13;
    private const int tileCorridor = 14;
    private const int tileStart = 15;
    private const int tileEnd = 16;

    int numberOfMadeRooms = 0;
    int numOfRooms = 0;
    int numOfDoors = 0;
    int numOfCorrs = 0;

    int xsize;
    int ysize;

    int objects = 0;
    int chanceRoom = 75;
    int chanceCorridor = 25;
    // Maximum room size
    private int room_max_x = 15;
    private int room_max_y = 15;

    // Use this for initialization
    void Start () {
        createDungeon(50, 50, 1);
        System.IO.File.WriteAllText(@"C:\Users\Mats\Desktop\PCG\PCGProject\DungeonGenerator\Assets\TextMaps\WriteText.txt", showDungeon());
      //  print(showDungeon());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void createDungeon(int inx, int iny, int inobj)
    {
        /*******************************************************************************/
        // Here's the one generating the whole map
        if (inobj < 1)
            objects = 10;
        else
            objects = inobj;

        // Adjust the size of the map if it's too small
        if (inx < 3)
           xsize = 3;
        else
           xsize = inx;

        if (iny < 3)
          ysize = 3;
        else
           ysize = iny;

        // redefine the map var, so it's adjusted to our new map size
        dungeon_map = new int [xsize * ysize];

        // start with making the "standard stuff" on the map
        for (int y = 0; y < ysize; y++)
        {
            for (int x = 0; x < xsize; x++)
            {
                // ie, making the borders of unwalkable walls
                if (y == 0)
                    setCell(x, y, tileBLOCKER);
                else if (y == ysize - 1)
                    setCell(x, y, tileBLOCKER);
                else if (x == 0)
                    setCell(x, y, tileBLOCKER);
                else if (x == xsize - 1)
                    setCell(x, y, tileBLOCKER);

                // and fill the rest with dirt
                else
                    setCell(x, y, tileUnused);
            }
        }

        /*******************************************************************************
		 * And now the code of the random-map-generation-algorithm begins!
		 *******************************************************************************/

        // start with making a room in the middle, which we can start building
        // upon
        makeRoom(xsize / 2, ysize / 2, 8, 6, getRand(0, 3)); // startx, starty,
                                                             // lenghtx,
                                                             // lengthy,
                                                             // direction
                                                             // (n,e,s,w)

        // keep count of the number of "objects" we've made
        int currentFeatures = 1; // +1 for the first room we just made

        // then we start the main loop
        for (int countingTries = 0; countingTries < 1000; countingTries++)
        {

            // check if we've reached our quota
            if (currentFeatures == objects)
            {
                break;
            }
            // start with a random wall
            int newX = 0;
            int xmod = 0;
            int newY = 0;
            int ymod = 0;
            int validTile = -1;

            // 1000 chances to find a suitable object (room or corridor)..
            // (yea, i know it's kinda ugly with a for-loop... -_-')

            for (int testing = 0; testing < 1000; testing++)
            {
                newX = getRand(1, xsize - 1);
                newY = getRand(1, ysize - 1);
                validTile = -1;

                // System.out.println("tempx: " + newx + "\ttempy: " + newy);

                if (getCell(newX, newY) == tileBLOCKER
                        || getCell(newX, newY) == tileCorridor)
                {
                    // check if we can reach the place
                    if (getCell(newX, newY + 1) == tileFloor
                            || getCell(newX, newY + 1) == tileCorridor)
                    {
                        validTile = 0; //
                        xmod = 0;
                        ymod = -1;
                    }
                    else if (getCell(newX - 1, newY) == tileFloor
                          || getCell(newX - 1, newY) == tileCorridor)
                    {
                        validTile = 1; //
                        xmod = +1;
                        ymod = 0;
                    }

                    else if (getCell(newX, newY - 1) == tileFloor
                            || getCell(newX, newY - 1) == tileCorridor)
                    {
                        validTile = 2; //
                        xmod = 0;
                        ymod = +1;
                    }

                    else if (getCell(newX + 1, newY) == tileFloor
                            || getCell(newX + 1, newY) == tileCorridor)
                    {
                        validTile = 3; //
                        xmod = -1;
                        ymod = 0;
                    }

                    // check that we haven't got another door nearby, so we
                    // won't get alot of openings besides each other

                    if (validTile > -1)
                    {
                        if (getCell(newX, newY + 1) == tileDoor) // north
                            validTile = -1;
                        else if (getCell(newX - 1, newY) == tileDoor)// east
                            validTile = -1;
                        else if (getCell(newX, newY - 1) == tileDoor)// south
                            validTile = -1;
                        else if (getCell(newX + 1, newY) == tileDoor)// west
                            validTile = -1;
                    }

                    // if we can, jump out of the loop and continue with the
                    // rest
                    if (validTile > -1)
                        break;
                }
            }

            if (validTile > -1)
            {

                // choose what to build now at our newly found place, and at
                // what direction
                int feature = getRand(0, 100);
                if (feature <= chanceRoom)
                { // a new room
                    if (makeRoom((newX + xmod), (newY + ymod), room_max_x,
                            room_max_y, validTile))
                    {
                        currentFeatures++; // add to our quota

                        // then we mark the wall opening with a door
                        setCell(newX, newY, tileDoor);

                        // clean up infront of the door so we can reach it
                        setCell((newX + xmod), (newY + ymod), tileFloor);
                    }
                }

                else if (feature > chanceRoom)
                { // new corridor
                    if (makeCorridor((newX + xmod), (newY + ymod), 6, validTile))
                    {
                        // same thing here, add to the quota and a door
                        currentFeatures++;
                        setCell(newX, newY, tileDoor);
                    }
                }
            }
        }

        /*******************************************************************************
		 * All done with the building, let's finish this one off
		 *******************************************************************************/

        // sprinkle out the bonusstuff (stairs, chests etc.) over the map
        int newx = 0;
        int newy = 0;
        int ways = 0; // from how many directions we can reach the random spot
                      // from
        int state = 0; // the state the loop is in, start with the stairs

        // Find a random place on the map to put the "start stairs" -> '<'
        // and when that is done, find a place for the "end stairs" -> '>'
        // if the place found has 4 possible directions to go, it places the
        // stairs/start/end points.

        while (state != 10)
        {
            for (int testing = 0; testing < 1000; testing++)
            {
                newx = getRand(1, xsize - 1);
                newy = getRand(1, ysize - 2); // cheap bugfix, pulls down newy
                                              // to 0<y<24, from 0<y<25
                                              // System.out.println("x: " + newx + "\ty: " + newy);
                ways = 4; // the lower the better

                // check if we can reach the spot
                if (getCell(newx, newy + 1) == tileFloor
                        || getCell(newx, newy + 1) == tileCorridor)
                {
                    // north
                    if (getCell(newx, newy + 1) != tileDoor)
                        ways--;
                }

                if (getCell(newx - 1, newy) == tileFloor
                        || getCell(newx - 1, newy) == tileCorridor)
                {
                    // east
                    if (getCell(newx - 1, newy) != tileDoor)
                        ways--;
                }

                if (getCell(newx, newy - 1) == tileFloor
                        || getCell(newx, newy - 1) == tileCorridor)
                {
                    // south
                    if (getCell(newx, newy - 1) != tileDoor)
                        ways--;
                }

                if (getCell(newx + 1, newy) == tileFloor
                        || getCell(newx + 1, newy) == tileCorridor)
                {
                    // west
                    if (getCell(newx + 1, newy) != tileDoor)
                        ways--;
                }

                if (state == 0)
                {
                    if (ways == 0)
                    {
                        // we're in state 0, let's place a "upstairs" thing
                        setCell(newx, newy, tileStart);
                        state = 1;
                        break;
                    }
                }

                else if (state == 1)
                {
                    if (ways == 0)
                    {
                        // state 1, place a "downstairs"
                        setCell(newx, newy, tileEnd);
                        state = 10;
                        break;
                    }
                }
            }
        }

        // all done with the map generation, tell the user about it and finish
        numberOfMadeRooms = currentFeatures;
        //System.out.println(msgNumObjects + currentFeatures);

    }

    private bool makeRoom(int x, int y, int xlength, int ylength,
            int direction)
    {
        /*******************************************************************************/

        // define the dimensions of the room, it should be at least 4x4 tiles
        // (2x2 for walking on, the rest is walls)
        int xlen = getRand(4, xlength);
        int ylen = getRand(4, ylength);

        // the tile type it's going to be filled with
        int floor = tileFloor; // jordgolv..
        int wall = tileBLOCKER; // jordv????gg

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
                        if (getCell(xtemp, ytemp) != tileUnused)
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
                        if (getCell(xtemp, ytemp) != tileUnused)
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
                        if (getCell(xtemp, ytemp) != tileUnused)
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
                        if (getCell(xtemp, ytemp) != tileUnused)
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

    private bool makeCorridor(int x, int y, int lenght, int direction)
    {
        /*******************************************************************************/
        // define the dimensions of the corridor (er.. only the width and
        // height..)
        int len = getRand(2, lenght);
        int floor = tileCorridor;
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
                    if (getCell(xtemp, ytemp) != tileUnused)
                        return false;
                }

                // if we're still here, let's start building
                for (ytemp = y; ytemp > (y - len); ytemp--)
                {
                    setCell(xtemp, ytemp, floor);
                }
                break;

            case 1: // east
                ytemp = y;

                for (xtemp = x; xtemp < (x + len); xtemp++)
                {
                    if (xtemp < 0 || xtemp > xsize)
                        return false;
                    if (getCell(xtemp, ytemp) != tileUnused)
                        return false;
                }

                for (xtemp = x; xtemp < (x + len); xtemp++)
                {
                    setCell(xtemp, ytemp, floor);
                }
                break;

            case 2: // south
                xtemp = x;

                for (ytemp = y; ytemp < (y + len); ytemp++)
                {
                    if (ytemp < 0 || ytemp > ysize)
                        return false;
                    if (getCell(xtemp, ytemp) != tileUnused)
                        return false;
                }

                for (ytemp = y; ytemp < (y + len); ytemp++)
                {
                    setCell(xtemp, ytemp, floor);
                }
                break;

            case 3: // west
                ytemp = y;

                for (xtemp = x; xtemp > (x - len); xtemp--)
                {
                    if (xtemp < 0 || xtemp > xsize)
                        return false;
                    if (getCell(xtemp, ytemp) != tileUnused)
                        return false;
                }

                for (xtemp = x; xtemp > (x - len); xtemp--)
                {
                    setCell(xtemp, ytemp, floor);
                }
                break;
        }

        return true;
    }

    // setting a tile's type
    private void setCell(int x, int y, int celltype)
    {
        dungeon_map[x + xsize * y] = celltype;
    }

    // returns the type of a tile
    private int getCell(int x, int y)
    {
        return dungeon_map[x + xsize * y];
    }

    private int getRand(int min, int max)
    {
        int res = Random.Range(min, max);
        return res;
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
                    case tileUnused:
                        dungeonMap += " ";
                        break;
                    case tileBLOCKER:
                        dungeonMap += "@";
                        break;
                    case tileFloor:
                        dungeonMap += ".";
                        break;
                  //  case tileBLOCKER:
                    //    dungeonMap += "\"";
                      //  break;
                    case tileCorridor:
                        dungeonMap += "#";
                        break;
                    case tileDoor:
                        dungeonMap += ".";
                        break;
                    case tileStart:
                        dungeonMap += "S";
                        break;
                    case tileEnd:
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
