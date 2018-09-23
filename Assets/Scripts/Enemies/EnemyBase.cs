using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MovementBehaviour
{
	public int ActiveStage;
	public EnemyManager EnemyManager;
	public LevelManager LevelManager;
	protected BeatManager beatManager;
	public BeatManager BeatManager
	{
		set
		{
			beatManager = value;
			beatManager.OnBeat += OnBeat;
		}
	}

	public abstract void Hit(Projectile projectile);
}
