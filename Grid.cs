using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Grid : MonoBehaviour
{
	// unwalkable Mask
	public LayerMask unwalkableMask;
	// area of world cordinates the grid covers
	public Vector2 gridWorldSize;
	// how much space each node covers
	public float nodeRadius;
	// 2D array of nodes
	Node[,] grid;

	// to know how many nodes can we fit in the grid
	float nodeDiameter;
	int gridSizeX, gridSizeY;

	public List<Node> pathAstarManhattan;
	public List<Node> pathAstarEuclidian;
	public List<Node> pathDFS;
	public List<Node> pathBFS;
	public List<Node> pathUCS;

	// how many nodes can we fit in the grid based on the node radius
	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid();
	}

	// create the grid 
	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];

		// check if the node is walkable or not (collision check) 
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				// As x increases, we go in increments of node diameter until we reach the edge
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				// collision check for each point 
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				// new node with the walkable value and world position
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}


	// returs list of nodes that are neighbours of the node 
	public List<Node> GetNeighbours(Node node) {
		// list of empty neighbours
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				// if the node is not the node itself
				// because we are searching in a 3x3 grid around the node
				// when x = 0 and y = 0, we are checking the node itself 
				if (x == 0 && y == 0)
					continue;
				// if the node is within the grid
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				// check x and y are within the grid
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					// add the neighbour to the list of neighbours
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		// return the list of neighbours
		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];
	}


	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (pathDFS != null)
		{
			foreach (Node n in pathDFS)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}

		if (pathAstarManhattan != null)
		{
			foreach (Node n in pathAstarManhattan)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}

		if (pathAstarEuclidian != null)
		{
			foreach (Node n in pathAstarEuclidian)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}

		if (pathBFS != null)
		{
			foreach (Node n in pathBFS)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
		if (pathUCS != null)
		{
			foreach (Node n in pathUCS)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}
}