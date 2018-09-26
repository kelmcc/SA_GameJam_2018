using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCube : MonoBehaviour
{
	private Transform target;
	private Rigidbody rb;

	public Renderer Renderer;

	public void SetMaterial(Material mat)
	{
		Renderer.sharedMaterial = mat;
	}

	void Start ()
	{
		target = FindObjectOfType<PlayerMovementBehaviour>().transform;
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float speed = 1f;
		Vector3 dir = (target.position +(Vector3.up * 1f) - transform.position).normalized;

		rb.MovePosition(transform.position + dir * speed * Random.Range(0.5f, 1f));
	}
}
