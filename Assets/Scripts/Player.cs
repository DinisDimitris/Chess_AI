
public class Player
{
    private int _pieceColour;

    private int _turns;

    private bool _ischecked;

    private bool _won;

    public Player(int pieceColour)
    {
        _pieceColour = pieceColour;
    }

    public int getColour()
    {
        return _pieceColour;
    }

}
