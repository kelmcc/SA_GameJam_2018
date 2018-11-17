using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject OnDestroyParticleSystem;

	public Material bossCubeMaterial;
	public CubePool cubePool;

	[Space]
	public float Speed;
	public LevelManager LevelManager;

	public TriggerCallback BossTriggerCallback;
	public TriggerCallback EnemyTriggerCallback;
	public TriggerCallback ObstacleTriggerCallback;

	public float lifetime = 5f;

	public float startScale = 0.5f;

	private void Start()
	{
		transform.localScale = new Vector3(startScale, startScale, startScale);

		BossTriggerCallback.TriggerEntered += OnBossTrigger;
		EnemyTriggerCallback.TriggerEntered += OnEnemyTrigger;
		ObstacleTriggerCallback.TriggerEntered += OnObstacleTrigger;
	}

	void Update ()
	{
		//set rotation
		Vector3 towardsCenter = -(new Vector3(LevelManager.transform.position.x, transform.position.y, LevelManager.transform.position.z) - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(transform.forward, towardsCenter);

		Vector3 movement = transform.forward * Speed;
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref movement);
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

	private void OnEnemyTrigger(Collider coll)
	{
		EnemyBase enemy = coll.gameObject.GetComponent<EnemyBase>();
		if (enemy == null)
		{
			enemy = coll.gameObject.transform.parent.GetComponent<EnemyBase>();
		}

		if (enemy != null)
		{
			enemy.Hit(this);
		}
		DestroyProjectile();
	}

	private void OnBossTrigger(Collider coll)
	{
		for (int i = 0; i < 2; i++)
		{
			cubePool.GetCube(bossCubeMaterial, transform.position);
		}
	}

	private void OnObstacleTrigger(Collider coll)
	{
		DestroyProjectile();
	}

	void DestroyProjectile()
	{
		if (OnDestroyParticleSystem != null)
		{
			GameObject.Instantiate(OnDestroyParticleSystem);
			OnDestroyParticleSystem.transform.position = transform.position;
		}
		Destroy(gameObject);
	}
}
