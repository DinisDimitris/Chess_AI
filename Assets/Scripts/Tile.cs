using UnityEngine;


public class Tile 
{
	private GameObject _square;
	private int _colour;
	private int _x;
	private int _y;

	private Piece _piece;
	public Tile(int colour, int x, int y, GameObject square)
	{
		setCords(x, y);
		setColour(colour);

		setSquare(square);
	}
	
	public void setColour(int colour){
		_colour = colour;
	}
	
	public void setCords(int x, int y){
		_x = x;
		_y = y;
	}
	
	public int getColour(){
		return _colour;
	}
	
	public Vector2 getCords()
	{
		return new Vector2(_x, _y);
	}

	public void setSquare(GameObject square)
	{
		_square = square;
		
		
	}

	public GameObject getSquare()
	{
		return _square;
	}

	public Piece getPiece()
	{
		return _piece;
	}

	public void setPiece(Piece piece)
	{
		_piece = piece;
		
	}
}
