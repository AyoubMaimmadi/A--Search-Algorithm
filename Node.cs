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

	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}