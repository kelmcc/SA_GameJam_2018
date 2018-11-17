using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnHit : MonoBehaviour
{
	private Animator animator;

	private void OnCollisionEnter(Collision collision)
	{
		animator.SetTrigger("Shake");
	}
}
