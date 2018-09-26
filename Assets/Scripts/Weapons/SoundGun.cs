using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGun : Weapon
{
	float intensitySpeedMultiplier = 0.25f;
	private PlayerMovementBehaviour player;

	public CubePool cubePool;
	public BeatMultiplier beatMultiplier;

	public void Start()
	{
		base.BaseStart();
		player = GetComponent<PlayerMovementBehaviour>();
		cubePool = FindObjectOfType<CubePool>();
		beatMultiplier = FindObjectOfType<BeatMultiplier>();
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

			Vector3 offset = Vector3.zero;
			if(beatMultiplier.CurrentBeatKeeperLevel == 0)
			{
				offset = Vector3.up * 1f;
			}

			projectileObject.transform.position = gameObject.transform.position + offset;
			projectileObject.transform.forward = direction;
			Projectile proj = projectileObject.GetComponent<Projectile>();
			proj.cubePool = cubePool;
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
