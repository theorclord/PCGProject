﻿using System.Collections;
using UnityEngine;

public class Generator2 : MonoBehaviour
{
    Board board;
	public Generator2()
	{
        //board = new Board(30,30,1,1); // Size of the board is 30x30, and the tile size is 1x1
        // Create 1 room of size 5x5
        //Room startRoom = new Room(board.xsize / 2, board.ysize / 2, 5, 5);
        //Piece startRoom = new Piece(board.xSize/2, board.ySize/2, 5, 5);
        //board.placeRoom(startRoom);

        

	}

	/*
		Create a door somewhere in every room

		when a corridor is connected to a room, start at door posision

		Create corridor from Room's door -> when creating, do random for direction and check if possible.

	 */

    void Start()
    {
        board = new Board(30, 30, 1, 1); // Size of the board is 30x30, and the tile size is 1x1
        // Create 1 room of size 5x5
        Room startRoom = new Room(board.xsize / 2, board.ysize / 2, 5, 5);
        //Piece startRoom = new Piece(board.xSize/2, board.ySize/2, 5, 5);
        board.placeRoom(startRoom);
    }
    void Update()
    {

    }


}
