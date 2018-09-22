using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    //Spawn the enemies on the scene
    //Keep a min level op un-captured enemies

    public GameObject EnemyPrefab;
    public LevelManager LevelManager;
    private BeatManager beatManager;
    public BeatManager BeatManager
    {
        set
        {
            beatManager = value;
            beatManager.OnBeat += OnBeat;
        }
    }


    //manage the enemy spawning



    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void OnBeat()
    {
        //every beat interval, spawn new enemies
        if (LevelManager.GetActiveEnemySpawners().Count > 0)
        {
            SpawnEnemy();
        }
    }

    [ContextMenu("Spawn Enemy")]
    GameObject SpawnEnemy()
    {
        Vector3 spawnPos = LevelManager.GetEnemySpawn();
        GameObject enemy = Instantiate(EnemyPrefab);
        enemy.transform.SetParent(transform);
        enemy.transform.position = spawnPos;
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        //setup the enemy behaviour
        enemyBehaviour.LevelManager = LevelManager;
        enemyBehaviour.BeatManager = beatManager;


        return enemy;
    }
}
