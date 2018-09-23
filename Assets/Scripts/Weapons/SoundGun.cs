using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGun : Weapon
{
	float intensitySpeedMultiplier = 0.25f;

	public override void Fire(Vector3 direction, float intensity)
	{
		StartCoroutine(SpawnProjectiles(direction, intensity));
	}

	IEnumerator SpawnProjectiles(Vector3 direction, float intensity)
	{
		for (int i = 0; i < intensity; i++)
		{
			GameObject projectileObject = Instantiate(Projectile.gameObject);
			projectileObject.transform.position = gameObject.transform.position + (direction * 1f) + (Vector3.up * 2f);
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
