using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject OnDestroyParticleSystem;
	public LayerMask HitLayerMask;
	public LayerMask BossHitLayerMask;

	public GameObject bossCube;

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
		//set rotation
		Vector3 towardsCenter = -(new Vector3(LevelManager.transform.position.x, transform.position.y, LevelManager.transform.position.z) - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(transform.forward, towardsCenter);

		Vector3 movement = transform.forward * Speed;
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref movement);
		//transform.forward = movement.normalized;
		transform.position = position + transform.forward * Speed;
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
		int layerMask = HitLayerMask.value;
		if (layerMask == (layerMask | (1 << coll.gameObject.layer)))
		{
			EnemyBase enemy = coll.gameObject.GetComponent<EnemyBase>();
			if(enemy == null)
			{
				enemy = coll.gameObject.transform.parent.GetComponent<EnemyBase>();
			}
			
			if (enemy != null)
			{
				enemy.Hit(this);
			}
			DestroyProjectile();
		}

		layerMask = BossHitLayerMask.value;
		if (layerMask == (layerMask | (1 << coll.gameObject.layer)))
		{
			for(int i = 0; i < 3; i++)
			{
				GameObject c = Instantiate(bossCube);
				c.transform.position = transform.position;
			}
		}
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
