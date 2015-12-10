using UnityEngine;
using System.Collections;

public class DialogController : MonoBehaviour {
    public GameObject DialogCanvas;
    public GameObject gameController;

    private GameController gameCon;
    // Use this for initialization
    void Start () {
        // Init dialog
        DialogCanvas.SetActive(false);

        gameCon = gameController.GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
        //For testing Purpose
        if (Input.GetKeyDown(KeyCode.O))
        {
            DialogCanvas.SetActive(!DialogCanvas.activeSelf);
        }
    }

    public void SetCanvas(bool val)
    {
        DialogCanvas.SetActive(val);
    }

    public void ButtonClicked(bool selection)
    {
        if (selection)
        {
            Debug.Log("You clicked yes");
            // Increase the adventure scale
            gameCon.ChangeAdventureScale(0.5f);
            DialogCanvas.SetActive(false);
        } else
        {
            Debug.Log("You clicked no");
            // Decrease the adventure scale
            gameCon.ChangeAdventureScale(-0.5f);
            DialogCanvas.SetActive(false);
        }
    }
}
