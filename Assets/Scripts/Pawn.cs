using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
  private int _x;
  private int _y;
  
  private int _colour;

  private string _name;
  
  private GameObject _piece;
  public Pawn(string name, int x, int y, int color, GameObject piece)
  {
    SetPos(x,y);
    SetColour(color);
    SetName(name);
  

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

  public override void SetName(string name)
  {
    _name = name;
  }

  public override string GetName()
  {
    return _name;
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

    int initialWhitePos = 1;

    int initialBlackPos = 6;
    if (GetColour() == 1)
    {
      Vector2 cords = GetPos();
    
      if (GetPos().y.Equals(initialWhitePos))
      {
        Vector2[] legalMoves =
        {
          new Vector2(cords.x, cords.y + 1),
          new Vector2(cords.x, cords.y + 2),
        };

        return legalMoves;
      }

      else
      {
        Vector2[] legalMoves =
        {
          new Vector2(cords.x, cords.y + 1)
        };
        return legalMoves;

      }

    }

    else
    {
      Vector2 cords = GetPos();


      if (GetPos().y.Equals(initialBlackPos))
      {
              
        Vector2[] legalMoves = 
        {
        
          new Vector2(cords.x, cords.y - 1),
           
          new Vector2(cords.x, cords.y - 2)
        };

        return legalMoves;
        
      }

      else
      {

        Vector2[] legalMoves =
        {

          new Vector2(cords.x, cords.y - 1),
        };
        return legalMoves;

      }
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
