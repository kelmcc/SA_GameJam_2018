using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MovementBehaviour
{
	public LevelManager LevelManager;

	public void Update()
	{
		float horizontal = Input.GetAxis("Horizontal");
		
		transform.rotation = LevelManager.SnapRotationToRadius(transform.rotation);
		Vector3 localHorizontal = transform.forward * horizontal;
		LevelManager.SnapPositionToRadius(transform.position);
		transform.position = position;
		transform.position += localHorizontal;
	}


	public override void OnBeat()
	{

	}
}
