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
		gameManager.BeatManager.OnBeat += OnBeat;
	}
	
	void OnBeat ()
	{
		animator.SetTrigger("Bounce");
	}

	private void OnDestroy()
	{
		gameManager.BeatManager.OnBeat -= OnBeat;
	}
}
