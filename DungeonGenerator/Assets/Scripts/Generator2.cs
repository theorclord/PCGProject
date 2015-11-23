using System.Collections;
using UnityEngine;

public class Generator2 : MonoBehaviour
{
    Board board;
	public Generator2()
	{
        board = new Board(30,30,1,1); // Size of the board is 30x30, and the tile size is 1x1
        // Create 1 room of size 5x5
        Room startRoom = new Room(board.xsize / 2, board.ysize / 2, 10, 10);
        //Piece startRoom = new Piece(board.xSize/2, board.ySize/2, 5, 5);
        board.placeRoom(startRoom);

        

	}
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
