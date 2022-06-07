using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;
}
