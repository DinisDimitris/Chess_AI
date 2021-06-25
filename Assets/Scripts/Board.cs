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

    public GameObject trail;

    private GameObject[] trails;

    private Tile[,] board = new Tile[cols, rows];

    private Tile[] boardUpdate = new Tile[cols * rows];

    [SerializeField] private Camera _camera;

    Ray ray;
    RaycastHit hit;

    private bool one_click;

    private Vector2[] newPos;

    private Vector2 originPos;

    private Piece piece;

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
                if ((y % 2 == 0 & x % 2 == 0) | (y % 2 == 1 & x % 2 == 1))
                {

                    // whites
                    board[x, y] = new Tile(1, x, y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourY;
                }
                else
                {
                    
                    // darks
                    board[x, y] = new Tile(0, x, y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourX;
                }

                if (y == 1)
                {

                    board[x, y].setPiece(new Pawn(x, y, 1, whitePawn));
                }

                else if (y == 6)
                {
                    board[x, y].setPiece(new Pawn(x, y, 0, blackPawn));
                }


                board[x, y].setSquare(Instantiate(board[x, y].getSquare(), new Vector2(x, y), Quaternion.identity));
                Piece currentPiece = board[x, y].getPiece();

                if (currentPiece != null)
                {
                    if (currentPiece.GetColour().Equals(1))
                        currentPiece.SetGameObject(Instantiate(whitePawn, new Vector2(x, y), Quaternion.identity));


                    else if (currentPiece.GetColour().Equals(0))
                        currentPiece.SetGameObject(Instantiate(blackPawn, new Vector2(x, y), Quaternion.identity));
                }


            }
        }


        // shrink to array to access during update
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

    private void FixedUpdate()
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

                                newPos = piece.Move();

                                originPos = piece.GetPos();
                                
                                int trailSpace = 0;
                                
                                // Allocate fixed space for direction bullets based on change in direction
                                foreach (var dir  in newPos)
                                {
                                    if (!originPos.x.Equals(dir.x)) trailSpace += (int) dir.x;

                                    if (!originPos.y.Equals(dir.y)) trailSpace += (int) dir.y;
                                }
                                trails = new GameObject[trailSpace];


                                foreach (var dir in newPos)
                                {
                                    if (piece.GetColour() == 1)
                                    {

                                    
                                        for (int trailLength = 0; trailLength < dir.y - originPos.y; trailLength++)
                                        {
                                            trails[trailLength] = Instantiate(trail, new Vector2(originPos.x,
                                                    originPos.y + trailLength + 1
                                                ),
                                                Quaternion.identity);

                                        }

                                    }

                                    else
                                    {
                                        for (int trailLength = 0; trailLength < originPos.y - dir.y; trailLength++)
                                        {
                                            trails[trailLength] = Instantiate(trail, new Vector2(originPos.x,
                                                    originPos.y - trailLength - 1
                                                ),
                                                Quaternion.identity);

                                        }
                                    }
                                }



                                one_click = true;
                            }
                        }

                        else
                        {
                            foreach (var dir in newPos)
                            {
                                if (hit.collider.gameObject.transform.position.Equals(dir))
                                {
                                    one_click = false;
                                    for (int j = 0; j < trails.Length; j++) Destroy(trails[j]);
                                    // border check
                                    if (0 <= dir.x && dir.x <= 7 && 0 <= dir.y && dir.y <= 7)
                                        //TODO index this on 1d array
                                        if (board[(int) dir.x, (int) dir.y].getPiece() == null)
                                        {

                                            piece.GetGameObject().transform.position = dir;

                                            piece.SetPos((int) dir.x, (int) dir.y);

                                            boardUpdate[i].setPiece(null);

                                            //TODO index this on 1d array
                                            board[(int) dir.x, (int) dir.y].setPiece(piece);


                                        }
                                }
                            }
                        }



                    }
                }
            }
        }
    }
}

