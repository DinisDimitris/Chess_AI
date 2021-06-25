using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
 private int _x;
   private int _y;
   
   private int _colour;
   
   
   private GameObject _piece;
 
   
   public King(int x, int y, int color, GameObject piece)
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
 
   public override Vector2[] Move()
   {
  
       Vector2 cords = GetPos();

       Vector2[] LegalMoves = new[]
       {
         new Vector2(cords.x, cords.y + 1),
         new Vector2(cords.x + 1, cords.y + 1),
         new Vector2(cords.x + 1, cords.y),
         new Vector2(cords.x + 1, cords.y - 1),
         new Vector2(cords.x, cords.y - 1),
         new Vector2(cords.x - 1, cords.y - 1),
         new Vector2(cords.x - 1, cords.y),
         new Vector2(cords.x - 1, cords.y + 1)
       };

      
       
       return LegalMoves;

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

