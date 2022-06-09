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
				RetracePath(startNode,targetNode);
				return;
			}
			// other wise loop through the neighbours of the node
			// there is an 8 way connection between nodes in average
			// but if it is on the edge of the grid it will have less
			foreach (Node neighbour in grid.GetNeighbours(node)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

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

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
} 