using System.Collections.Generic;
using UnityEngine;

public static class DefendKing{
    public static List<Vector2> GetAttackingPath(Vector2 kingPos, Vector2 attackingPos, List<Vector2> attackingMoves, int side)
    {
        Vector2 dir = kingPos - attackingPos;

        List<Vector2> attackingPath = new List<Vector2>();

        Vector2 attackMove = new Vector2(0,0);

        if (dir.y == 0){
        for (int x = 0; x < Mathf.Abs(dir.x) ; x++){
            if(side == 0){
                attackMove = new Vector2(attackingPos.x + (-x), attackingPos.y);
            }
            else{
                attackMove = new Vector2(attackingPos.x + x, attackingPos.y);
            }
            if (attackingMoves.Contains(attackMove)){
                attackingPath.Add(attackMove);
                }
            }

        return attackingPath;
        }

        if (dir.x == 0){
            for (int y = 0; y < Mathf.Abs(dir.y) ; y++){
            if (side == 0){
                attackMove = new Vector2(attackingPos.x, attackingPos.y + (-y));
            }
            else{
             attackMove = new Vector2(attackingPos.x, attackingPos.y + y);
            }
            if (attackingMoves.Contains(attackMove)){
                attackingPath.Add(attackMove);
                }
            }

        return attackingPath;
        }

        if (dir.x == dir.y){
        for (int k = 0; k < Mathf.Abs(dir.x); k++){
            if (side ==0){
                attackMove = new Vector2(attackingPos.x + (-k), attackingPos.y + (-k));
                Debug.Log(attackMove);
            }
                attackMove = new Vector2(attackingPos.x + k , attackingPos.y + k);
            if (attackingMoves.Contains(attackMove)){
                    attackingPath.Add(attackMove);
            }

        }

        return attackingPath;
        }

        if (dir.x + dir.y == 0){
        for (int k = 0; k < Mathf.Abs(dir.x); k++){
            if (side ==0){
                attackMove = new Vector2(attackingPos.x + (-k), attackingPos.y + (-k));
                Debug.Log(attackMove);
            }
                attackMove = new Vector2(attackingPos.x + k , attackingPos.y + k);
            if (attackingMoves.Contains(attackMove)){
                    attackingPath.Add(attackMove);
            }

        }

        return attackingPath;
        }
        return null;
    }
}