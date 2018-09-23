﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    //Spawn the enemies on the scene
    //Keep a min level op un-captured enemies
    [HideInInspector]
    public GameObject EnemyPrefab;
    public GameObject TallEnemyPrefab;
    public GameObject SnakeEnemyPrefab;

    [HideInInspector]
    public LevelManager LevelManager;
    private BeatManager beatManager;
    public BeatMultiplier BeatMultiplier;

    public BeatManager BeatManager
    {
        set
        {
            beatManager = value;
            beatManager.OnBeat += OnBeat;
        }
    }

    List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    //manage the enemy spawning

    public EnemySettings EnemySettings;

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
        if (enemies.Count < EnemySettings.maxEnemies)
        {
            enemies.Add(SpawnEnemy(LevelManager.GetEnemySpawn(), EnemyPrefab));
        }
    }

    [ContextMenu("Spawn Enemy")]
    EnemyBehaviour SpawnEnemy(Vector3 spawnPos, GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);
        enemy.transform.SetParent(transform);
        enemy.transform.position = spawnPos;
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.EnemyManager = this;

        //setup the enemy behaviour
        enemyBehaviour.LevelManager = LevelManager;
        enemyBehaviour.BeatManager = beatManager;

        return enemyBehaviour;
    }

    public void Merge(EnemyBehaviour a, EnemyBehaviour b)
    {
        if (BeatMultiplier.CurrentBeatKeeperLevel > 0)
        {
            enemies.Remove(a);
            enemies.Remove(b);

            Vector3 pos = a.transform.localPosition;

            GameObject.Destroy(a.gameObject);
            GameObject.Destroy(b.gameObject);

            enemies.Add(SpawnEnemy(pos, TallEnemyPrefab));
        }
    }

    public void Snake(EnemyBehaviour a, EnemyBehaviour b)
    {
        if (BeatMultiplier.CurrentBeatKeeperLevel > 1)
        {
            enemies.Remove(a);
            enemies.Remove(b);

            Vector3 pos = a.transform.localPosition;
            GameObject.Destroy(a.gameObject);
            GameObject.Destroy(b.gameObject);

            enemies.Add(SpawnEnemy(pos, SnakeEnemyPrefab));
        }
    }
}
