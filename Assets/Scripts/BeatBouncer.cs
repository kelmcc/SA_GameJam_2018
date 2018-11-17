using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BeatBouncer : MonoBehaviour
{
	private Animator animator;

	private static BeatManager beatManager;
	void Start ()
	{
		animator = gameObject.GetComponent<Animator>();

		if(beatManager == null)
		{
			beatManager = GameObject.FindObjectOfType<BeatManager>();
		}
		beatManager.OnBeat += OnBeat;
	}
	
	void OnBeat (long beatCount)
	{
		animator.SetTrigger("Bounce");
	}

	private void OnDestroy()
	{
		if(beatManager != null)
		{
			beatManager.OnBeat -= OnBeat;
		}	
	}
}
