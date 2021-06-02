using UnityEngine;
using System.Collections;

public class Tile 
{
	private GameObject _square;
	private int _colour;
	private char _x;
	private int _y;
	
	public Tile(int colour, char x, int y, GameObject square)
	{
		setCords(x, y);
		setColour(colour);
		setSquare(square);
	}
	public void setColour(int colour){
		_colour = colour;
	}
	
	public void setCords(char x, int y){
		_x = x;
		_y = y;
	}
	
	public int getColour(){
		return _colour;
	}
	
	public (char,int) getCords(){
		return (_x,_y);
	}

	public void setSquare(GameObject square)
	{
		_square = square;
	}

	public GameObject getSquare()
	{
		return _square;
	}
	
}
