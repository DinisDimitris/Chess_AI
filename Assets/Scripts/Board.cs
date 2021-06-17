using UnityEngine;

public class Board : MonoBehaviour
{
    private static int rows = 8;
    private static int cols = 8;
    
    private char[] ycords = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

    public Color colourX;

    public Color colourY;

    public GameObject square;

    public GameObject whitePawn;

    public GameObject blackPawn;
    
    private Tile[,] board = new Tile[cols, rows];

    private Tile[] boardUpdate = new Tile[cols * rows];
    
    [SerializeField] private Camera _camera;
    
    Ray ray;
    RaycastHit hit;
    

    private void Awake()
    {
        if(!_camera) _camera = Camera.main;
    }

    public void Start()
    {
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                if ((y % 2 == 0 & x % 2 == 0) | (y % 2 == 1 & x % 2 == 1))
                {

                    board[x, y] = new Tile(1, x , y , square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourY;
                }
                else
                {
                    board[x, y] = new Tile(0, x , y, square);

                    GameObject tile = board[x, y].getSquare();
                    SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = colourX;
                }

                if (y == 1)
                {

                    board[x, y].setPiece(new Pawn(x,y,1,whitePawn));
                }

                else if (y == 6)
                {
                    board[x, y].setPiece(new Pawn(x,y,0,blackPawn));
                }
                
                
                board[x,y].setSquare(Instantiate(board[x, y].getSquare(), new Vector2(x, y), Quaternion.identity));
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
    private void Update()
    {
        ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
                    foreach (Tile tile in boardUpdate)
                    {
                        if (hit.collider.gameObject.transform.position.Equals(tile.getSquare().transform.position))
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                Piece piece = tile.getPiece();
                                if (piece != null)
                                {
                                    Vector2 newPos = piece.Move();

                                    piece.GetGameObject().transform.position = newPos;
                                }
                            }
                        }
                    }
        }
    }
}
