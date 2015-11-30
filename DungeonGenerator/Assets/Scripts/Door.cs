using UnityEngine;
using System.Collections;

public class Door {

	Room[] connections = new Room[2]; // a door only connects 2 rooms/corridors
	Vector2 position;
    public enum DIR
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

	public Door (Room r1, Room r2, Vector2 pos)
	{
		connections [0] = r1;
		connections [1] = r2;
		this.position = pos;
	}

	public Door(Vector2 pos)
	{
		this.position = pos;
	}

	public bool setRoom(Room r)
	{
		bool res = false;
		for (int i = 0; i < connections.Length; i++) 
		{
			if(connections[i] == null)
			{
				connections[i] = r;
				res = true;
				break;
			}
		}
		return res;
	}

}
