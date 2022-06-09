using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	public Transform seeker, target;
	Grid grid;

	void Awake() {
		grid = GetComponent<Grid> ();
	}

	void Update() {
		FindPath (seeker.position, target.position);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		// List of nodes to be evaluated for the open set
		List<Node> openSet = new List<Node>();
		// List of nodes that have already been evaluated
		HashSet<Node> closedSet = new HashSet<Node>();
		// add the start node to the open set
		openSet.Add(startNode);
		// while the open set is not empty 
		while (openSet.Count > 0) {
			// find the node in the open set with the lowest f cost
			// first element in the list
			Node node = openSet[0];
			// loop through the open set to find the node with the lowest f cost
			for (int i = 1; i < openSet.Count; i ++) {
				// if the f cost of the node is lower than the current node
				// or if the f cost of the node is equal to the current node
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					// if the node has a lower h cost
					if (openSet[i].hCost < node.hCost)
						// set the node to be the current node
						node = openSet[i];
				}
			}
			// remove the node from the open set
			openSet.Remove(node);
			// add the node to the closed set
			closedSet.Add(node);
			// if the node is the target node we are done
			if (node == targetNode) {
				// before we return the path, we need to retrace it back to the start
				RetracePath(startNode,targetNode);
				// return the path
				return;
			}
			// other wise loop through the neighbours of the node
			// there is an 8 way connection between nodes in average
			// but if it is on the edge of the grid it will have less
			foreach (Node neighbour in grid.GetNeighbours(node)) {
				// if the neighbour is not walkable or if it is in the closed set
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					// skip to the next neighbour
					continue;
				}

				// calculate the g cost of the neighbour node
				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				// if the new cost is less than the neighbour's current g cost
				// or if the neighbour is not currently in the open set
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					// calculate the new g cost of the neighbour node
					neighbour.gCost = newCostToNeighbour;
					// calculate the new h cost of the neighbour node
					neighbour.hCost = GetDistance(neighbour, targetNode);
					// set the parent of the neighbour node to be the current node
					neighbour.parent = node;
					// if the neighbour is not in the open set
					if (!openSet.Contains(neighbour))
						// add the neighbour to the open set
						openSet.Add(neighbour);
				}
			}
		}
	}

	// retrace the path from the target node to the start node
	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;

	}

	// returns the distance between two nodes as an integer
	int GetDistance(Node nodeA, Node nodeB) {
		// the distance between two nodes is the square root of the sum 
		// of the squares of the difference in x and y
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		// return the distance between the two nodes
		if (dstX > dstY)
		// return the distance between the two nodes
			return 14*dstY + 10* (dstX-dstY);
		// return the distance between the two nodes
		return 14*dstX + 10 * (dstY-dstX);
	}
} 