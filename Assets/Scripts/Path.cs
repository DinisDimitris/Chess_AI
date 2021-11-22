using System;
using UnityEngine;

public class Path
{

    private Vector2 _originPos;

    private Vector2 _newPos;

    public Path(Vector2 originPos, Vector2 newPos)
    {
        _originPos = originPos;

        _newPos = newPos;
    }

    public Vector2 Offset()
    {
        return _newPos - _originPos;
    }

    public bool OnX()
    {
        return Offset().x == 0;
    }

    public bool OnY()
    {
        return Offset().y == 0;
    }

    public Vector2 getOrigin()
    {
        return _originPos;
    }

    public Vector2 getNewPos()
    {
        return _newPos;
    }
}
