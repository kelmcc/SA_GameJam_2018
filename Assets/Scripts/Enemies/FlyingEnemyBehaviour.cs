using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyBehaviour : EnemyBase
{
	private int TotalLife;
	public int Life;

	public GameObject DeathCube;
	public int DeathCubeCount;

    public EnemySettings EnemySettings;
  
    public void Unsub()
    {
        beatManager.OnBeat -= OnBeat;
    }

    Vector2 velocity = new Vector2();

	Vector3 lastTargetPosition = Vector3.zero;

    public Rigidbody enemyRigidBody;
    public BoxCollider boxCollider;
	private BeatMultiplier beatMultiplier;

	Transform player;

    public float moveMultiplier = 1f;
	bool targetFound = false;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
        previousPosition = Vector3.zero;
		TotalLife = Life;
		beatMultiplier = FindObjectOfType<BeatMultiplier>();

		player = FindObjectOfType<PlayerMovementBehaviour>().transform;
	}

    Vector3 previousPosition;
    public void Update()
    {
		if(beatMultiplier.CurrentBeatKeeperLevel != ActiveStage)
		{
			Destroy(gameObject);
		}

        DecreaseVelocity();

		if(targetFound)
		{
			if (lastTargetPosition.x - transform.position.x < 1f)
			{
				velocity.x = lastTargetPosition.x - -transform.position.x;
			}
			else if (lastTargetPosition.y - transform.position.y < 1f)
			{
				velocity.y = lastTargetPosition.y - -transform.position.y;
			}
		}	
	}

    public void DecreaseVelocity()
    {
		if(!targetFound)
		{
			if (velocity.x > 0)
			{
				velocity.x = Mathf.Max(0, velocity.x - (EnemySettings.horizontalDecrease * Time.deltaTime));
			}
			else if (velocity.x < 0)
			{
				velocity.x = Mathf.Min(0, velocity.x + (EnemySettings.horizontalDecrease * Time.deltaTime));
			}

			if (velocity.y > 0)
			{
				velocity.y = Mathf.Max(0, velocity.y - (EnemySettings.verticalDecrease * Time.deltaTime));
			}
			else if (velocity.y < 0)
			{
				velocity.y = Mathf.Min(0, velocity.y + (EnemySettings.verticalDecrease * Time.deltaTime));
			}
		}       
    }

	public override void Hit(Projectile projectile)
	{
		if (projectile != null)
		{
			StartCoroutine(TakeHit(projectile.transform.position));
		}
	}

	private IEnumerator TakeHit(Vector3 position)
	{
		Life--;

		for (int i = 0; i < DeathCubeCount; i++)
		{
			GameObject cubeOb = Instantiate(DeathCube);
			cubeOb.transform.position = transform.position;
			yield return new WaitForSeconds(0.05f);

			while(transform.localScale.x > (0))
			{
				transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 2, transform.localScale.x - Time.deltaTime * 2, transform.localScale.x - Time.deltaTime * 2);
				if(transform.localScale.x > 1)
				{
					transform.localScale = Vector3.one;
				}
				yield return null;
			}
		}

		if(Life <= 0)
		{
			Destroy(gameObject);
		}
	}

    void FixedUpdate()
    {
        //set rotation
        Vector3 goalVec = -(new Vector3(LevelManager.transform.position.x, transform.position.y, LevelManager.transform.position.z) - transform.position).normalized;
        enemyRigidBody.MoveRotation(enemyRigidBody.rotation * Quaternion.FromToRotation(transform.right, goalVec));

        //snap to circle
        Vector3 localHorizontal = transform.forward * velocity.x;
        Vector3 position = transform.position;
        LevelManager.SnapMovementToRadius(ref position, ref localHorizontal);
        Vector3 finalPosition = position;
        enemyRigidBody.MovePosition(new Vector3(finalPosition.x, transform.position.y, finalPosition.z));

        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player

        enemyRigidBody.velocity = new Vector3(0, velocity.y, 0) + localHorizontal;
    }

    public override void OnBeat()
    {
		lastTargetPosition = player.transform.position;
		Vector3 difference = player.transform.position - transform.position;
		float distance = difference.magnitude;
		Vector3 direction = difference.normalized;
		if (distance < 50)
		{
			targetFound = true;
			if(Mathf.Abs(difference.x) > 0.5f)
			{
				Vector3 dummyPos = Vector3.zero;
				Vector3 horizontalDir = new Vector3(direction.x, 0, direction.z);
				LevelManager.SnapMovementToRadius(ref dummyPos, ref horizontalDir);
				velocity = horizontalDir * EnemySettings.horizontalBoost;
			}
			else if (Mathf.Abs(difference.y) > 0.5f)
			{
				velocity.y = direction.y * EnemySettings.horizontalBoost;
			}
		}
		else
		{
			targetFound = false;
			velocity = Random.insideUnitSphere.normalized * EnemySettings.horizontalBoost;
		}
	}

	private void OnDestroy()
	{
		beatManager.OnBeat -= OnBeat;
		EnemyManager.RemoveEnemy(this);
	}
}
