using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    private GameObject mainCamera;
    private Board.MAP_REF[,] dungeon;
    private GameObject[,] interactable;
    private GameController gameCon;
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
        //Debug.Log(dungeon[(int)transform.position.x, (int)transform.position.y]);
        int transXTrans = (int)(transform.position.x+vec.x);
        int transYTrans = (int)(transform.position.y + vec.y);
        if (!(dungeon.GetLongLength(0) <= transXTrans || 0 > transXTrans || dungeon.GetLongLength(1) <= transYTrans || transYTrans < 0))
        {
            if (dungeon[(int)(transform.position.x + vec.x), (int)(transform.position.y + vec.y)] == Board.MAP_REF.WALL)
            {
                Debug.Log("Wall, can't move through");
            }
            else if(dungeon[(int)(transform.position.x + vec.x), (int)(transform.position.y + vec.y)] == Board.MAP_REF.DOOR)
            {
                transform.Translate(vec);
                gameCon.OpenDoor(transform.position);
            }
            else
            {
                transform.Translate(vec);
            }
        }
        if(interactable[transXTrans,transYTrans] != null){
            GameObject gObj = interactable[transXTrans, transYTrans];
            Interactable inter = gObj.GetComponent<Interactable>();
            gameCon.playerInteraction(inter);
        }
        mainCamera.transform.position = transform.position;
    }

    public void SetGameController(GameController gameController)
    {
        gameCon = gameController;
    }
}
