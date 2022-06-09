using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
	// walkable or not (if not, it's a wall)
	public bool walkable;
	// what point in the world this node represents
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	// G and H cost in order to calculate the F cost
	public int gCost;
	public int hCost;
	public Node parent;
	
	// constructor for the node class 
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	// calculate the F cost of the node
	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}