using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour {
    public int DungeonWidth;
    public int DungeonHeight;
    public GameObject mainCam;

    private GameObject[,] gameBoard;
    private Board board;

    private GameObject player;
    // Use this for initialization
    void Start () {
        mainCam.transform.position = new Vector3(DungeonHeight / 2, DungeonWidth / 2,-1);
        gameBoard = new GameObject[DungeonWidth, DungeonHeight];
        board = new Board(DungeonWidth,DungeonHeight,1,1);
        Room center = new Room(board.xsize / 2, board.ysize / 2, 5, 5);
        Room cor = new Room(board.xsize / 2 + 3, board.ysize / 2 + 3, 5, 1);
        board.placeRoom(center);
        board.placeRoom(cor);

        //player init section
        player = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        player.GetComponent<PlayerController>().SetPlayerCamera(mainCam);
        player.GetComponent<PlayerController>().SetDungeon(board.RefMap);
        player.GetComponent<PlayerController>().SetGameController(this);

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
                if (boardMap[i, j] == Board.MAP_REF.UNUSED)
                {
                    //ToDo logic for no piece
                }
                else if (boardMap[i, j] == Board.MAP_REF.WALL)
                {
                    GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                    piece.transform.position = new Vector3(i, j);
                    gameBoard[i, j] = piece;
                    piece.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Wall", typeof (Sprite)) as Sprite;
                }
                else if (boardMap[i, j] == Board.MAP_REF.DOOR)
                {
                    if(boardMap[i+1, j] == Board.MAP_REF.WALL)
                    {
                        GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                        piece.transform.position = new Vector3(i, j);
                        gameBoard[i, j] = piece;
                        piece.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DoorNS", typeof(Sprite)) as Sprite;
                    } else
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

    public void OpenDoor(Vector3 position)
    {
        Debug.Log("Generate New Room");
    }
}
