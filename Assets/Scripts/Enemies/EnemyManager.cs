using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    //Spawn the enemies on the scene
    //Keep a min level op un-captured enemies
    [HideInInspector]
    public GameObject EnemyPrefab;

    [HideInInspector]
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
            enemies.Add(SpawnEnemy());
        }
    }

    [ContextMenu("Spawn Enemy")]
    EnemyBehaviour SpawnEnemy()
    {
        Vector3 spawnPos = LevelManager.GetEnemySpawn();
        GameObject enemy = Instantiate(EnemyPrefab);
        enemy.transform.SetParent(transform);
        enemy.transform.position = spawnPos;
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        //setup the enemy behaviour
        enemyBehaviour.LevelManager = LevelManager;
        enemyBehaviour.BeatManager = beatManager;


        return enemyBehaviour;
    }
}
