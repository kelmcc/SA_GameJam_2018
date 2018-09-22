using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public BeatManager BeatManager;
	public UIRoot UIRoot;

	void Start ()
	{
		BeatManager.OnBeat += OnBeat;
	}

	void OnBeat ()
	{
		
	}
}
