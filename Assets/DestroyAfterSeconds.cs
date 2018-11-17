using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
	public float Seconds;
	private float currentTime;

	void Start ()
	{
		currentTime = 0;
	}
	
	void Update ()
	{
		if(currentTime >= Seconds)
		{
			Destroy(gameObject);
		}
		else
		{
			currentTime += Time.deltaTime;
		}
	}
}
