using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class MovePositionVisualizer : MonoBehaviour
{
	private LevelManager levelManager;
	private MeshRenderer meshRenderer;

	void Start ()
	{
		
	}
	
	void LateUpdate ()
	{
		//TODO: draw quads at each stopping position (debug x first)
		//Debug.DrawLine(p);
	}
}
