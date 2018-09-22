using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    //Spawn the enemies on the scene
    //Keep a min level op un-captured enemies

    public GameObject EnemyPrefab;
    public LevelManager LevelManager;
    public BeatManager BeatManager;


    //manage the enemy spawning


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("Spawn Enemy")]
    GameObject SpawnEnemy()
    {
        Vector3 spawnPos = LevelManager.GetEnemySpawn();
		GameObject enemy = Instantiate(EnemyPrefab);
		enemy.transform.position = spawnPos;
		EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        //setup the enemy behaviour

        return enemy;
    }
}
