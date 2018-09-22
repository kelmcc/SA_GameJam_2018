using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Jam/GameSettings")]
public class GameSettings : ScriptableObject
{
	public GameObject PlayerPrefab;
	public GameObject CameraPrefab;
}
