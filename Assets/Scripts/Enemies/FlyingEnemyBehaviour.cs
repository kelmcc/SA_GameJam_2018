using System.Collections;
using UnityEngine;

public class FlyingEnemyBehaviour : EnemyBase
{
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

	Vector3 lastTargetPosition = Vector3.zero;
	float lastDistanceToCover = 0f;

	static FlyingEnemyBehaviour active = null;

    public Rigidbody enemyRigidBody;
    public BoxCollider boxCollider;
	private BeatMultiplier beatMultiplier;

	Transform player;

	private CubePool cubePool;

    public float moveMultiplier = 1f;

    void Start()
    {
		cubePool = FindObjectOfType<CubePool>();
		enemyRigidBody = GetComponent<Rigidbody>();
        previousPosition = Vector3.zero;
		TotalLife = Life;
		beatMultiplier = FindObjectOfType<BeatMultiplier>();

		player = FindObjectOfType<PlayerMovementBehaviour>().transform;
		lastTargetPosition = transform.position;
	}

    Vector3 previousPosition;
    public void Update()
    {
		if(beatMultiplier.CurrentBeatKeeperLevel != ActiveStage)
		{
			Destroy(gameObject);
		}

		Debug.DrawLine(transform.position, lastTargetPosition);
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
		if(Life <= 0)
		{
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

    void FixedUpdate()
    {
		enemyRigidBody.velocity = enemyRigidBody.velocity / 2f;

		//set rotation
		Vector3 goalVec = -(new Vector3(LevelManager.transform.position.x, transform.position.y, LevelManager.transform.position.z) - transform.position).normalized;
        enemyRigidBody.MoveRotation(enemyRigidBody.rotation * Quaternion.FromToRotation(transform.right, goalVec));

		Vector3 position = Vector3.Lerp(transform.position, lastTargetPosition, 0.125f);
		Vector3 direction = position - transform.position.normalized;
        LevelManager.SnapMovementToRadius(ref position, ref direction);

        enemyRigidBody.MovePosition(position);

		Debug.DrawLine(transform.position, position, Color.red);
    }

    public override void OnBeat()
    {
		
	

		Vector3 difference = player.transform.position - transform.position;
		float distance = difference.magnitude;
		Vector3 direction = difference.normalized;
		if (distance < 30)
		{
			if(active != this && active != null)
			{
				return;
			}

			if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
			{
				lastTargetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
				lastDistanceToCover = Vector3.Distance(transform.position, lastTargetPosition);
			}
			else
			{
				lastTargetPosition = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
				lastDistanceToCover = Vector3.Distance(transform.position, lastTargetPosition);
			}
			active = this;
		}
		else
		{
			lastTargetPosition = transform.position + (Random.insideUnitSphere * 10f) + (player.position - transform.position).normalized * 5;
		}
	}

	private void SetTarget(Transform target)
	{

	}

	private void OnDestroy()
	{
		if(active == this)
		{
			active = null;
		}

		beatManager.OnBeat -= OnBeat;
		EnemyManager.RemoveEnemy(this);
	}
}
