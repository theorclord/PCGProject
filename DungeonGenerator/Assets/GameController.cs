using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public int DungeonWidth;
    public int DungeonHeight;
    public GameObject mainCam;

    private float adventuresScale = 0.05f;
    private int turnCount = 0;
    private bool exitNotSet = true;

    private GameObject[,] gameBoard;
    private GameObject[,] interactables;
    private Board board;

    public int MIN_ROOM_SIZE = 4;
    public int MAX_ROOM_SIZE = 7;

    private GameObject player;
    // Use this for initialization
    void Start () {
        mainCam.transform.position = new Vector3(DungeonHeight / 2, DungeonWidth / 2,-1);
        gameBoard = new GameObject[DungeonWidth, DungeonHeight];
        interactables = new GameObject[DungeonWidth, DungeonHeight];
        board = new Board(DungeonWidth,DungeonHeight,1,1);
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
        string[,] tempLab = { { null, "W", null }, { "W", null, null }, { null, "W", "W" } };
        //Debug.Log(tempLab.GetLength(0) + " " + tempLab.GetLength(1));
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

    public void OpenDoor(Vector3 position)
    {
        Debug.Log("Generate New Room");
        int xpos = (int)position.x;
        int ypos = (int)position.y;
        Vector3 tempPos = Vector3.zero;
		int xprev = 0;
		int yprev = 0;
        for(int i = -1; i < 2; i += 2)
        {
            if (board.RefMap[xpos + i, ypos] == Board.MAP_REF.UNUSED)
            {
                //change pos in x dir
                tempPos = new Vector3(xpos + i, ypos);
				xprev = i;
                break;
            } else if(board.RefMap[xpos, ypos+i] == Board.MAP_REF.UNUSED)
            {
                //change pos in y dir
                tempPos = new Vector3(xpos, ypos+i);
				yprev = i;
                break;
            }
        }
        bool prev = false;
        Room newroom = randomRoom(tempPos);
        int attempts = 0;
        while(prev == false && attempts < 10)
        {
            if (yprev == -1)
            {//North
                prev = board.placeRoom(newroom, 0);
            }
            else if (xprev == 1)
            {//East
                prev = board.placeRoom(newroom, 1);

            }
            else if (yprev == 1)
            {//south
                prev = board.placeRoom(newroom, 2);

            }
                if (xprev == -1)
            {//west
                prev = board.placeRoom(newroom, 3);
                
            }
            attempts++;
        }

        //board.placeRoom(randomRoom(tempPos));
        //board.setCell((int)tempPos.x,(int)tempPos.y,Board.MAP_REF.DOOR);
        visualizeBoard(board.RefMap);

        // Logic for when the player should find the exit
        float chanceOfExit = 1 / (1 + Mathf.Exp(-turnCount * adventuresScale));
        Debug.Log(chanceOfExit);
        if (chanceOfExit > 0.999 && exitNotSet)
        {
            Debug.Log("Deploy exit");
            int x = newroom.startX;
            int y = newroom.startY;
            
            GameObject exit = Instantiate(Resources.Load("Prefabs/Interactable") as GameObject);
            exit.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Exit", typeof(Sprite)) as Sprite;
            exit.transform.position = new Vector3(x, y);
            exit.GetComponent<Interactable>().Type = Interactable.InteractType.Exit;
            interactables[x, y] = exit;
            exitNotSet = false;
        }

    }

    private Room randomRoom(Vector3 position)
    {
        return new Room((int)position.x, (int)position.y, Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE), Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE));
    }

    public void playerInteraction(Interactable inter)
    {
        Interactable.InteractType type = inter.Type;
        if(type == Interactable.InteractType.Dialog)
        {
            Debug.Log("Start dialog");
        }
        else if(type == Interactable.InteractType.Exit)
        {
            Debug.Log("You win");
        }
    }

    /// <summary>
    /// Executes turn based events
    /// </summary>
    public void NextTurn()
    {
        turnCount++;
    }
}
