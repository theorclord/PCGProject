using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {
    public enum InteractType
    {
        Entrance,
        Exit
    }

    public InteractType Type
    {
        get;
        set;
    }

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
