using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCallback : MonoBehaviour
{
	public event Action<Collider> TriggerEntered = delegate { };
	public LayerMask LayerMask;

	private void OnTriggerEnter(Collider other)
	{
		int layerMask = LayerMask.value;
		if (layerMask == (layerMask | (1 << other.gameObject.layer)))
		{
			TriggerEntered(other);
		}
	}
}
