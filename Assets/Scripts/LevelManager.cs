using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CODE ASSUMES THE LEVEL MANAGER IS AT WORLD ZERO. DONT MOVE IT
public class LevelManager : MonoBehaviour
{
	public LevelSettings LevelSettings;

	public Transform playerTransform;
	public List<EnemySpawnTransform> enemyTransforms;

	public Vector3 GetPlayerSpawn()
	{
		//force player spawn position onto circle
		Vector3 position = playerTransform.position;
		SnapPositionToRadius(ref position);
		playerTransform.position = position;
		return playerTransform.position;
	}

    public Vector3 GetEnemySpawn()
    {
        int index = Random.Range(0, enemyTransforms.Count);
        while (!enemyTransforms[index].IsActivated)
        {
            index = Random.Range(0, enemyTransforms.Count);
        }
        
        Vector3 position = enemyTransforms[index].transform.position;
		SnapPositionToRadius(ref position);
		enemyTransforms[index].transform.position = position;
		return enemyTransforms[index].transform.position;
    }

    public List<EnemySpawnTransform> GetActiveEnemySpawners()
    {
        List<EnemySpawnTransform> activeSpawners = new List<EnemySpawnTransform>();
        foreach (EnemySpawnTransform spawner in enemyTransforms)
        {
            if (spawner.IsActivated)
            {
                activeSpawners.Add (spawner);
            }
        }
        return activeSpawners;
    }

	//takes a direction and projects it onto the circle
	public void SnapMovementToRadius(ref Vector3 position, ref Vector3 direction)
	{
		//snap position if its moved (physics drift, large direction magnitudes)
		SnapPositionToRadius(ref position);

		//correct direction vector
		Vector3 nieveTargetPoint = position + direction;
		Vector3 centerPos = new Vector3(transform.position.x, position.y, transform.position.z);
		Vector3 centerToTarget = nieveTargetPoint - centerPos;

		Debug.DrawLine(centerPos, centerPos + centerToTarget, Color.red);

		Vector3 projectedPoint = centerToTarget.normalized * LevelSettings.movementRadius;
		//we now have added the magnitude of the initial direction back, but with the modfied direction for the circle
		Vector3 finalPoint = (projectedPoint - position).normalized * direction.magnitude; 
	}

	public void SnapPositionToRadius(ref Vector3 position)
	{
		Vector3 centerPos = new Vector3(transform.position.x, position.y, transform.position.z);
		position = centerPos + (position - centerPos).normalized * LevelSettings.movementRadius;
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
