using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitFollowCamera : MonoBehaviour
{
	public Transform Target;
	public LevelManager LevelManager;

	private Camera cam;

	void Start ()
	{
		cam = GetComponent<Camera>();
	}
	
	void Update ()
	{
		
	}
}
