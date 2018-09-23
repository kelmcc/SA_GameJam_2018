using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouse : MonoBehaviour
{
	public float multiplier;

	private void Update()
	{
		Vector2 offset = -Input.mousePosition * multiplier;
		transform.localPosition = offset;
	}
}
