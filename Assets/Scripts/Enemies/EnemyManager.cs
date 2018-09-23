using System.Collections;
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
	public GameObject FlyingEnemyPrefab;

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

    List<EnemyBase> enemies = new List<EnemyBase>();
	//manage the enemy spawning

	public List<int> maxEnemiesPerLevel;

    public EnemySettings EnemySettings;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

	private void Update()
	{
		
	}

	public void OnBeat()
    {
        //every beat interval, spawn new enemies
        if (enemies.Count < maxEnemiesPerLevel[BeatMultiplier.CurrentBeatKeeperLevel])
        {
			Vector3 spawnPos;
			if(LevelManager.GetEnemySpawn(BeatMultiplier.CurrentBeatKeeperLevel, out spawnPos))
			{
				if(BeatMultiplier.CurrentBeatKeeperLevel == 0)
				{
					enemies.Add(SpawnEnemy<EnemyBehaviour>(spawnPos, EnemyPrefab, BeatMultiplier.CurrentBeatKeeperLevel));
				}
				else
				{
					enemies.Add(SpawnEnemy<FlyingEnemyBehaviour>(spawnPos, FlyingEnemyPrefab, BeatMultiplier.CurrentBeatKeeperLevel));
				}
			}      
        }
    }

    [ContextMenu("Spawn Enemy")]
    T SpawnEnemy<T>(Vector3 spawnPos, GameObject prefab, int currentStage) where T : EnemyBase
    {
        GameObject enemy = Instantiate(prefab);
        enemy.transform.SetParent(transform);
        enemy.transform.position = spawnPos;
        EnemyBase enemyBehaviour = enemy.GetComponent<T>();
        enemyBehaviour.EnemyManager = this;
		enemyBehaviour.ActiveStage = currentStage;

		//setup the enemy behaviour
		enemyBehaviour.LevelManager = LevelManager;
        enemyBehaviour.BeatManager = beatManager;

        return (T)enemyBehaviour;
    }

	public void RemoveEnemy(EnemyBase e)
	{
		enemies.Remove(e);
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

            enemies.Add(SpawnEnemy<EnemyBehaviour>(pos, TallEnemyPrefab, BeatMultiplier.CurrentBeatKeeperLevel));
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

            enemies.Add(SpawnEnemy<EnemyBehaviour>(pos, SnakeEnemyPrefab, BeatMultiplier.CurrentBeatKeeperLevel));
        }
    }
}
