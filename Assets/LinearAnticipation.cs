using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearAnticipation : MonoBehaviour
{
	public AnimationCurve StartLineCurve;
	public LineRenderer LineRenderer;

	private int density = 10;

	void Start ()
	{
		LineRenderer.SetPositions(new Vector3[0]);
		LineRenderer.useWorldSpace = true;
	}

	public void Show(Vector3 start, Vector3 end)
	{
		List<Vector3> positions = new List<Vector3>();
		Vector3 distance = end - start;
		for(int i = 0; i < density; i++)
		{
			positions.Add(start + (distance / (10 - i)));
		}
		LineRenderer.SetPositions(positions.ToArray());
		LineRenderer.widthCurve = StartLineCurve;
	}

	void Update ()
	{
		Keyframe[] keys = LineRenderer.widthCurve.keys;
		for(int i = 0; i < keys.Length; i++)
		{
			keys[i].value =  Mathf.Max(0, keys[i].value - (Time.deltaTime * 5f));
		}

		AnimationCurve curve = new AnimationCurve(keys);
		LineRenderer.widthCurve = curve;
	}
}
