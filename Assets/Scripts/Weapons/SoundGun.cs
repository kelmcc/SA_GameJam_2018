using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGun : Weapon
{
	float intensitySpeedMultiplier = 0.25f;
	private PlayerMovementBehaviour player;

	public void Start()
	{
		base.BaseStart();
		player = GetComponent<PlayerMovementBehaviour>();
	}

	public override void Fire(Vector3 direction, float intensity)
	{
		StartCoroutine(SpawnProjectiles(direction, intensity));
	}

	IEnumerator SpawnProjectiles(Vector3 direction, float intensity)
	{
		for (int i = 0; i < intensity; i++)
		{
			GameObject projectileObject = Instantiate(Projectile.gameObject);
			projectileObject.transform.position = gameObject.transform.position + (direction * 1f) + (player.OnGround()?(Vector3.up * 2f):Vector3.zero);
			projectileObject.transform.forward = direction;
			Projectile proj = projectileObject.GetComponent<Projectile>();
			proj.LevelManager = levelManager;
			proj.Speed = intensity * intensitySpeedMultiplier;

			yield return new WaitForSeconds(0.05f);
		}
	}

	public void Update()
	{
		UpdateAim();
		BaseUpdate();
	}
}
