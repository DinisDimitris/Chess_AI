using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
   public static string ToString(Vector2 vec){
       return vec.x + " " + vec.y;
   }

    public static List<Vector2> RemoveIntersectingMoves(List<Vector2> a, List<Vector2> b)
    {
        List<Vector2> t1 = new List<Vector2>(a);
        List<Vector2> t2 = new List<Vector2>(b);
        
        for (int i = 0; i < t1.Count; i++)
        {
            for (int z= t2.Count - 1; z >= 0; z--)
            {

                
                if (t1[i].Equals(t2[z])) {
                    t2.RemoveAt(z);
                }
            }
        }

        return t2;
    }

    public static List<Vector2> GetIntersectingMoves(List<Vector2> t1, List<Vector2> t2)
    {
        var t3 = new List<Vector2>();
        for (int i = 0; i < t1.Count; i++)
        {
            for (int z= 0; z < t2.Count; z++)
            {
                if (t1[i].Equals(t2[z])) t3.Add(t2[z]);
            }
        }

        return t3;
    }


    public static List<Piece> GetAllPiecesByColour(int side, Tile[] boardUpdate)
    {
        List<Piece> pieces = new List<Piece>();
        for(int i =0 ; i < boardUpdate.Length ; i++){
            if(boardUpdate[i].getPiece() != null && boardUpdate[i].getPiece().GetColour() == side)
                pieces.Add(boardUpdate[i].getPiece());
        }

        return pieces;
    }
}
