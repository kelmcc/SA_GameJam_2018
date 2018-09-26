using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
	public Queue<GameObject> normalEnemies;
	public Queue<GameObject> tallEnemies;
	public Queue<GameObject> snakeEnemies;
	public Queue<GameObject> flyingEnemies;
	public Queue<GameObject> flyingAngelEnemies;
}
