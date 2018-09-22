using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MovementBehaviour
{
	public LevelManager LevelManager;

	public void Update()
	{
		float horizontal = Input.GetAxis("Horizontal");
		Vector3 localHorizontal = transform.forward * horizontal;

		//snap to circle
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref localHorizontal);
		transform.position = position;
		transform.position += localHorizontal;
	}


	public override void OnBeat()
	{

	}
}
