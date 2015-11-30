using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public int DungeonWidth;
    public int DungeonHeight;
    private GameObject[,] gameBoard;

    // Use this for initialization
    void Start () {
        gameBoard = new GameObject[DungeonWidth, DungeonHeight];
        visualizeBoard();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void visualizeBoard()
    {
        string[,] tempLab = { { null, "W", null }, { "W", null, null }, { null, "W", "W" } };
        //Debug.Log(tempLab.GetLength(0) + " " + tempLab.GetLength(1));
        for (int i = 0; i < tempLab.GetLength(0); i++)
        {
            for (int j = 0; j < tempLab.GetLength(1); j++)
            {
                if (tempLab[i, j] == null)
                {
                    //ToDo logic for no piece
                }
                else if (tempLab[i, j] == "W")
                {
                    GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                    piece.transform.position = new Vector3(i, j);
                    gameBoard[i, j] = piece;
                }
            }
        }
    }
}
