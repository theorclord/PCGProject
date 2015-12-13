using UnityEngine;
using System.Collections;
using Assets.Scripts.AStar;

public class GameController : MonoBehaviour {
    public int DungeonWidth;
    public int DungeonHeight;
    public GameObject mainCam;
    public GameObject DialogCont;

    // the adventuresness of the player. Should be between 0 and 1
    private float adventureScale = 0.5f;
    private float timeScale = 0.5f;
    private int turnCount = 0;
    private bool exitSet = false;

    //Astar
    private int aStarDist;
    private float adventurenessCalc;
    private int totalMovement;

    private GameObject[,] gameBoard;
    private GameObject[,] interactables;
    private Board board;

    private int MIN_ROOM_SIZE = 3;
    private int MAX_ROOM_SIZE = 8;

    private GameObject player;
    private bool dialogSet;

    // Use this for initialization
    void Start () {
        mainCam.transform.position = new Vector3(DungeonHeight / 2, DungeonWidth / 2,-1);
        gameBoard = new GameObject[DungeonWidth, DungeonHeight];
        interactables = new GameObject[DungeonWidth, DungeonHeight];
        board = new Board(DungeonWidth,DungeonHeight,1,1, MIN_ROOM_SIZE, MAX_ROOM_SIZE);
        Room center = new Room(board.xsize / 2, board.ysize / 2, 5, 5);
        //Room cor = new Room(board.xsize / 2 + 3, board.ysize / 2 + 3, 5, 1);
        board.placeRoom(center);
        //board.placeRoom(cor);

        //player init section
        player = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        PlayerController playCon = player.GetComponent<PlayerController>();
        playCon.SetPlayerCamera(mainCam);
        playCon.SetDungeon(board.RefMap);
        playCon.SetGameController(this);
        playCon.SetInteractable(interactables);
        playCon.TurnMove = 10;
        player.transform.position = new Vector3(DungeonWidth/2, DungeonHeight/2);
        
        //Setting entrance
        GameObject entrance = Instantiate(Resources.Load("Prefabs/Interactable") as GameObject);
        entrance.transform.position = player.transform.position;
        interactables[(int)player.transform.position.x, (int)player.transform.position.y] = entrance;

        visualizeBoard(board.RefMap);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    /// <summary>
    /// Visualizes the board based on the enum map 
    /// </summary>
    /// <param name="boardMap">Enum map repesentation of the board</param>
    private void visualizeBoard(Board.MAP_REF[,] boardMap)
    {
        for (int i = 0; i < boardMap.GetLength(0); i++)
        {
            for (int j = 0; j < boardMap.GetLength(1); j++)
            {
                if (gameBoard[i, j] == null)
                {
                    if (boardMap[i, j] == Board.MAP_REF.UNUSED)
                    {
                        //ToDo logic for no piece
                    }
                    else if (boardMap[i, j] == Board.MAP_REF.WALL)
                    {
                        GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                        piece.transform.position = new Vector3(i, j);
                        gameBoard[i, j] = piece;
                        piece.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Wall", typeof(Sprite)) as Sprite;
                    }
                    else if (boardMap[i, j] == Board.MAP_REF.DOOR)
                    {
                        if (boardMap[i + 1, j] == Board.MAP_REF.WALL)
                        {
                            GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                            piece.transform.position = new Vector3(i, j);
                            gameBoard[i, j] = piece;
                            piece.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DoorNS", typeof(Sprite)) as Sprite;
                        }
                        else
                        {
                            GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                            piece.transform.position = new Vector3(i, j);
                            gameBoard[i, j] = piece;
                            piece.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DoorWE", typeof(Sprite)) as Sprite;
                        }
                    }
                    else
                    {
                        // TODO logic for different pieces
                        GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                        piece.transform.position = new Vector3(i, j);
                        gameBoard[i, j] = piece;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Logic for how to handle dungeon generation when a door is opened
    /// </summary>
    /// <param name="position">Current position of player</param>
    /// <param name="dir">The direction moved, 0 up, 1 right, 2 down, 3 left</param>
    public void OpenDoor(Vector3 position, int dir)
    {
        // When door opens, set the logic of the location to floor
        board.RefMap[(int)(position.x), (int)(position.y)] = Board.MAP_REF.FLOOR;
        // logic for the liniarity of the dungeon based on the adventureScale
        int numdoors = 0;

        float limit1 = 0.45f * adventureScale + 0.35f;
        float limit2 = 0.25f * adventureScale + 0.65f;
        float chanceNumDoors = Random.Range(0f, 1f);
        // Chance for dead end
        // TODO Check for remaining doors
        bool doorFound = false;
        foreach (Board.MAP_REF mr in board.RefMap)
        {
            if(mr == Board.MAP_REF.DOOR)
            {
                doorFound = true;
            }
        }
        float chance = 1 / (1 + Mathf.Exp(-aStarDist * 0.2f * adventureScale*adventurenessCalc + 2));
        if (Random.Range(0f, 1f) <= chance && (doorFound || exitSet))
        {
            numdoors = 0;    
        } else {
            //Number of doors
            if (chanceNumDoors <= limit1)
            { numdoors = 1; }
            else if (chanceNumDoors <= limit2)
            { numdoors = 2; }
            else
            { numdoors = 3; }
        }
        //Prioritiesed order of the doors
        ArrayList doorOrder = getDoorOrder(limit1, limit2);

        Debug.Log("Number of doors: " + numdoors + ", Door order" + doorOrder[0] + doorOrder[1] + doorOrder[2]);

        Debug.Log("Generate New Room");
        int xpos = (int)position.x;
        int ypos = (int)position.y;
        Vector3 tempPos = Vector3.zero;
        for(int i = -1; i < 2; i += 2)
        {
            if (board.RefMap[xpos + i, ypos] == Board.MAP_REF.UNUSED)
            {
                //change pos in x dir
                tempPos = new Vector3(xpos + i, ypos);
                break;
            } else if(board.RefMap[xpos, ypos+i] == Board.MAP_REF.UNUSED)
            {
                //change pos in y dir
                tempPos = new Vector3(xpos, ypos+i);
                break;
            }
        }
        Room newroom = randomRoom(tempPos);
        //Door directions:
        // North = 0
        // East = 1
        // South = 2
        // West = 3
        // TODO: Connected check should add walls to the corners if the next piece is a wall 
        switch (dir)
        {
            case 0:
                //checks if room is connected
                if (board.RefMap[(int)(position.x), (int)(position.y+1)] != Board.MAP_REF.UNUSED)
                {
                    return;
                }
                board.placeRoom(newroom, 2, position, doorOrder, numdoors);
                break;
            case 1:
                //checks if room is connected
                if (board.RefMap[(int)(position.x+1), (int)(position.y)] != Board.MAP_REF.UNUSED)
                {
                    return;
                }
                board.placeRoom(newroom, 1, position, doorOrder, numdoors);
                break;
            case 2:
                //checks if room is connected
                if (board.RefMap[(int)(position.x), (int)(position.y-1)] != Board.MAP_REF.UNUSED)
                {
                    return;
                }
                board.placeRoom(newroom, 0, position, doorOrder, numdoors);
                break;
            case 3:
                //checks if room is connected
                if (board.RefMap[(int)(position.x - 1), (int)(position.y)] != Board.MAP_REF.UNUSED)
                {
                    return;
                }
                board.placeRoom(newroom, 3, position, doorOrder, numdoors);
                break;
        }

        visualizeBoard(board.RefMap);

        // Logic for when the player should find the exit
        //TODO change the impact of the adventure scale
        float chanceOfExit = 1 / (1 + Mathf.Exp(-turnCount * 1/(adventureScale*adventurenessCalc*timeScale+1)));
        //Debug.Log("Exit chance " + chanceOfExit);
        // Sets the exit in the new room
        if (chanceOfExit > 0.999 && !exitSet)
        {
            Debug.Log("Deploy exit");
            int x = newroom.startX;
            int y = newroom.startY;
            Board.MAP_REF mr = Board.MAP_REF.UNUSED;
            while(mr != Board.MAP_REF.WALL && mr != Board.MAP_REF.DOOR)
            {
                switch (dir)
                {
                    case 0:
                        y++;
                        break;
                    case 1:
                        x++;
                        break;
                    case 2:
                        y--;
                        break;
                    case 3:
                        x--;
                        break;
                }           
                mr = board.RefMap[x, y];
            }
            switch (dir)
            {
                case 0:
                    y--;
                    break;
                case 1:
                    x--;
                    break;
                case 2:
                    y++;
                    break;
                case 3:
                    x++;
                    break;
            }
            GameObject exit = Instantiate(Resources.Load("Prefabs/Interactable") as GameObject);
            exit.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Exit", typeof(Sprite)) as Sprite;
            exit.transform.position = new Vector3(x, y);
            exit.GetComponent<Interactable>().Type = Interactable.InteractType.Exit;
            interactables[x, y] = exit;
            exitSet = true;
        }

        // Logic for dialog interaction
        // Set a dialog in the first generated room
        if (!dialogSet)
        {
            Debug.Log("Setting dialog");
            int x = newroom.startX;
            int y = newroom.startY;
            Board.MAP_REF mr = Board.MAP_REF.UNUSED;
            while (mr != Board.MAP_REF.WALL && mr != Board.MAP_REF.DOOR)
            {
                switch (dir)
                {
                    case 0:
                        y++;
                        break;
                    case 1:
                        x++;
                        break;
                    case 2:
                        y--;
                        break;
                    case 3:
                        x--;
                        break;
                }
                mr = board.RefMap[x, y];
            }
            switch (dir)
            {
                case 0:
                    y--;
                    break;
                case 1:
                    x--;
                    break;
                case 2:
                    y++;
                    break;
                case 3:
                    x++;
                    break;
            }
            GameObject dialog = Instantiate(Resources.Load("Prefabs/Interactable") as GameObject);
            dialog.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DialogStarter", typeof(Sprite)) as Sprite;
            dialog.transform.position = new Vector3(x, y);
            dialog.GetComponent<Interactable>().Type = Interactable.InteractType.Dialog;
            interactables[x, y] = dialog;
            dialogSet = true;
        }
    }

    private Room randomRoom(Vector3 position)
    {
        Room r = new Room((int)position.x, (int)position.y, Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE), Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE));
        //Debug.Log("Room.xL = " + r.xLength + ", R.yl = " + r.yLength);
        return r;
    }

    public void playerInteraction(Interactable inter)
    {
        Interactable.InteractType type = inter.Type;
        if(type == Interactable.InteractType.Dialog)
        {
            Debug.Log("Start dialog");
            DialogCont.GetComponent<DialogController>().SetCanvas(true);
        }
        else if(type == Interactable.InteractType.Exit)
        {
            Debug.Log("You win");
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    /// <summary>
    /// Adds the value to the adventurescal
    /// </summary>
    /// <param name="val"></param>
    public void ChangeAdventureScale(float val)
    {
        adventureScale += val;
        adventureScale = Mathf.Clamp(adventureScale, 0.0f, 1.0f);
    }


    /// <summary>
    /// Executes turn based events
    /// </summary>
    public void NextTurn()
    {
        turnCount++;
        totalMovement += 10;
        //Astar test
        //Find entrance
        GameObject entrance = null;
        foreach (GameObject gObj in interactables)
        {
            if(gObj != null) {
                if( gObj.GetComponent<Interactable>().Type == Interactable.InteractType.Entrance)
                {
                    entrance = gObj;
                }
            }
        }
        //Create algorithm board
        bool[,] passable = new bool[board.RefMap.GetLength(0), board.RefMap.GetLength(1)];
        for(int i = 0; i< board.RefMap.GetLength(0); i++)
        {
            for(int j=0;j< board.RefMap.GetLength(1); j++)
            {
                Board.MAP_REF mapPos = board.RefMap[i, j];
                if(mapPos == Board.MAP_REF.UNUSED || mapPos == Board.MAP_REF.WALL)
                {
                    passable[i, j] = false;
                } else
                {
                    passable[i, j] = true;
                }
                
            }
        }

        int disFromStart = AStar.distance(passable, (int)player.transform.position.x, (int)player.transform.position.y,
            (int)entrance.transform.position.x, (int)entrance.transform.position.y);
        aStarDist = disFromStart;
        //Use a combination of AStar and total movement to update the adventureness. 
        // The higher the value the more adventuress
        adventurenessCalc = totalMovement / (aStarDist+1);
        Debug.Log("Astar distance from start " + disFromStart);
    }

    /// <summary>
    /// Function responsible for the control of the door order
    /// </summary>
    /// <param name="limit1"></param>
    /// <param name="limit2"></param>
    /// <returns></returns>
    private ArrayList getDoorOrder(float limit1, float limit2)
    {
        ArrayList doorOrder = null;

        float chanceDoorOrder = Random.Range(0f, 1f);
        float doorOrderTemp = Random.Range(0f, 1f);
        if (chanceDoorOrder <= limit1)
        {
            // Low adventure value
            if (doorOrderTemp <= 0.5)
            {
                doorOrder = new ArrayList(new int[] { 0, 1, 2 });
            }
            else
            {
                doorOrder = new ArrayList(new int[] { 0, 2, 1 });
            }
        }
        else if (chanceDoorOrder <= limit2)
        {
            //Medium adventure value
            if (doorOrderTemp <= 0.5)
            {
                doorOrder = new ArrayList(new int[] { 2, 0, 1 });
            }
            else
            {
                doorOrder = new ArrayList(new int[] { 1, 0, 2 });
            }
        }
        else
        {
            //High adventure value
            if (doorOrderTemp <= 0.5)
            {
                doorOrder = new ArrayList(new int[] { 2, 1, 0 });
            }
            else
            {
                doorOrder = new ArrayList(new int[] { 1, 2, 0 });
            }
        }
        return doorOrder;
    }
}
