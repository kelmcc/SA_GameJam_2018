using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CODE ASSUMES THE LEVEL MANAGER IS AT WORLD ZERO. DONT MOVE IT
public class LevelManager : MonoBehaviour
{
	public LevelSettings LevelSettings;

	public Transform playerTransform;

	public Vector3 GetPlayerSpawn()
	{
		//force player spawn position onto circle
		Vector3 position = playerTransform.position;
		SnapPositionToRadius(ref position);
		playerTransform.position = position;
		return playerTransform.position;
	}


	/*//takes a direction and projects it onto the circle
	public void SnapMovementToRadius(ref Vector3 position, ref Vector3 direction)
	{
		//snap position if its moved (physics drift, large direction magnitudes)
		SnapPositionToRadius(ref position);

		//correct direction vector
		Vector3 nieveTargetPoint = position + direction;
		Vector3 centerPos = new Vector3(transform.position.x, position.y, transform.position.z);
		Vector3 centerToTarget = nieveTargetPoint - centerPos;
		Vector3 projectedPoint = centerToTarget.normalized * LevelSettings.movementRadius;
		//we now have added the magnitude of the initial direction back, but with the modfied direction for the circle
		Vector3 finalPoint = (projectedPoint - position).normalized * direction.magnitude; 
	}
*/
	public void SnapRotationToRadius(ref Vector3 position)
	{
		Vector3 centerPos = new Vector3(transform.position.x, position.y, transform.position.z);
		position = (position - centerPos).normalized * LevelSettings.movementRadius;
	}

	public void SnapPositionToRadius(ref Vector3 position)
	{
		Vector3 centerPos = new Vector3(transform.position.x, position.y, transform.position.z);
		position = (position - centerPos).normalized * LevelSettings.movementRadius;
	}
	
	private void OnDrawGizmos()
	{
		//draw circle guides for level creation

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, LevelSettings.movementRadius);
		Gizmos.DrawWireSphere(transform.position + Vector3.up * 30, LevelSettings.movementRadius);
		Gizmos.DrawWireSphere(transform.position + Vector3.up * 60, LevelSettings.movementRadius);
		Gizmos.DrawWireSphere(transform.position + Vector3.up * 90, LevelSettings.movementRadius);
		Gizmos.DrawWireSphere(transform.position + Vector3.up * 120, LevelSettings.movementRadius);

		Gizmos.color = Color.white;
	}
}
