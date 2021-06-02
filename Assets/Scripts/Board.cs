using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static int rows = 8;
    private static int cols = 8;
    
    private char[] xcords = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

    public Color colourX;

    public Color colourY;

    public GameObject square;
    
    private Tile[,] board = new Tile[cols, rows];
    
    // used to change the color in update
    private Tile[] _tiles = new Tile[cols * rows];
    
    public void Start()
    {

        int k = 0;
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                if ( (y% 2 == 0 &  x % 2 == 0) | (y % 2 == 1 & x % 2 == 1) )
                {
                    board[x, y] = new Tile(1, xcords[x], y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourY;

                }

                else
                {
                    board[x, y] = new Tile(0, xcords[x], y, square);
                    
                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourX;
                }

                _tiles[k] = board[x, y];
                k += 1;
                Instantiate(board[x, y].getSquare() , new Vector2(x,y) , Quaternion.identity);
                
                
            }
        }

    }

    public void FixedUpdate()
    {
        for (int i = 0; i < 64; i++)
        {
            if (_tiles[i].getColour() == 1)
            {
                SpriteRenderer spriteRenderer = _tiles[i].getSquare().GetComponent<SpriteRenderer>();
                spriteRenderer.color = colourY;
            }

            else
            {
                SpriteRenderer spriteRenderer = _tiles[i].getSquare().GetComponent<SpriteRenderer>();
                spriteRenderer.color = colourX;
            }
        }
    }
}
