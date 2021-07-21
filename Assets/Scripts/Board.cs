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

    private List<ObstructedAxis> obstructedMoves = new List<ObstructedAxis>();

    private Piece piece;

    private Tile prevTile;

    private List<GameObject> bullets = new List<GameObject>();

    private Player player1;

    private Player player2;

    private int turn = 1;

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

        player1 = new Player(1);
        player2 = new Player(2);
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

    private void DestroyBullets(Vector2 move)
    {
        for (int z = bullets.Count - 1; z >= 0 ; z--)
        {
            if (bullets[z].transform.position
                .Equals(move))
            {
                Destroy(bullets[z]);
                bullets.RemoveAt(z);
            } 
        }
    }

    // filter the list of legal moves based on obstructed pieces
    // i feel sorry for whoever is about to look at this
    private void FilterMoves(List<ObstructedAxis> obstructedAxisList)
    {
        
        for (int i = 0; i < obstructedAxisList.Count; i++)
        {
            ObstructedAxis currentBlocked = obstructedAxisList[i];

            //direction vector
            Vector2 offset = currentBlocked.Offset();

            Vector2 origin = currentBlocked.getOrigin();

            Vector2 dest = currentBlocked.getNewPos();

            // x axis is the same as our piece , check for moves on the y 
            if (currentBlocked.xblocked() && !currentBlocked.yblocked())
            {
                // positive offset means the obstructed piece is above our current piece
                if (offset.y > 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];
                        
                        if (move.x.Equals(origin.x) && move.y > dest.y)
                        {
                            DestroyBullets(move);
                                // illegal move
                            legalMoves.RemoveAt(k);
                        }

                        
                    }
                }
                
                // negative offset means the obstructed piece is below our piece
                else if (offset.y < 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];

                        if (move.x.Equals(origin.x) && move.y < dest.y)
                        {
                            
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                }
            }
            
            // y is the same, check for moves on the x
            else if (!currentBlocked.xblocked() && currentBlocked.yblocked())
            {
                if (offset.x > 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];

                        if (move.y.Equals(origin.y) && move.x > dest.x)
                        {
                            
                            for (int z = bullets.Count - 1; z >= 0 ; z--)
                            {
                                if (bullets[z].transform.position
                                    .Equals(move))
                                {
                                    Destroy(bullets[z]);
                                    bullets.RemoveAt(z);
                                } 
                            }
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                    
                }
                else if (offset.x < 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];

                        if (move.y.Equals(origin.y) && move.x < dest.x)
                        {
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                    
                }
            }
            
            // diagonal obstruction
            else if (!currentBlocked.xblocked() && !currentBlocked.yblocked())
            {
                if (offset.x.Equals(offset.y) && offset.x > 0 && offset.y > 0 )
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];
                        
                        if ( move.x > dest.x && move.y  > dest.y )
                        {
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                    
                }
                
                else if (offset.x.Equals(offset.y) && offset.x < 0 && offset.y < 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];
                        
                        if ( move.x < dest.x && move.y  < dest.y )
                        {
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                }
                
                else if (offset.x.Equals(Mathf.Abs(offset.y ) ) && offset.x > 0 && offset.y < 0)
                {
                  
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];
                        
                        if ( move.x > dest.x && move.y < dest.y )
                        {
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                }
                
                else if (offset.y.Equals(Mathf.Abs(offset.x ) ) && offset.x < 0 && offset.y > 0)
                {
                    for (int k = legalMoves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = legalMoves[k];
                        
                        if ( move.x < dest.x && move.y > dest.y )
                        {
                            DestroyBullets(move);
                            legalMoves.RemoveAt(k);
                            
                        }
                    }
                }
            }
        }
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
                            
                            if (piece != null && piece.GetColour() == turn)
                            {
                                legalMoves = piece.Move();

                                originPos = piece.GetPos();
                                
                                
                                //TODO optimize 
                                GameObject tile = boardUpdate[i].getSquare();
                                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                                spriteRenderer.color = hitColor;

                                prevTile = boardUpdate[i];
                                obstructedMoves = new List<ObstructedAxis>();

                                if (piece.GetName().Equals("pawn") )
                                {
                                    if (piece.GetColour() == 1)
                                    {
                                        Piece xdiagonal = board[(int) originPos.x - 1, (int) originPos.y + 1]
                                            .getPiece();

                                        Piece ydiagonal = board[(int) originPos.x + 1, (int) originPos.y + 1]
                                            .getPiece();

                                        if (xdiagonal != null && !xdiagonal.GetColour().Equals(piece.GetColour()))
                                            legalMoves.Add(new Vector2(originPos.x - 1, originPos.y + 1));

                                        if (ydiagonal != null && !ydiagonal.GetColour().Equals(piece.GetColour()))
                                            legalMoves.Add(new Vector2(originPos.x + 1, originPos.y + 1));
                                    }
                                    else
                                    {
                                        Piece xdiagonal = board[(int) originPos.x + 1, (int) originPos.y - 1]
                                            .getPiece();

                                        Piece ydiagonal = board[(int) originPos.x - 1, (int) originPos.y - 1]
                                            .getPiece();
                                        
                                        if (xdiagonal != null && !xdiagonal.GetColour().Equals(piece.GetColour()))
                                            legalMoves.Add(new Vector2(originPos.x +  1, originPos.y - 1));

                                        if (ydiagonal != null && !ydiagonal.GetColour().Equals(piece.GetColour()))
                                            legalMoves.Add(new Vector2(originPos.x - 1, originPos.y - 1));
                                    }
                                }
                                for (int t = legalMoves.Count - 1; t >= 0 ; t --)
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
                                            if (board[(int) dir.x, (int) dir.y].getPiece().GetColour()
                                                .Equals(piece.GetColour()))
                                            {
                                                obstructedMoves.Add(new ObstructedAxis(originPos, dir));
                                                legalMoves.RemoveAt(t);
                                            }
                                            else
                                            {
                                                obstructedMoves.Add(new ObstructedAxis(originPos, dir));
                                            }
                                        }
                                    }
                                }
                                
                                if (obstructedMoves.Count > 0) FilterMoves(obstructedMoves);
                                one_click = true;
                            }

                        }

                        else
                        {
                            if (legalMoves.Count < 1)
                            {
                                one_click = false;
                                ResetColour(prevTile);
                            }
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
                                    turn = turn == 1 ? 0 : 1;
                                    // border check
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int) dir.x, (int) dir.y);

                                        //TODO index this on 1d array
                                        board[(int) dir.x, (int) dir.y].setPiece(piece);
                                    }
                                }

                                else if (hit.collider.gameObject.transform.position.Equals(dir) &&
                                          !board[(int) dir.x, (int) dir.y].getPiece().GetColour().Equals(piece.GetColour()))
                                {
                                    one_click = false;
                                    
                                    ResetColour(prevTile);
                                    prevTile.setPiece(null);
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        Destroy(board[(int) dir.x , (int) dir.y ].getPiece().GetGameObject());
                                        
                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int) dir.x, (int) dir.y);

                                        //TODO index this on 1d array
                                        board[(int) dir.x, (int) dir.y].setPiece(piece);
                                        
                                        turn = turn == 1 ? 0 : 1;
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