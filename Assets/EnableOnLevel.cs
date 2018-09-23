using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnableOnLevel : MonoBehaviour
{

	public int[] levels;

	BeatMultiplier multiplier;

	public GameObject toEnable;

	// Update is called once per frame
	void Update ()
	{
		if(multiplier == null)
		{
			multiplier = FindObjectOfType<BeatMultiplier>();
		}
		
		if (multiplier != null && levels.Contains(multiplier.CurrentBeatKeeperLevel))
		{
			toEnable.SetActive(true);
		}
		else
		{
			toEnable.SetActive(false);
		}
	}
}
