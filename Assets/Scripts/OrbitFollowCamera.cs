using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitFollowCamera : MonoBehaviour
{
	public float distance = 80f;
	public float slerpSpeed = 20f;

	public Transform Target;
	public LevelManager LevelManager;

	private Camera cam;

	void Start ()
	{
		cam = GetComponent<Camera>();
	}
	
	void LateUpdate ()
	{
		Vector3 levelManagerPos = new Vector3(LevelManager.transform.position.x,Target.position.y, LevelManager.transform.position.z);
		Vector3 dirFromLvlManager = (Target.position - levelManagerPos).normalized;
		Vector3 offsetFromLvlManager = dirFromLvlManager * distance;

		Debug.DrawLine(levelManagerPos, levelManagerPos + offsetFromLvlManager, Color.cyan);

		Vector3 goalPos = levelManagerPos + offsetFromLvlManager;

		transform.position = Vector3.Slerp(transform.position, goalPos, Time.deltaTime * slerpSpeed);
		Vector3 targetLookAt = Vector3.Lerp(transform.forward, Target.position, Time.deltaTime * slerpSpeed);
		transform.LookAt(targetLookAt);	
	}
}
