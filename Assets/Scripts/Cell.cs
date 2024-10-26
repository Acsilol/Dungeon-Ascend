using UnityEngine;

public class Cell
{
    public bool visited = false;
    public bool[] status = new bool[4]; // Up, Down, Right, Left
    public bool isMainPath = false;     // Mark if part of the main path
}
