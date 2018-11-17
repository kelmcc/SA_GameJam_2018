using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnHit : MonoBehaviour
{
	public Animator animator;

	private void OnTriggerEnter(Collider collider)
	{
		animator.SetTrigger("Shake");
	}
}
