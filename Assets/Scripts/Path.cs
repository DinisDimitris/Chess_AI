using System.Collections.Generic;
using UnityEngine;

public class Path
{

    private Vector2 _originPos;

    private Vector2 _targetPos;

    public Path(Vector2 originPos, Vector2 targetPos)
    {
        _originPos = originPos;

        _targetPos = targetPos;
    }

    public List<Vector2> GetAllVectorsOnPath()
    {
        var dir = this.Offset();

        List<Vector2> path = new List<Vector2>();
        if (dir.y == 0 && dir.x > 0){
            for (int x = 0; x < dir.x ; x++){
              path.Add(new Vector2(_originPos.x + x, _originPos.y));
              
            }
        }
        else if (dir.y == 0 && dir.x < 0){
            for (int x = 0; x < Mathf.Abs(dir.x) ; x++){
              path.Add(new Vector2(_originPos.x + (-x), _originPos.y));
            }
        }

        else if (dir.x == 0 && dir.y > 0){
            for (int y = 0; y < dir.y ; y++){
                path.Add(new Vector2(_originPos.x, _originPos.y + y));
            }
        }

        else if (dir.x == 0 && dir.y < 0){
             for (int y = 0; y < Mathf.Abs(dir.y) ; y++){
                path.Add(new Vector2(_originPos.x, _originPos.y + y));
            }
        }

        else if (dir.x == dir.y && dir.x > 0)
        {
            for (int k = 0; k < (dir.x); k++){
                path.Add(new Vector2(_originPos.x + k, _originPos.y + k));
            }
        }

        else if (dir.x == dir.y && dir.x < 0)
        {
            for (int k = 0; k < Mathf.Abs(dir.x); k++){
                path.Add(new Vector2(_originPos.x + -k, _originPos.y + -k));
            }
        }

        else if (dir.x + dir.y == 0 && dir.x > 0){
            for (int k = 0; k < dir.x; k++){
                path.Add(new Vector2(_originPos.x + k, _originPos.y + -k));
            }
        }

        else if (dir.x + dir.y == 0 && dir.x < 0){
            for (int k = 0; k < Mathf.Abs(dir.x); k++){
                path.Add(new Vector2(_originPos.x + -k, _originPos.y + k));
            }
        }

    return path;
    }

    public Vector2 Offset()
    {
        return _targetPos - _originPos;
    }

    public bool OnX()
    {
        return Offset().x == 0;
    }

    public bool OnY()
    {
        return Offset().y == 0;
    }

    public Vector2 GetOriginPos()
    {
        return _originPos;
    }

    public Vector2 GetTargetPos()
    {
        return _targetPos;
    }
}
