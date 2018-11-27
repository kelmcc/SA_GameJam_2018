using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnticipation : MonoBehaviour {

	public ParticleSystem ParticleSystem;

	public void Show(Vector3 point)
	{
		ParticleSystem.transform.position = point;
		ParticleSystem.Play();
	}
}
