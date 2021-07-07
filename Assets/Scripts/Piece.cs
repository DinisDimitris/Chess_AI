using UnityEngine;

public abstract class Piece
{
    private GameObject _piece;
    public virtual int GetColour()
    {
        return 0;
    }

    public virtual void SetName(string name)
    {
        
    }

    public virtual string GetName()
    {
        return null;
    }

    public virtual void SetPos(int x, int y)
    {
        
    }

    public virtual Vector2 GetPos()
    {
        return new Vector2(0, 0);
    }
    public virtual void SetColour(int colour)
    {
        
    }

    public virtual GameObject GetGameObject()
    {
        return _piece;
    }

    public virtual void  SetGameObject(GameObject piece)
    {
        _piece = piece;
    }
    public virtual Vector2[] Move()
    {
        return new Vector2[0];
    }


    public virtual bool Defeat()
    {
        return false;
    }


}