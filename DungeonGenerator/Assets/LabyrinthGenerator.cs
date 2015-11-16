using UnityEngine;
using System.Collections;

public class LabyrinthGenerator : MonoBehaviour {
    public GameObject[,] gameBoard;
	// Use this for initialization
	void Start () {
        string[,] tempLab = { { null, "W", null }, { "W", null, null }, { null, "W", "W" } };
        gameBoard = new GameObject[tempLab.GetLength(0),tempLab.GetLength(1)];
        Debug.Log(tempLab.GetLength(0) + " " + tempLab.GetLength(1));
        for (int i = 0; i < tempLab.GetLength(0); i++)
        {
            for (int j = 0; j < tempLab.GetLength(1); j++)
            {
                if(tempLab[i,j] == null)
                {
                    Debug.Log("Null hit");
                } else if(tempLab[i,j] == "W")
                {
                    GameObject piece = Instantiate(Resources.Load("Prefabs/GamePiece")) as GameObject;
                    piece.transform.position = new Vector3(i, j);
                    gameBoard[i, j] = piece;
                }
            }
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
