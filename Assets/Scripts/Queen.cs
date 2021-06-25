using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
 private int _x;
  private int _y;
  
  private int _colour;
  
  
  private GameObject _piece;

  
  public Queen(int x, int y, int color, GameObject piece)
  {
    SetPos(x,y);
    SetColour(color);


    _piece = piece; 

  }

  public Vector2 GetPos()
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

  

  public override GameObject GetGameObject()
  {
    return _piece;
  }

  public  override void SetGameObject(GameObject piece)
  {
    _piece = piece;
  }
}
