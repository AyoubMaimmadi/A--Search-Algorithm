using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

    float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}
}
