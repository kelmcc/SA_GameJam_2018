using System;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelSettings", menuName ="Jam/LevelSettings")]
public class LevelSettings : ScriptableObject
{
	public float movementRadius;

	public float LevelCircumfrance
	{
		get
		{
			return 2f * Mathf.PI * movementRadius;
		}
	}


	public float verticalMoveGap;
	public float horizontalMoveGap;


	private void OnValidate()
	{
		//Ensure the horizontalMoveGap fits the circumfrance seamlessly
		float numStepsInCircumfrance = LevelCircumfrance / horizontalMoveGap;
		int closestSteps = (int)Mathf.Floor(numStepsInCircumfrance);
		if (Mathf.Repeat(numStepsInCircumfrance, 1f) > 0.5f)
		{
			closestSteps += 1;
		}

		horizontalMoveGap = LevelCircumfrance / closestSteps;
	}
}
