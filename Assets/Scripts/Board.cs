using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static int rows = 8;
    private static int cols = 8;

    public Color colourX;

    public Color colourY;
    
    public GameObject square;

    public GameObject blackCrazyman,
        whiteCrazyman,
        blackHorse,
        whiteHorse,
        blackRook,
        whiteRook,
        blackKing,
        whiteKing,
        blackQueen,
        whiteQueen,
        blackPawn,
        whitePawn;

    public Color hitColor;
    
    public GameObject trail;


    private Tile[,] board = new Tile[cols, rows];

    private Tile[] boardUpdate = new Tile[cols * rows];

    [SerializeField] private Camera _camera;

    Ray ray;
    RaycastHit hit;

    private bool one_click;

    private bool check;

    private List<Vector2> legalMoves;

    private Vector2 originPos;

    private List<ObstructedAxis> obstructedMoves = new List<ObstructedAxis>();

    private Piece piece;

    private Tile prevTile;

    private List<GameObject> bullets = new List<GameObject>();

    private Player player1;

    private Player player2;
    

    private int turn = 1;

    private Vector2 prevMove; 
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

    private void SetColour(Color color, Tile tile)
    {
        //TODO optimize 
        GameObject curr_tile = tile.getSquare();
        SpriteRenderer spriteRenderer = curr_tile.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
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
    private void FilterMoves(List<ObstructedAxis> obstructedAxisList, List<Vector2> moves)
    {
        
        for (int i = 0; i < obstructedAxisList.Count; i++)
        {
            ObstructedAxis currentBlocked = obstructedAxisList[i];

            //direction vector
            Vector2 offset = currentBlocked.Offset();
            
            //base position
            Vector2 origin = currentBlocked.getOrigin();

            //target position
            Vector2 dest = currentBlocked.getNewPos();
            
            //Based on target - position , we know which axis is obstructed
            // then simply compare the other component

            // x axis is the same as our piece , check for moves on the y 
            if (currentBlocked.xblocked() && !currentBlocked.yblocked())
            {
                // positive offset means the obstructed piece is above our current piece
                if (offset.y > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];
                        
                        if (move.x.Equals(origin.x) && move.y > dest.y)
                        {
                            DestroyBullets(move);
                                // illegal move
                            moves.RemoveAt(k);
                        }

                        
                    }
                }
                
                // negative offset means the obstructed piece is below our piece
                else if (offset.y < 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.x.Equals(origin.x) && move.y < dest.y)
                        {
                            
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                }
            }
            
            // y is the same, check for moves on the x
            else if (!currentBlocked.xblocked() && currentBlocked.yblocked())
            {
                if (offset.x > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.y.Equals(origin.y) && move.x > dest.x)
                        {
                            
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                    
                }
                else if (offset.x < 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.y.Equals(origin.y) && move.x < dest.x)
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                    
                }
            }
            
            // diagonal obstruction
            else if (!currentBlocked.xblocked() && !currentBlocked.yblocked())
            {
                if (offset.x.Equals(offset.y) && offset.x > 0 && offset.y > 0 )
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];
                        
                        if ( move.x > dest.x && move.y  > dest.y )
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                    
                }
                
                else if (offset.x.Equals(offset.y) && offset.x < 0 && offset.y < 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];
                        
                        if ( move.x < dest.x && move.y  < dest.y )
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                }
                
                else if (offset.x.Equals(Mathf.Abs(offset.y ) ) && offset.x > 0 && offset.y < 0)
                {
                  
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];
                        
                        if ( move.x > dest.x && move.y < dest.y )
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                }
                
                else if (offset.y.Equals(Mathf.Abs(offset.x ) ) && offset.x < 0 && offset.y > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];
                        
                        if ( move.x < dest.x && move.y > dest.y )
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                            
                        }
                    }
                }
            }
        }
    }

    private void CheckPawnAttack()
    {
        if (piece.GetName().Equals("pawn") )
        {
            if (piece.GetColour() == 1)
            {
                    var illegalMove = new Vector2(originPos.x,originPos.y + 1);
                        if (legalMoves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                        {
                            var potentialIllegalMove = new Vector2(illegalMove.x, illegalMove.y + 1);
                            if (legalMoves.Contains(potentialIllegalMove))
                                legalMoves.Remove(potentialIllegalMove);
                            legalMoves.Remove(illegalMove);
                        }
                    
                

                if (0 <= originPos.x - 1  && originPos.x - 1 <= 7 && 0 <= originPos.y + 1 &&
                    originPos.y + 1 <= 7)
                {
                    Piece xdiagonal = board[(int) originPos.x - 1, (int) originPos.y + 1]
                        .getPiece();


                    if (xdiagonal != null && !xdiagonal.GetColour().Equals(piece.GetColour()))
                        legalMoves.Add(new Vector2(originPos.x - 1, originPos.y + 1));
                }

                if (0 <= originPos.x + 1 && originPos.x + 1 <= 7  && 0 <= originPos.y + 1 &&
                    originPos.y + 1 <= 7)
                {
                    Piece ydiagonal = board[(int) originPos.x + 1, (int) originPos.y + 1]
                        .getPiece();

                    if (ydiagonal != null && !ydiagonal.GetColour().Equals(piece.GetColour()))
                        legalMoves.Add(new Vector2(originPos.x + 1, originPos.y + 1));

                }

            }
            else
            {
                var illegalMove = new Vector2(originPos.x,originPos.y - 1);
                if (legalMoves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                {
                    var potentialIllegalMove = new Vector2(illegalMove.x, illegalMove.y - 1);
                    if (legalMoves.Contains(potentialIllegalMove))
                        legalMoves.Remove(potentialIllegalMove);
                    legalMoves.Remove(illegalMove);
                    }
                
                if (0 <= originPos.x + 1 && originPos.x  + 1 <= 7 && 0 <= originPos.y - 1 &&
                    originPos.y - 1 <= 7)
                {
                    Piece xdiagonal = board[(int) originPos.x + 1, (int) originPos.y - 1]
                        .getPiece();

                    if (xdiagonal != null && !xdiagonal.GetColour().Equals(piece.GetColour()))
                        legalMoves.Add(new Vector2(originPos.x + 1, originPos.y - 1));
                }

                if (0 <= originPos.x - 1 && originPos.x - 1 <= 7 && 0 <= originPos.y - 1 &&
                    originPos.y - 1 <= 7)
                {
                    Piece ydiagonal = board[(int) originPos.x - 1, (int) originPos.y - 1]
                        .getPiece();

                    if (ydiagonal != null && !ydiagonal.GetColour().Equals(piece.GetColour()))
                        legalMoves.Add(new Vector2(originPos.x - 1, originPos.y - 1));
                }

            }
        }
    }

    private bool IsChecked(Vector2 move)
    {
        Piece attackingPiece = board[(int) move.x, (int) move.y].getPiece();


        GameObject king = new GameObject();
        
        if (attackingPiece.GetColour() == 1)
        {
            king = GameObject.FindGameObjectWithTag("bking");
        }
        else
        {
            king = GameObject.FindGameObjectWithTag("wking");
        }
        Vector2 origPos = attackingPiece.GetPos();
        List<Vector2> attackingMoves = attackingPiece.Move();

        List<ObstructedAxis> obsMoves = new List<ObstructedAxis>();
        
        for (int t = 0; t < attackingMoves.Count; t++)
        {
                Vector2 curr = attackingMoves[t];

                if (0 <= curr.x && curr.x <= 7 && 0 <= curr.y && curr.y <= 7)
                {
                    if (board[(int) curr.x, (int) curr.y].getPiece() != null)
                    {
                        
                        if (!board[(int) curr.x, (int) curr.y].getPiece().GetName().Equals("king"))
                        {
                            obsMoves.Add(new ObstructedAxis(origPos, curr));
                        }
                    }
                }

        }
            
            
        if (obsMoves.Count > 0) FilterMoves(obsMoves, attackingMoves);
            
        for (int k = 0; k < attackingMoves.Count; k++)
        {
                Vector2 dir = attackingMoves[k];
                
                if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                {
                    if (king.transform.position.Equals(dir) )
                    {
                        
                            return true;
                    }
                }
        }
        

        return false;
    }

    private List<Vector2> GetAttackingPath(Vector2 kingPos, Vector2 attackingPos)
    {
        Vector2 dir = kingPos - attackingPos;

        List<Vector2> attackingPath = new List<Vector2>();


        return null;
    }

    private List<Vector2> RemoveIntersectingMoves(List<Vector2> t1, List<Vector2> t2)
    {
        for (int i = 0; i < t1.Count; i++)
        {
            for (int z= t2.Count - 1; z >= 0; z--)
            {
                if (t1[i].Equals(t2[z])) t2.RemoveAt(z);
            }
        }

        return t2;
    }

    private List<Vector2> GetIntersectingMoves(List<Vector2> t1, List<Vector2> t2)
    {
        var t3 = new List<Vector2>();
        for (int i = 0; i < t1.Count; i++)
        {
            for (int z= t2.Count - 1; z >= 0; z--)
            {
                if (t1[i].Equals(t2[z])) t3.Add(t2[z]);
            }
        }

        return t3;
    }

    private void InstantiateMoves(List<Vector2> moves, List<ObstructedAxis> obs, List<GameObject> bull)
    {
        for (int t = moves.Count - 1; t >= 0 ; t --)
        {
            Vector2 dir = moves[t];
            if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
            {

                if (board[(int) dir.x, (int) dir.y].getPiece() == null)
                {
                    bull.Add(Instantiate(trail, dir, Quaternion.identity));

                }
                else
                {
                    if (board[(int) dir.x, (int) dir.y].getPiece().GetColour()
                        .Equals(piece.GetColour()))
                    {
                        obs.Add(new ObstructedAxis(originPos, dir));
                        moves.RemoveAt(t);
                    }
                    else
                    {
                        obs.Add(new ObstructedAxis(originPos, dir));
                    }
                }
            }
                                    
        }
        
    }

    private List<Piece> GetAllPiecesByColour(int side)
    {
        List<Piece> pieces = new List<Piece>();
        for(int i =0 ; i < boardUpdate.Length; i++){
            if(boardUpdate[i].getPiece() != null && boardUpdate[i].getColour() == side)
                pieces.Add(boardUpdate[i].getPiece());
        }

        return pieces;
    }

    private List<Piece> GetPiecesThatBlockAttack(List<Vector2> attackingTrajectory, int side )
    {
        side = side == 1 ? 0 : 1;
        var potentialDefendingPieces = GetAllPiecesByColour(side);

        var defendingPieces = new List<Piece>();

        for (int i =0; i < potentialDefendingPieces.Count; i++){
            var possibleMoves = potentialDefendingPieces[i].Move();

            var blockingMoves = this.GetIntersectingMoves(possibleMoves, attackingTrajectory);
            if (blockingMoves.Count > 0 ){
                defendingPieces.Add(potentialDefendingPieces[i]);
            }
        }
        return defendingPieces;

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
                                originPos = piece.GetPos();
                                if (!check)
                                {
                                    legalMoves = piece.Move();

                                    SetColour(hitColor, boardUpdate[i]);
                                    CheckPawnAttack();

                                    prevTile = boardUpdate[i];
                                    obstructedMoves = new List<ObstructedAxis>();

                                    InstantiateMoves(legalMoves, obstructedMoves, bullets);

                                    if (obstructedMoves.Count > 0) FilterMoves(obstructedMoves, legalMoves);
                                    one_click = true;
                                }
                                else
                                {
                                    var prevPiece = board[(int) prevMove.x, (int) prevMove.y].getPiece();
                                    var attackingTrajectory = prevPiece.Move();

                                    var piecesBlockingAttack = GetPiecesThatBlockAttack(attackingTrajectory, prevPiece.GetColour());

                                    if (piece.GetName().Equals("king"))
                                    {
                                        legalMoves = RemoveIntersectingMoves(attackingTrajectory, piece.Move());
                                    }
                                    else if (piecesBlockingAttack.Contains(piece)){
                                        
                                        legalMoves =  GetIntersectingMoves(attackingTrajectory, piece.Move());
                                    }
                                
                                    if (legalMoves.Count > 0){
                                    SetColour(hitColor, boardUpdate[i]); 
                                    prevTile = boardUpdate[i];
                                    obstructedMoves = new List<ObstructedAxis>();

                                    InstantiateMoves(legalMoves, obstructedMoves, bullets);

                                    if (obstructedMoves.Count > 0) FilterMoves(obstructedMoves, legalMoves);
                                    one_click = true;
                                    }

                                    else{
                                        
                                    }
                                }

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

                                        
                                        prevMove = dir;
                                        //TODO index this on 1d array
                                        board[(int) dir.x, (int) dir.y].setPiece(piece);
                                    }
                                }

                                else if (hit.collider.gameObject.transform.position.Equals(dir) &&
                                          !board[(int) dir.x, (int) dir.y].getPiece().GetColour().Equals(piece.GetColour()))
                                {
                                    one_click = false;
                                    
                                    ResetColour(board[(int) dir.x, (int) dir.y]);
                                    ResetColour(prevTile);
                                    prevTile.setPiece(null);
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        Destroy(board[(int) dir.x , (int) dir.y ].getPiece().GetGameObject());
                                        
                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int) dir.x, (int) dir.y);

                                        //TODO index this on 1d array
                                        board[(int) dir.x, (int) dir.y].setPiece(piece);
                                        prevMove = dir;

                                        turn = turn == 1 ? 0 : 1;

                                    }
                                }
                                else if (!hit.collider.gameObject.transform.position.Equals(dir))
                                {
                                    one_click = false;
                                    ResetColour(prevTile);

                                }
                                
                            }
                            check = IsChecked(prevMove);
                        }
                    }
                    
                }
            }
        }
    }
}