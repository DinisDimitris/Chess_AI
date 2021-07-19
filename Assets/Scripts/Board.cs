using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static int rows = 8;
    private static int cols = 8;

    public Color colourX;

    public Color colourY;
    
    public GameObject square;

    public GameObject whitePawn;

    public GameObject blackPawn;

    // prefabs used to set game objects to the board
    public GameObject whiteQueen;

    public GameObject blackQueen;

    public GameObject whiteKing;

    public GameObject blackKing;

    public GameObject whiteRook;

    public GameObject blackRook;

    public GameObject whiteHorse;

    public GameObject blackHorse;

    public GameObject whiteCrazyman;

    public GameObject blackCrazyman;

    public Color hitColor;
    public GameObject trail;


    private Tile[,] board = new Tile[cols, rows];

    private Tile[] boardUpdate = new Tile[cols * rows];

    [SerializeField] private Camera _camera;

    Ray ray;
    RaycastHit hit;

    private bool one_click;

    private List<Vector2> legalMoves;

    private Vector2 originPos;

    private Piece piece;

    private Tile prevTile;

    private List<GameObject> bullets = new List<GameObject>();

    private void Awake()
    {
        if (!_camera) _camera = Camera.main;
    }

    public void Start()
    {
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                if ((y % 2 == 0 && x % 2 == 0) | (y % 2 == 1 && x % 2 == 1))
                {

                    // white tiles
                    board[x, y] = new Tile(1, x, y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourY;
                }
                else
                {

                    // dark tiles
                    board[x, y] = new Tile(0, x, y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourX;
                }

                //cant instantiate game objects in constructors
                if (y == 1) board[x, y].setPiece(new Pawn("pawn", x, y, 1, whitePawn));

                else if (y == 6) board[x, y].setPiece(new Pawn("pawn", x, y, 0, blackPawn));
                
                else if (x == 4 && y == 0) board[x, y].setPiece(new King("king", x, y, 1, whiteKing));

                else if (x == 4 && y == 7) board[x, y].setPiece(new King("king", x, y, 0, blackKing));
                
                else if (x == 3 && y == 0) board[x,y].setPiece(new Queen("queen",x,y,1,whiteQueen));
                
                else if (x == 3 & y == 7 ) board[x,y].setPiece(new Queen("queen",x,y,0,blackQueen));

                else if (x == 0 && y == 0 || x == 7 && y == 0) board[x,y].setPiece(new Rook("rook",x,y,1,whiteRook));
                
                else if (x == 0 && y == 7 || x == 7 && y == 7) board[x,y].setPiece(new Rook("rook",x,y,0,blackRook));
                
                else if ( x == 1 &&  y == 0 || x == 6 && y == 0 ) board[x,y].setPiece(new Horse("horse",x,y,1,whiteHorse));
                
                else if (x == 1 && y == 7 || x == 6 && y == 7) board[x,y].setPiece(new Horse("horse",x,y,0,blackHorse));
                
                else if (x == 2 && y == 0 || x == 5 && y == 0) board[x,y].setPiece(new Crazyman("crazyman",x,y,1,whiteCrazyman));
                
                else if (x == 2 && y ==7 || x == 5 && y == 7) board[x,y].setPiece(new Crazyman("crazyman",x,y,0,blackCrazyman));



                board[x, y].setSquare(Instantiate(board[x, y].getSquare(), new Vector2(x, y), Quaternion.identity));
                Piece currentPiece = board[x, y].getPiece();
                
                if (currentPiece?.GetGameObject() )
                {
                    currentPiece.SetGameObject(Instantiate(currentPiece.GetGameObject(), new Vector2(x,y), Quaternion.identity));
                }


            }
        }
        // shrink to 1d array to access during update
        boardUpdate = jagg2dArray();
    }

    private Tile[] jagg2dArray()
    {

        Tile[] newBoard = new Tile[cols * rows];

        int x = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int k = 0; k < rows; k++)
            {
                newBoard[x] = board[i, k];

                x += 1;
            }
        }

        return newBoard;

    }

    private void DestroyAll(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        for(int i=enemies.Length - 1; i >= 0 ; i--)
        {
            Destroy(enemies[i]);
        }
    }

    private void ResetColour(Tile tile)
    {
        GameObject obj = tile.getSquare();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                                        
        if ( tile.getColour() == 0 ) spriteRenderer.color = colourX;

        if ( tile.getColour() == 1) spriteRenderer.color = colourY;
    }

    private void Update()
    {
        ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            
            for (int i = 0; i < boardUpdate.Length; i++)
            {

                if (hit.collider.gameObject.transform.position.Equals(boardUpdate[i].getSquare().transform.position))
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        
                        if (!one_click)

                        {
                            piece = boardUpdate[i].getPiece();

                            //TODO check for piece type 
                            if (piece != null)
                            {
                                legalMoves = piece.Move();

                                originPos = piece.GetPos();
                                
                                
                                //TODO optimize 
                                GameObject tile = boardUpdate[i].getSquare();
                                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                                spriteRenderer.color = hitColor;

                                prevTile = boardUpdate[i];
                                
                                for (int t = 0; t < legalMoves.Count; t ++)
                                {
                                    Vector2 dir = legalMoves[t];
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {

                                        if (board[(int) dir.x, (int) dir.y].getPiece() == null)
                                        {
                                            bullets.Add(Instantiate(trail, dir, Quaternion.identity));
                                           
                                        }
                                        else
                                        {
                                            Vector2 dist = dir - originPos;
                                            
                                            Debug.Log( "dir" + dir + "originPos" + originPos + "minus will be" + dist );

                                            bool xblocked = dist.x == 0;
                                            bool yblocked = dist.y == 0;
                                                
                                            if (xblocked && !yblocked)
                                            {
                                                for (int j = legalMoves.Count-1; j >=0; j--)
                                                {
                                                    for (int k = 1; k < cols ; k++)
                                                    {
                                                        if (legalMoves[j].Equals(new Vector2(originPos.x,dir.y + k)) && dist.y > 0)
                                                        {
                                                            for (int z = bullets.Count - 1; z >= 0 ; z--)
                                                            {
                                                                Debug.Log(" z" + z + "bullet:" + bullets[z].transform.position);
                                                                if (bullets[z].transform.position
                                                                    .Equals(new Vector2(originPos.x, dir.y + k)))
                                                                {
                                                                    Debug.Log("removed " +bullets[z].transform.position);
                                                                    Destroy(bullets[z]);
                                                                    bullets.RemoveAt(z);
                                                                } 
                                                            }
                                                            // remove the move with the same direction
                                                            legalMoves.RemoveAt(j);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                one_click = true;
                            }
                        }

                        else
                        {
                            foreach (var dir in legalMoves)
                            {

                                for (int cnt = bullets.Count - 1; cnt >= 0; cnt--)
                                {
                                    if ( dir.Equals(bullets[cnt].transform.position) ) 
                                    {
                                        Destroy(bullets[cnt]);
                                        bullets.RemoveAt(cnt);
                                    }
                                }
                                //TODO index this on 1d array
                                if (hit.collider.gameObject.transform.position.Equals(dir) && board[(int) dir.x, (int) dir.y].getPiece() == null )
                                {
                                    one_click = false;
                                    //TODO optimize
                                    ResetColour(prevTile);
                                    prevTile.setPiece(null);
                                    // border check
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int) dir.x, (int) dir.y);

                                        //TODO index this on 1d array
                                        board[(int) dir.x, (int) dir.y].setPiece(piece);
                                    }
                                }

                                else
                                {

                                    one_click = false;
                                    
                                    ResetColour(prevTile);

                                }
                                
                            }
                        }



                    }
                }
            }
        }
    }
}