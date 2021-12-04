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

    private Piece piece;

    private Tile prevTile;

    private List<GameObject> bullets = new List<GameObject>();

    private Player player1;

    private Player player2;

    private King _wking;

    private King _bking;
    private int turn = 1;

    private Vector2 prevMove;

    private Dictionary<Path,Piece> _piecesDefendingKing = new Dictionary<Path,Piece>();
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

                else if (x == 2 && y == 3)
                {
                    _wking = new King("king", x, y, 1, whiteKing);
                    board[x, y].setPiece(_wking);
                }

                else if (x == 5 && y == 5)
                {
                    _bking = new King("king", x, y, 0, blackKing);
                    board[x, y].setPiece(_bking);
                }

                else if (x == 3 && y == 0) board[x, y].setPiece(new Queen("queen", x, y, 1, whiteQueen));

                else if (x == 3 & y == 7) board[x, y].setPiece(new Queen("queen", x, y, 0, blackQueen));

                else if (x == 0 && y == 0 || x == 7 && y == 0) board[x, y].setPiece(new Rook("rook", x, y, 1, whiteRook));

                else if (x == 0 && y == 7 || x == 7 && y == 7) board[x, y].setPiece(new Rook("rook", x, y, 0, blackRook));

                else if (x == 1 && y == 0 || x == 6 && y == 0) board[x, y].setPiece(new Horse("horse", x, y, 1, whiteHorse));

                else if (x == 1 && y == 7 || x == 6 && y == 7) board[x, y].setPiece(new Horse("horse", x, y, 0, blackHorse));

                else if (x == 2 && y == 0 || x == 5 && y == 0) board[x, y].setPiece(new Crazyman("crazyman", x, y, 1, whiteCrazyman));

                else if (x == 2 && y == 7 || x == 5 && y == 7) board[x, y].setPiece(new Crazyman("crazyman", x, y, 0, blackCrazyman));


                board[x, y].setSquare(Instantiate(board[x, y].getSquare(), new Vector2(x, y), Quaternion.identity));
                Piece currentPiece = board[x, y].getPiece();

                if (currentPiece?.GetGameObject())
                {
                    currentPiece.SetGameObject(Instantiate(currentPiece.GetGameObject(), new Vector2(x, y), Quaternion.identity));
                }


            }
        }

        player1 = new Player(1);
        player2 = new Player(2);
        // shrink to 1d array to access during update
        boardUpdate = Optimization.jagg2dArray(board, cols, rows);
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

        if (tile.getColour() == 0) spriteRenderer.color = colourX;

        if (tile.getColour() == 1) spriteRenderer.color = colourY;
    }

    private void DestroyBullets(Vector2 move)
    {
        for (int z = bullets.Count - 1; z >= 0; z--)
        {
            if (bullets[z].transform.position
                .Equals(move))
            {
                Destroy(bullets[z]);
                bullets.RemoveAt(z);
            }
        }
    }

    private void FilterMoves(List<Path> blockedPaths, List<Vector2> moves)
    {
        for (int i = 0; i < blockedPaths.Count; i++)
        {
            Path currentBlocked = blockedPaths[i];

            Vector2 offset = currentBlocked.Offset();

            Vector2 origin = currentBlocked.GetOriginPos();

            Vector2 dest = currentBlocked.GetTargetPos();

            if (currentBlocked.OnX() && !currentBlocked.OnY())
            {
                if (offset.y > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.x.Equals(origin.x) && move.y > dest.y)
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);
                        }
                    }
                }

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

            else if (!currentBlocked.OnX() && currentBlocked.OnY())
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

            else if (!currentBlocked.OnX() && !currentBlocked.OnY())
            {
                if (offset.x.Equals(offset.y) && offset.x > 0 && offset.y > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.x > dest.x && move.y > dest.y)
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

                        if (move.x < dest.x && move.y < dest.y)
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);

                        }
                    }
                }

                else if (offset.x.Equals(Mathf.Abs(offset.y)) && offset.x > 0 && offset.y < 0)
                {

                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.x > dest.x && move.y < dest.y)
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);

                        }
                    }
                }

                else if (offset.y.Equals(Mathf.Abs(offset.x)) && offset.x < 0 && offset.y > 0)
                {
                    for (int k = moves.Count - 1; k >= 0; k--)
                    {
                        Vector2 move = moves[k];

                        if (move.x < dest.x && move.y > dest.y)
                        {
                            DestroyBullets(move);
                            moves.RemoveAt(k);

                        }
                    }
                }
            }
        }
    }
    private List<Vector2> CheckPawnAttack(List<Vector2> moves, Piece attackingPiece)
    {
        if (attackingPiece.GetName().Equals("pawn"))
        {
            var pos = attackingPiece.GetPos();
            if (attackingPiece.GetColour() == 1)
            {
                var illegalMove = new Vector2(pos.x, pos.y + 1);
                if (moves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                {
                    var potentialIllegalMove = new Vector2(illegalMove.x, illegalMove.y + 1);
                    if (moves.Contains(potentialIllegalMove))
                        moves.Remove(potentialIllegalMove);
                    moves.Remove(illegalMove);
                }

                illegalMove.y += 1;
                if (moves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                {

                    moves.Remove(illegalMove);
                }

                if (0 <= pos.x - 1 && pos.x - 1 <= 7 && 0 <= pos.y + 1 &&
                    pos.y + 1 <= 7)
                {
                    Piece xdiagonal = board[(int)pos.x - 1, (int)pos.y + 1]
                        .getPiece();

                    if (xdiagonal != null && !xdiagonal.GetColour().Equals(attackingPiece.GetColour()))
                    {
                        moves.Add(new Vector2(pos.x - 1, pos.y + 1));
                    }

                }
                if (0 <= pos.x + 1 && pos.x + 1 <= 7 && 0 <= pos.y + 1 &&
                    pos.y + 1 <= 7)
                {
                    Piece ydiagonal = board[(int)pos.x + 1, (int)pos.y + 1]
                        .getPiece();

                    if (ydiagonal != null && !ydiagonal.GetColour().Equals(attackingPiece.GetColour()))
                        moves.Add(new Vector2(pos.x + 1, pos.y + 1));
                }
            }
            else
            {
                var illegalMove = new Vector2(pos.x, pos.y - 1);

                if (moves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                {
                    var potentialIllegalMove = new Vector2(illegalMove.x, illegalMove.y - 1);
                    if (moves.Contains(potentialIllegalMove))
                        moves.Remove(potentialIllegalMove);
                    moves.Remove(illegalMove);
                }

                illegalMove.y -= 1;

                if (moves.Contains(illegalMove) && board[(int)illegalMove.x, (int)illegalMove.y].getPiece() != null)
                {
                    moves.Remove(illegalMove);
                }

                if (0 <= pos.x + 1 && pos.x + 1 <= 7 && 0 <= pos.y - 1 &&
                    pos.y - 1 <= 7)
                {
                    Piece xdiagonal = board[(int)pos.x + 1, (int)pos.y - 1]
                        .getPiece();
                    if (xdiagonal != null && !xdiagonal.GetColour().Equals(attackingPiece.GetColour()))
                        moves.Add(new Vector2(pos.x + 1, pos.y - 1));

                }
                if (0 <= pos.x - 1 && pos.x - 1 <= 7 && 0 <= pos.y - 1 &&
                    pos.y - 1 <= 7)
                {
                    Piece ydiagonal = board[(int)pos.x - 1, (int)pos.y - 1]
                        .getPiece();

                    if (ydiagonal != null && !ydiagonal.GetColour().Equals(attackingPiece.GetColour()))
                    {
                        moves.Add(new Vector2(pos.x - 1, pos.y - 1));
                    }
                }

            }
        }

        return moves;
    }

    private List<Vector2> AddPawnDiagonalAttack(int side, List<Vector2> moves, Vector2 pos)
    {

        if (side == 1)
        {
            if (0 <= pos.x - 1 && pos.x - 1 <= 7 && 0 <= pos.y + 1 &&
                pos.y + 1 <= 7)
            {
                moves.Add(new Vector2(pos.x - 1, pos.y + 1));
            }

            if (0 <= pos.x + 1 && pos.x + 1 <= 7 && 0 <= pos.y + 1 &&
                pos.y + 1 <= 7)
            {
                moves.Add(new Vector2(pos.x + 1, pos.y + 1));

            }

        }

        else
        {
            if (0 <= pos.x + 1 && pos.x + 1 <= 7 && 0 <= pos.y - 1 &&
               pos.y - 1 <= 7)
            {
                moves.Add(new Vector2(pos.x + 1, pos.y - 1));
            }

            if (0 <= pos.x - 1 && pos.x - 1 <= 7 && 0 <= pos.y - 1 &&
                pos.y - 1 <= 7)
            {
                moves.Add(new Vector2(pos.x - 1, pos.y - 1));
            }
        }
        return moves;
    }

    private bool IsChecked(Vector2 move)
    {
        Piece attackingPiece = board[(int)move.x, (int)move.y].getPiece();

        Piece king;

        if (attackingPiece.GetColour() == 1)
        {
            king = _bking;
        }
        else
        {
            king = _wking;
        }
        Vector2 origPos = attackingPiece.GetPos();
        List<Vector2> attackingMoves = attackingPiece.Move();

        List<Path> obsMoves = new List<Path>();

        for (int t = 0; t < attackingMoves.Count; t++)
        {
            Vector2 curr = attackingMoves[t];

            if (0 <= curr.x && curr.x <= 7 && 0 <= curr.y && curr.y <= 7)
            {
                if (board[(int)curr.x, (int)curr.y].getPiece() != null)
                {

                    if (!board[(int)curr.x, (int)curr.y].getPiece().GetName().Equals("king"))
                    {
                        obsMoves.Add(new Path(origPos, curr));
                    }
                }
            }

        }


        if (obsMoves.Count > 0)
            FilterMoves(obsMoves, attackingMoves);

        attackingMoves = CheckPawnAttack(attackingMoves, attackingPiece);

        for (int k = 0; k < attackingMoves.Count; k++)
        {
            Vector2 dir = attackingMoves[k];

            if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
            {
                if (king.GetPos().Equals(dir))
                {

                    Debug.Log("King attacked from " + dir + "By " + attackingPiece);
                    return true;
                }
            }
        }


        return false;
    }

    private void InstantiateMoves(List<Vector2> moves, List<GameObject> bullets)
    {
        legalMoves = RemoveIllegalMoves(moves, piece);

        for (int indx = 0; indx < legalMoves.Count; indx++)
        {
            Vector2 dir = legalMoves[indx];
            if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
            {
                bullets.Add(Instantiate(trail, dir, Quaternion.identity));
            }
        }
    }

    private List<Vector2> RemoveIllegalMoves(List<Vector2> moves, Piece pieceToMove)
    {
        List<Path> blockedPaths = new List<Path>();
        for (int t = moves.Count - 1; t >= 0; t--)
        {
            Vector2 dir = moves[t];
            if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
            {
                if (board[(int)dir.x, (int)dir.y].getPiece() != null)
                {
                    if (board[(int)dir.x, (int)dir.y].getPiece().GetColour()
                        .Equals(pieceToMove.GetColour()))
                    {
                        blockedPaths.Add(new Path(pieceToMove.GetPos(), dir));
                        moves.RemoveAt(t);
                    }
                    else
                    {
                        blockedPaths.Add(new Path(pieceToMove.GetPos(), dir));
                    }
                }
            }

        }
        if (blockedPaths.Count > 0)
            FilterMoves(blockedPaths, moves);

        return moves;
    }

    private List<Vector2> RemoveAnyMoveThatWouldResultInCheck(List<Vector2> moves)
    {

        var attackingPieces = new List<Piece>();
        Piece defendingKing;

        if (turn == 1)
        {
            attackingPieces = Utils.GetAllPiecesByColour(0, boardUpdate);
            defendingKing = _wking;
        }
        else
        {
            attackingPieces = Utils.GetAllPiecesByColour(1, boardUpdate);
            defendingKing = _bking;
        }
        List<Vector2> attackingTrajectory = new List<Vector2>();
        bool isKingMoving = false;
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            List<Vector2> attackingMoves = attackingPieces[i].Move();
            attackingMoves = CheckPawnAttack(attackingMoves, attackingPieces[i]);

            //hack
            if (piece.GetName() == "king"){
            attackingMoves = RemoveIllegalMoves(attackingMoves, attackingPieces[i]);
            }

            //remove any pawn moves going forward as they cannot capture
            if (attackingPieces[i].GetName() == "pawn")
            {
                Vector2 origin = attackingPieces[i].GetPos();
                Vector2 pawnForward = new Vector2(0, 0);
                Vector2 pawnForward2 = new Vector2(0, 0);

                attackingMoves = AddPawnDiagonalAttack(attackingPieces[i].GetColour(), attackingMoves, origin);

                if (defendingKing.GetColour() == 0)
                {

                    pawnForward = new Vector2(origin.x, origin.y + 1);
                    pawnForward2 = new Vector2(origin.x, origin.y + 2);
                }
                else if (defendingKing.GetColour() == 1)
                {

                    pawnForward = new Vector2(origin.x, origin.y - 1);
                    pawnForward2 = new Vector2(origin.x, origin.y - 2);
                }

                attackingMoves.Remove(pawnForward);
                attackingMoves.Remove(pawnForward2);
            }
            for (int k = 0; k < attackingMoves.Count; k++)
            {
                if (defendingKing.GetPos().Equals(piece.GetPos()))
                {
                    moves = Utils.RemoveIntersectingMoves(attackingMoves, moves);
                    isKingMoving = true;
                }
                if (attackingMoves.Count > 0)
                {
                    if (attackingMoves[k].Equals(defendingKing.GetPos()) && !isKingMoving)
                    {
                        Path pathToKing = new Path(attackingMoves[k], defendingKing.GetPos());
                        attackingTrajectory = pathToKing.GetAllVectorsOnPath();

                        if ( attackingTrajectory.Count > 0){
                        var legalDefendingMoves = Utils.GetIntersectingMoves(attackingTrajectory,moves);

                        if (legalDefendingMoves.Count > 0);
                        {
                            moves = legalDefendingMoves;
                            _piecesDefendingKing.Add(pathToKing,piece);
                            
                        }
                        }
                    }
                
                }
            }
        }
        return moves;
    }
    private List<Piece> GetPiecesThatBlockAttack(List<Vector2> attackingTrajectory, int side)
    {
        side = side == 1 ? 0 : 1;
        var potentialDefendingPieces = Utils.GetAllPiecesByColour(side, boardUpdate);
        var defendingPieces = new List<Piece>();

        for (int i = 0; i < potentialDefendingPieces.Count; i++)
        {
            var legalMoves = potentialDefendingPieces[i].Move();

            legalMoves = RemoveIllegalMoves(legalMoves, piece);

            legalMoves = CheckPawnAttack(legalMoves, potentialDefendingPieces[i]);

            var blockingMoves = Utils.GetIntersectingMoves(legalMoves, attackingTrajectory);
            if (blockingMoves.Count > 0)
            {
                if (potentialDefendingPieces[i].GetName() != "king")
                {
                    defendingPieces.Add(potentialDefendingPieces[i]);

                }
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
                            legalMoves = new List<Vector2>();
                            piece = boardUpdate[i].getPiece();

                            if (piece != null && piece.GetColour() == turn)
                            {
                                originPos = piece.GetPos();
                                if (!check)
                                {
                                    legalMoves = piece.Move();

                                    legalMoves = CheckPawnAttack(legalMoves, piece);

                                    legalMoves = RemoveAnyMoveThatWouldResultInCheck(legalMoves);
                                }
                                else
                                {
                                    var prevPiece = board[(int)prevMove.x, (int)prevMove.y].getPiece();
                                    var attackingMoves = prevPiece.Move();
                                    if (piece.GetName().Equals("king"))
                                    {
                                        legalMoves = Utils.RemoveIntersectingMoves(attackingMoves, piece.Move());
                                        legalMoves = RemoveAnyMoveThatWouldResultInCheck(legalMoves);
                                    }

                                    else if (!prevPiece.GetName().Equals("horse"))
                                    {
                                        var piecesBlockingAttack = new List<Piece>();

                                        var defendingKing = new GameObject();

                                        var lastTurn = prevPiece.GetColour();
                                        if (lastTurn == 1)
                                        {
                                            defendingKing = GameObject.FindGameObjectWithTag("bking");
                                        }
                                        else
                                        {
                                            defendingKing = GameObject.FindGameObjectWithTag("wking");
                                        }


                                        Path kingToAttackingPiece = new Path(prevPiece.GetPos(), defendingKing.transform.position);
                                        var attackingPath = kingToAttackingPiece.GetAllVectorsOnPath();

                                        piecesBlockingAttack = GetPiecesThatBlockAttack(attackingPath, lastTurn);

                                        if (piecesBlockingAttack.Contains(piece))
                                        {
                                            legalMoves = Utils.GetIntersectingMoves(attackingPath, piece.Move());

                                            legalMoves = CheckPawnAttack(legalMoves, piece);
                                        }

                                    }
                                }
                                if (legalMoves.Count > 0)
                                {
                                    SetColour(hitColor, boardUpdate[i]);
                                    prevTile = boardUpdate[i];

                                    InstantiateMoves(legalMoves, bullets);
                                    one_click = true;
                                }

                                else
                                {
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
                                    if (dir.Equals(bullets[cnt].transform.position))
                                    {
                                        Destroy(bullets[cnt]);
                                        bullets.RemoveAt(cnt);
                                    }
                                }
                                //TODO index this on 1d array
                                if (hit.collider.gameObject.transform.position.Equals(dir) && board[(int)dir.x, (int)dir.y].getPiece() == null)
                                {
                                    prevTile.setPiece(null);
                                    turn = turn == 1 ? 0 : 1;
                                    // border check
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int)dir.x, (int)dir.y);


                                        prevMove = dir;

                                        //TODO index this on 1d array
                                        board[(int)dir.x, (int)dir.y].setPiece(piece);
                                    }
                                }

                                else if (hit.collider.gameObject.transform.position.Equals(dir) &&
                                          !board[(int)dir.x, (int)dir.y].getPiece().GetColour().Equals(piece.GetColour()))
                                {
                                    ResetColour(board[(int)dir.x, (int)dir.y]);
                                    prevTile.setPiece(null);
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                    {
                                        Destroy(board[(int)dir.x, (int)dir.y].getPiece().GetGameObject());

                                        piece.GetGameObject().transform.position = dir;

                                        piece.SetPos((int)dir.x, (int)dir.y);

                                        //TODO index this on 1d array
                                        board[(int)dir.x, (int)dir.y].setPiece(piece);
                                        prevMove = dir;

                                        turn = turn == 1 ? 0 : 1;
                                    }
                                }
                                one_click = false;
                                ResetColour(prevTile);
                            }
                            check = IsChecked(prevMove);
                        }
                    }

                }
            }
        }
    }
}