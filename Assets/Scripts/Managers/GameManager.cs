﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameSettings gameSettings;

	public BeatManager BeatManager;
	public UIRoot UIRoot;
	public LevelManager LevelManager;

	void Start ()
	{
		BeatManager.OnBeat += OnBeat;

		Vector3 spawnPos = LevelManager.GetPlayerSpawn();
		GameObject player = Instantiate(gameSettings.PlayerPrefab);
		player.transform.position = spawnPos;
		PlayerMovementBehaviour playerMoveBehaviour = player.GetComponent<PlayerMovementBehaviour>();
		playerMoveBehaviour.LevelManager = LevelManager;
		playerMoveBehaviour.BeatManager = BeatManager;


		GameObject camera = Instantiate(gameSettings.CameraPrefab);
		OrbitFollowCamera orbitFollow = camera.GetComponent<OrbitFollowCamera>();
		orbitFollow.LevelManager = LevelManager;
		orbitFollow.Target = player.transform;
	}

	void OnBeat ()
	{
		
	}
}
