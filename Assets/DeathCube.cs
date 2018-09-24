using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCube : MonoBehaviour {

	private Transform target;
	private Rigidbody rb;

	static int aliveCount = 0;
	// Use this for initialization
	void Start ()
	{
		aliveCount++;
		target = FindObjectOfType<PlayerMovementBehaviour>().transform;
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float speed = 0.5f;
		Vector3 dir = (target.position - transform.position).normalized;
		//if (aliveCount > 10)
		//{
			rb.MovePosition(transform.position + dir * speed * Random.Range(0.5f, 1f));
		//}
		//else
		//{
			//rb.AddForce(dir * 40);
		//}		
	}

	private void OnDestroy()
	{
		aliveCount--;
	}
}
