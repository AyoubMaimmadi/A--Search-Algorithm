using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Pathfinding : MonoBehaviour
{
	public Transform seeker, target;
	Grid grid;
	int countAstarManhattan = 0;
	int countAstarEuclidian = 0;
	int countUCS = 0;
	int countBFS = 0;
	int countDFS = 0;

	void Awake()
	{
		grid = GetComponent<Grid>();
	}

	// find the path between the seeker and target using various search algorithms
	void Update()
	{
		// count the number of nodes in the path for each algorithm
		countAstarManhattan = 0;
		countAstarEuclidian = 0;
		countUCS = 0;
		countBFS = 0;
		countDFS = 0;

		// using a stop watch to measure the time it takes to find the path
		var watchAstarManhattan = new System.Diagnostics.Stopwatch();
		var watchAstarEuclidian = new System.Diagnostics.Stopwatch();
		var watchUCS = new System.Diagnostics.Stopwatch();
		var watchBFS = new System.Diagnostics.Stopwatch();
		var watchDFS = new System.Diagnostics.Stopwatch();

		// start the stop watch for the A* Manhattan algorithm
		watchAstarManhattan.Start();
		// find the path using the A* Manhattan heuristic
		FindPathAstarManhattan(seeker.position, target.position);
		// stop the stop watch
		watchAstarManhattan.Stop();

		// start the stop watch for the A* Euclidian algorithm
		watchAstarEuclidian.Start();
		// find the path using the A* Euclidian heuristic
		FindPathAstarEuclidian(seeker.position, target.position);
		// stop the stop watch
		watchAstarEuclidian.Stop();

		// start the stop watch for the UCS algorithm
		watchUCS.Start();
		// find the path using the UCS heuristic
		FindPathUCS(seeker.position, target.position);
		// stop the stop watch
		watchUCS.Stop();

		// start the stop watch for the BFS algorithm
		watchBFS.Start();
		// find the path using the BFS heuristic
		FindPathBFS(seeker.position, target.position);
		// stop the stop watch
		watchBFS.Stop();

		// start the stop watch for the DFS algorithm
		watchDFS.Start();
		// find the path using the DFS heuristic
		FindPathDFS(seeker.position, target.position);
		// stop the stop watch
		watchDFS.Stop();

		// output the time taken for each search algorithm in the console
		Debug.Log($"Execution Time A* Euclidian: {watchAstarEuclidian.ElapsedMilliseconds} ms, retracement : {countAstarEuclidian}\nExecution Time A* Manhattan: {watchAstarManhattan.ElapsedMilliseconds} ms, retracement : {countAstarManhattan}\nExecution Time UCS: {watchUCS.ElapsedMilliseconds} ms, retracement : {countUCS}\nExecution Time BFS: {watchBFS.ElapsedMilliseconds} ms, retracement : {countBFS}\nExecution Time DFS: {watchDFS.ElapsedMilliseconds} ms, retracement : {countDFS}");
	}


	// find the path using the A* Manhattan algorithm 
	void FindPathAstarManhattan(Vector3 startPos, Vector3 targetPos)
	{
		// set the start node to the node at the start position and 
		// set the target node to the node at the target position
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		// create the open and closed lists for the A* Manhattan algorithm
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		// add the start node to the open list
		openSet.Add(startNode);

		// while the open list is not empty
		while (openSet.Count > 0)
		{
			// find the node in the open set with the lowest f cost
			// first element in the list
			Node node = openSet[0];
			// loop through the open set to find the node with the lowest f cost
			for (int i = 1; i < openSet.Count; i++)
			{
				// if the f cost of the node is lower than the current node
				// or if the f cost of the node is equal to the current node
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					// if the node has a lower h cost
					if (openSet[i].hCost < node.hCost)
						// set the node to be the current node
						node = openSet[i];
				}
			}
			// remove the current node from the open list
			openSet.Remove(node);
			// add the current node to the closed list
			closedSet.Add(node);
			// if the node is the target node we are done
			if (node == targetNode)
			{
				// before we return the path, we need to retrace it back to the start
				RetracePathAstarManhattan(startNode, targetNode);
				// return the path
				return;
			}
			// other wise loop through the neighbours of the node
			// there is an 8 way connection between nodes in average
			// but if it is on the edge of the grid it will have less
			foreach (Node neighbour in grid.GetNeighbours(node))
			{
				// if the neighbour is not walkable or if it is in the closed set
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					// skip to the next neighbour
					continue;
				}
				// calculate the g cost of the neighbour node
				int newCostToNeighbour = node.gCost + GetDistanceManhattan(node, neighbour);
				// if the new cost is less than the neighbour's current g cost
				// or if the neighbour is not currently in the open set
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					// calculate the new g cost of the neighbour node
					neighbour.gCost = newCostToNeighbour;
					// calculate the new h cost of the neighbour node
					neighbour.hCost = GetDistanceManhattan(neighbour, targetNode);
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

	void FindPathAstarEuclidian(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePathAstarEuclidian(startNode, targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistanceEuclidian(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistanceEuclidian(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	void FindPathDFS(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		Stack<Node> StackDFS = new Stack<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		StackDFS.Push(startNode);

		while (StackDFS.Count != 0)
		{
			Node currentNode = StackDFS.Pop();
			if (currentNode == targetNode)
			{
				RetracePathDFS(startNode, targetNode);
				return;
			}
			closedSet.Add(currentNode);
			foreach (Node neighbour in grid.GetNeighbours(currentNode))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}
				if (neighbour.walkable || !StackDFS.Contains(neighbour))
				{
					closedSet.Add(neighbour);
					neighbour.parent = currentNode;
					StackDFS.Push(neighbour);
				}
			}
		}
	}


	void FindPathBFS(Vector3 startPos, Vector3 targetPos)
	{

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		Queue<Node> queueBFS = new Queue<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		queueBFS.Enqueue(startNode);

		while (queueBFS.Count != 0)
		{
			Node currentNode = queueBFS.Dequeue();
			if (currentNode == targetNode)
			{
				RetracePathBFS(startNode, targetNode);
				return;
			}
			closedSet.Add(currentNode);
			foreach (Node neighbour in grid.GetNeighbours(currentNode))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}
				if (neighbour.walkable || !queueBFS.Contains(neighbour))
				{
					closedSet.Add(neighbour);
					neighbour.parent = currentNode;
					queueBFS.Enqueue(neighbour);
				}
			}
		}
	}


	void FindPathUCS(Vector3 startPos, Vector3 targetPos)
	{

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePathUCS(startNode, targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost;
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = 0;
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}


	void RetracePathAstarManhattan(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
			countAstarManhattan++;
		}
		path.Reverse();
		grid.pathAstarManhattan = path;
	}

	void RetracePathAstarEuclidian(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
			countAstarEuclidian++;
		}
		path.Reverse();
		grid.pathAstarEuclidian = path;
	}


	void RetracePathUCS(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
			countUCS++;
		}
		path.Reverse();
		grid.pathUCS = path;
	}


	void RetracePathBFS(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
			countBFS++;
		}
		path.Reverse();
		grid.pathBFS = path;
	}


	void RetracePathDFS(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
			countDFS++;
		}
		path.Reverse();
		grid.pathDFS = path;
	}


	int GetDistanceManhattan(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		return dstX + dstY;
	}

	int GetDistanceEuclidian(Node nodeA, Node nodeB)
	{
		return (int)Mathf.Sqrt((Mathf.Pow(nodeA.gridX - nodeB.gridX, 2) + Mathf.Pow(nodeA.gridY - nodeB.gridY, 2)));
	}
}