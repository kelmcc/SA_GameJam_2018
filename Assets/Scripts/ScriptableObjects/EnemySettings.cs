using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Jam/EnemySettings")]
public class EnemySettings : ScriptableObject
{
	public float walkSpeed = 10;

	public float horizontalBoost = 50;
	public float horizontalDecrease = 50;

	public float verticalBoost = 50;
	public float verticalDecrease = 50;

	public float gravity = 20;

	public float secondsLeeway = 0.2f;

	public LayerMask groundRaycastLayer;
	public LayerMask edgeRaycastLayer;
}
