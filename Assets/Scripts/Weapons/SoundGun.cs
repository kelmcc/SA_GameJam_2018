using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGun : Weapon
{

	float intensitySpeedMultiplier = 5;

	public override void Fire(Vector3 direction, float intensity)
	{
		GameObject projectileObject = Instantiate(Projectile.gameObject);
		Projectile proj = projectileObject.GetComponent<Projectile>();
		proj.LevelManager = levelManager;
		proj.Speed = intensity * intensitySpeedMultiplier;
	}

	public void Update()
	{
		UpdateAim();
	}
}
