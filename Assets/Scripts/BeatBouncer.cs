using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BeatBouncer : MonoBehaviour
{
	private Animator animator;
	private GameManager gameManager;
	void Start ()
	{
		animator = gameObject.GetComponent<Animator>();
		gameManager = GameObject.FindObjectOfType<GameManager>();

		if (gameManager != null)
		{
			gameManager.BeatManager.OnBeat += OnBeat;
		}
		else
		{
			BeatManager b = GameObject.FindObjectOfType<BeatManager>();
			b.OnBeat += OnBeat;
		}
	}
	
	void OnBeat ()
	{
		animator.SetTrigger("Bounce");
	}

	private void OnDestroy()
	{
		if(gameManager != null && gameManager.BeatManager != null)
		{
			gameManager.BeatManager.OnBeat -= OnBeat;
		}	
	}
}
