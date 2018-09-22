﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Jam/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
	public float walkSpeed = 10;

	public float horizontalBoost = 50;
	public float horizontalDecrese = 50;
	public float verticalBoost = 50;
	public float verticalDecrese = 50;

	public float secondsLeeway = 0.3f;
}
