using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public int DungeonWidth;
    public int DungeonHeight;

    private GameObject[,] gameBoard;
    private Board board;

    // Use this for initialization
    void Start () {
        gameBoard = new GameObject[DungeonWidth, DungeonHeight];
        board = new Board(DungeonWidth,DungeonHeight,1,1);
        Room center = new Room(board.xsize / 2, board.ysize / 2, 5, 5);
        Room cor = new Room(board.xsize / 2 + 3, board.ysize / 2 + 3, 5, 1);
        board.placeRoom(center);
        board.placeRoom(cor);
        
        visualizeBoard(board.RefMap);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

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
                else 
                {
                    GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                    piece.transform.position = new Vector3(i, j);
                    gameBoard[i, j] = piece;
                }
            }
        }
    }
}
