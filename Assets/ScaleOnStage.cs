using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnStage : MonoBehaviour
{
	public int stage;

	public float delay;

	private BeatMultiplier multiplier;

	public Transform toScale;
	private Vector3 goalScale;

	private void Start()
	{
		goalScale = toScale.localScale;
		toScale.localScale = Vector3.zero;
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
			c = StartCoroutine(Scale(delay));
		}
	}

	IEnumerator Scale(float delay)
	{
		yield return new WaitForSeconds(delay);

		while(toScale.localScale.x < goalScale.x || toScale.localScale.y < goalScale.y || toScale.localScale.z < goalScale.z)
		{
			toScale.localScale = new Vector3(Mathf.Min(goalScale.x, toScale.localScale.x + Time.deltaTime * 2f),
				Mathf.Min(goalScale.y, toScale.localScale.y + Time.deltaTime * 2f),
				Mathf.Min(goalScale.z, toScale.localScale.z + Time.deltaTime * 2f));

			yield return null;
		}
	}
}
