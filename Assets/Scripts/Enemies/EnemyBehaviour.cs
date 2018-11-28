using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : EnemyBase
{
    public enum EnemyType
    {
        Single,
        Double,
        Snake
    }

	private int TotalLife;
	public int Life;

	public Material DeathCubeMat;
	public int DeathCubeCount;

    public EnemySettings EnemySettings;

    public void Unsub()
    {
        beatManager.OnBeat -= OnBeat;
    }

    Vector2 velocity = new Vector2();

    public Rigidbody enemyRigidBody;
    public BoxCollider boxCollider;
	private BeatMultiplier beatMultiplier;
	private CubePool cubePool;

	public EnemyType Type;
    public float moveMultiplier = 1f;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
        previousPosition = Vector3.zero;
		TotalLife = Life;
		beatMultiplier = FindObjectOfType<BeatMultiplier>();
		cubePool = FindObjectOfType<CubePool>();

	}

    Vector3 previousPosition;
    public void Update()
    {
		if(beatMultiplier.CurrentBeatKeeperLevel != ActiveStage)
		{
			Destroy(gameObject);
		}

        DecreaseVelocity();
        timer += Time.deltaTime;
    }

    public void DecreaseVelocity()
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

	public bool OnGround()
	{
		if (boxCollider == null)
			return false;

		float yOffset = 0.5f;
		if (Type == EnemyType.Double)
			yOffset = 1f;

		Debug.DrawLine(boxCollider.transform.position, boxCollider.transform.position + Vector3.down * (boxCollider.size.y + 0.2f), Color.green);

		RaycastHit[] hits1 = Physics.RaycastAll(boxCollider.transform.position + (boxCollider.transform.forward * 1f), Vector3.down, boxCollider.size.y + yOffset, EnemySettings.groundRaycastLayer.value);
		RaycastHit[] hits2 = Physics.RaycastAll(boxCollider.transform.position + (-boxCollider.transform.forward * 1f), Vector3.down, boxCollider.size.y + yOffset, EnemySettings.groundRaycastLayer.value);

		bool hitSomething = false;
		foreach(RaycastHit hit in hits1)
		{
			if(hit.collider.gameObject != gameObject && hit.collider.gameObject.transform.parent != gameObject)
			{
				hitSomething = true;
			}
		}
		foreach (RaycastHit hit in hits2)
		{
			if (hit.collider.gameObject != gameObject && hit.collider.gameObject.transform.parent != gameObject)
			{
				hitSomething = true;
			}
		}

		if (hitSomething)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            moveMultiplier = -moveMultiplier;
        }

        // Calculate Angle Between the collision point and the player
        collisionDir = collision.contacts[0].point - transform.position;

        // We then get the opposite (-Vector3) and normalize it
        collisionDir = -collisionDir.normalized * 0.1f;
    }


	Vector3 collisionDir;

    float timer = 0f;
    Collider currentCollider;
    private void OnTriggerEnter(Collider collider)
    {
		if(collider.gameObject.layer == LayerMask.NameToLayer(EnemySettings.killZoneLayer))
		{
			Destroy(gameObject);
			return;
		}

		PlayerMovementBehaviour player = collider.GetComponentInParent<PlayerMovementBehaviour>();
		if (player != null)
		{
			//Destroy(gameObject);
			//return;
		}

		EnemyBehaviour enemy = collider.GetComponent<EnemyBehaviour>();
        if (timer > 5f && enemy != null && Type == EnemyType.Single)
        {
            timer = 0f;
            currentCollider = collider;
        }
		else
		{
			int layermask = EnemySettings.edgeRaycastLayer.value;
			if (layermask == (layermask | (1 << collider.gameObject.layer)))
			{
				velocity.x = 0;
			}
		}
	}

	Coroutine hitC = null;
	public override void Hit(Projectile projectile)
	{
		if (projectile != null && hitC == null)
		{
			hitC = StartCoroutine(TakeHit(projectile.transform.position));
		}
	}

	private IEnumerator TakeHit(Vector3 position)
	{
		Life--;
		if (Life <= 0)
		{
            boxCollider.enabled = false;
			for (int i = 0; i < DeathCubeCount; i++)
			{
				cubePool.GetCube(DeathCubeMat, transform.position);
				yield return new WaitForSeconds(0.05f);				
			}

			while (transform.localScale.x > 0)
			{
				transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 2, transform.localScale.y - Time.deltaTime * 2, transform.localScale.z - Time.deltaTime * 2);
				if (transform.localScale.x < 0)
				{
					transform.localScale = Vector3.zero;
					break;
				}
				yield return null;
			}

			Destroy(gameObject);
		}

		hitC = null;
	}

    private void OnTriggerStay(Collider collider)
    {
        if (collider == currentCollider && timer > 1f)
        {
            timer = 0f;

            if (collider.transform.localPosition.y > 0f)
            {
                EnemyManager.Merge(this, collider.GetComponent<EnemyBehaviour>());
            }
            else
            {
                EnemyManager.Snake(this, collider.GetComponent<EnemyBehaviour>());
            }
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
        enemyRigidBody.MovePosition(new Vector3(finalPosition.x, transform.position.y, finalPosition.z) + collisionDir);

        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player

        if (velocity.y <= 0 && !OnGround()) velocity.y -= EnemySettings.gravity;
        enemyRigidBody.velocity = new Vector3(0, velocity.y, 0) + localHorizontal;
    }

    public override void OnBeat(long beatCount)
    {
        float horizontalMove = Random.Range(0f, 10f);
        float verticalMove = Random.Range(0f, 10f);

        switch (Type)
        {
            case EnemyType.Single:
                SingleBehaviour(verticalMove, horizontalMove);
                break;
            case EnemyType.Double:
                DoubleBehaviour(verticalMove, horizontalMove);
                break;
            case EnemyType.Snake:
                SnakeBehaviour(verticalMove, horizontalMove);
                break;
        }
    }

    public void VerticalMove(float typeMultiplier)
    {
        velocity.y = EnemySettings.verticalBoost * typeMultiplier;
    }

    public void HorizontalMove(float sign)
    {
		//dont move sideways if we have hit an edge above level 1
		if(boxCollider != null && Physics.CheckBox(boxCollider.center, boxCollider.size, boxCollider.transform.rotation, EnemySettings.edgeRaycastLayer))
		{
			if(beatMultiplier.CurrentBeatKeeperLevel == 0)
			{
				velocity.x = EnemySettings.horizontalBoost * sign;
			}			
		}   
		else
		{
			velocity.x = EnemySettings.horizontalBoost * sign;
		}
    }

    public void SingleBehaviour(float verticalMove, float horizontalMove)
    {
        if (Mathf.Abs(velocity.y) < 0.1f && verticalMove > 5f)
        {
            VerticalMove(1f);
        }

        HorizontalMove(moveMultiplier);
    }

    public void DoubleBehaviour(float verticalMove, float horizontalMove)
    {
        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            VerticalMove(1.1f);
        }
        HorizontalMove(moveMultiplier * 0.5f);
    }

    public void SnakeBehaviour(float verticalMove, float horizontalMove)
    {
        HorizontalMove(moveMultiplier * 1.2f);
    }

	private void OnDestroy()
	{
		beatManager.OnBeat -= OnBeat;
		EnemyManager.RemoveEnemy(this);
	}
}
