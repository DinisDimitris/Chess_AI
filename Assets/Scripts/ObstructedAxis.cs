using System;
using UnityEngine;

public class ObstructedAxis
{

    private Vector2 _originPos;

    private Vector2 _newPos;

    public ObstructedAxis(Vector2 originPos, Vector2 newPos)
    {
        _originPos = originPos;

        _newPos = newPos;
    }

    public Vector2 Offset()
    {
        return _newPos - _originPos;
    }

    public Boolean xblocked()
    {
        return Offset().x == 0;
    }

    public Boolean yblocked()
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
