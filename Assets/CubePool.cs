using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePool : MonoBehaviour
{
	public GameObject deathCubePrefab;

	private Queue<DeathCube> freeCubes;

	private void Start()
	{
		freeCubes = new Queue<DeathCube>();
	}

	public DeathCube GetCube(Material material, Vector3 initialPosition)
	{
		DeathCube newCube;
		if (freeCubes.Count > 0)
		{
			newCube = freeCubes.Dequeue();
		}
		else
		{
			newCube = Instantiate(deathCubePrefab).GetComponent<DeathCube>();
		}
		newCube.gameObject.SetActive(true);
		newCube.transform.position = initialPosition;
		newCube.SetMaterial(material);
		return newCube;
	}

	public void RecycleCube(DeathCube cube)
	{
		freeCubes.Enqueue(cube);
		cube.gameObject.SetActive(false);
	}
}
