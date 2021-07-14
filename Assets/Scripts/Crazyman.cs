using UnityEngine;

public class Crazyman : Piece
{
     private int _x;
      private int _y;
      
      private int _colour;
      
      
      private GameObject _piece;

      private string _name;
    
      
      public Crazyman(string name,int x, int y, int color, GameObject piece)
      {
        SetPos(x,y);
        SetColour(color);
        SetName(name);
    
        _piece = piece; 
    
      }
      
      public override void SetName(string name)
      {
        _name = name;
      }

      public override string GetName()
      {
        return _name;
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

      public override GameObject GetGameObject()
      {
        return _piece;
      }
    
      public  override void SetGameObject(GameObject piece)
      {
        _piece = piece;
      }
}
