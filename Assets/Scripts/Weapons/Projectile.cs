using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject OnDestroyParticleSystem;

	[Space]
	public float Speed;
	public LevelManager LevelManager;
	
	void Update ()
	{
		Vector3 movement = transform.forward * Speed;
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref movement);
		transform.position = position + movement;
	}

	private void OnTriggerEnter(Collider coll)
	{
		if(OnDestroyParticleSystem != null)
		{
			GameObject.Instantiate(OnDestroyParticleSystem);
		}		
		Destroy(gameObject);
	}
}
