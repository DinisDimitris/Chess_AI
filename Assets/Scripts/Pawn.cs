using UnityEngine;

public class Pawn : Piece
{
  private int _x;
  private int _y;
  
  private int _colour;
  
  
  private GameObject _piece;

  
  public Pawn(int x, int y, int color, GameObject piece)
  {
    SetPos(x,y);
    SetColour(color);


    _piece = piece; 

  }

  public override Vector2 GetPos()
  {
    return new Vector2(_x, _y);
  }

  public override void SetPos(int x, int y)
  {
    _x = x;
    _y = y;
  }

  public override void SetColour(int color)
  {
    _colour = color;
  }

  public override int GetColour()
  {
    return _colour;
  }
  
  public override  bool Defeat()
  {
    bool isAlive = GameObject.Find("Pawn");
    if (isAlive)
    {
      return true;
    }

    return false;
  }

  public override Vector2[] Move()
  {
    if (GetColour() == 1)
    {
      
      
      Vector2 cords = GetPos();
      Vector2[] legalMoves = new[]
      {
        new Vector2(cords.x, cords.y + 1)
      };

      return legalMoves;
    }

    else
    {
      Vector2 cords = GetPos();
      
      Vector2[] legalMoves = new[]
      {
        new Vector2(cords.x, cords.y - 1)
      };
      return legalMoves;

    }
    

  }

  public override GameObject GetGameObject()
  {
    return _piece;
  }

  public  override void SetGameObject(GameObject piece)
  {
    _piece = piece;
  }
}
