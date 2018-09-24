using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnStage : MonoBehaviour
{

	public int stage;

	public float delay;

	private BeatMultiplier multiplier;

	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	Coroutine c = null;
	void Update()
	{
		if (multiplier == null)
		{
			multiplier = FindObjectOfType<BeatMultiplier>();
		}

		if (multiplier != null && stage == multiplier.CurrentBeatKeeperLevel && c == null)
		{
			c = StartCoroutine(Animate(delay));
		}
	}

	IEnumerator Animate(float delay)
	{
		yield return new WaitForSeconds(delay);

		animator.SetTrigger("Animate");
	}
}
