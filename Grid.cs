using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	// unwalkable Mask
	public LayerMask unwalkableMask;
	// area of world cordinates the grid covers
	public Vector2 gridWorldSize;
	// how much space each node covers
	public float nodeRadius;
	// 2D array of nodes
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	// how many nodes can we fit in the grid based on the node radius
	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
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
	
	// method that returns the node at a given world position
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		// get the x and y position of the world position
		// fat right gives 1, fat left gives 0, and 0.5 for the middle
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		// make sure the percent is between 0 and 1
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		// get the x and y position of the node
		// since the array is 0 based, we need to subtract 1 from the position
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		// return the node at the x and y position
		return grid[x,y];
	}

	// list of nodes that make the path
	public List<Node> path;
	// draw the collision map using the gizmos method
	void OnDrawGizmos() {
		// draw the grid
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		// draw the nodes
		if (grid != null) {
			foreach (Node n in grid) {
				// if the node is walkable draw them white otherwise red
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				// if the path is not null, draw the path 
				if (path != null)
					if (path.Contains(n))
						// if the node is in the path, draw it in black
						Gizmos.color = Color.black;
				// draw a cube for each node
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}