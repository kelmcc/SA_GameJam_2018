﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject OnDestroyParticleSystem;

	[Space]
	public float Speed;
	public LevelManager LevelManager;

	public float lifetime = 5f;

	public float startScale = 0;

	private void Start()
	{
		transform.localScale = new Vector3(startScale, startScale, startScale);
	}

	void Update ()
	{
		Vector3 movement = transform.forward * Speed;
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref movement);
		transform.position = position + movement;
		lifetime -= Time.deltaTime;
		if(lifetime <=0)
		{
			DestroyProjectile();
		}

		if(transform.localScale.x < 1)
		{
			float scaleIncrease = Time.deltaTime * 6f;
			transform.localScale = new Vector3(transform.localScale.x + scaleIncrease, transform.localScale.y + scaleIncrease, transform.localScale.z + scaleIncrease);
		}
	}

	private void OnTriggerEnter(Collider coll)
	{
		DestroyProjectile();
	}

	void DestroyProjectile()
	{
		if (OnDestroyParticleSystem != null)
		{
			GameObject.Instantiate(OnDestroyParticleSystem);
		}
		Destroy(gameObject);
	}
}