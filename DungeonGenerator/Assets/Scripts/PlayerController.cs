using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    private GameObject mainCamera;
    private Board.MAP_REF[,] dungeon;
    private GameObject[,] interactable;
    private GameController gameCon;

    public int TurnMove
    {
        get; set;
    }
    private int currentMove;
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.W))
        {
            executeMove(Vector3.up);    
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            executeMove(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            executeMove(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            executeMove(Vector3.right);
        }
    }

    public void SetPlayerCamera(GameObject cam)
    {
        mainCamera = cam;
    }

    /// <summary>
    /// Sets the board of of the game. 
    /// </summary>
    /// <param name="dun"></param>
    public void SetDungeon(Board.MAP_REF[,] dun)
    {
        dungeon = dun;
    }

    public void SetInteractable(GameObject[,] interactable)
    {
        this.interactable = interactable;
    }

    private void executeMove(Vector3 vec)
    {
        int dir = -1;
        Vector3 curPos = transform.position;
        int transXTrans = (int)(transform.position.x + vec.x);
        int transYTrans = (int)(transform.position.y + vec.y);
        if ((int) curPos.y < transYTrans)
        {
            dir = 0;
        } else if((int)curPos.x < transXTrans)
        {
            dir = 1;
        } else if((int) curPos.y > transYTrans)
        {
            dir = 2;
        } else
        {
            dir = 3;
        }

        if (!(dungeon.GetLongLength(0) <= transXTrans || 0 > transXTrans || dungeon.GetLongLength(1) <= transYTrans || transYTrans < 0))
        {
            if (dungeon[(int)(transform.position.x + vec.x), (int)(transform.position.y + vec.y)] == Board.MAP_REF.WALL)
            {
                Debug.Log("Wall, can't move through");
            }
            else if(dungeon[(int)(transform.position.x + vec.x), (int)(transform.position.y + vec.y)] == Board.MAP_REF.DOOR)
            {
                transform.Translate(vec);
                currentMove++;
                gameCon.OpenDoor(transform.position, dir);
            }
            else
            {
                transform.Translate(vec);
                currentMove++;
            }
        }
        if(interactable[transXTrans,transYTrans] != null){
            GameObject gObj = interactable[transXTrans, transYTrans];
            Interactable inter = gObj.GetComponent<Interactable>();
            gameCon.playerInteraction(inter);
        }
        mainCamera.transform.position = transform.position;
        if (currentMove >= TurnMove)
        {
            currentMove = 0;
            gameCon.NextTurn();
        }
    }

    public void SetGameController(GameController gameController)
    {
        gameCon = gameController;
    }
}
