using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Jam/GameSettings")]
public class GameSettings : ScriptableObject
{
	public GameObject PlayerPrefab;
	public GameObject CameraPrefab;

    public GameObject EnemyManager;
    public GameObject EnemyPrefab;
    public GameObject TallEnemyPrefab;
    public GameObject SnakeEnemyPrefab;
	public GameObject FlyingEnemyPrefab;

	public int StartLevel = 0;
}
