using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MovementBehaviour
{
    public enum EnemyType
    {
        Single,
        Double,
        Snake
    }

    public EnemySettings EnemySettings;
    public LevelManager LevelManager;
    private BeatManager beatManager;
    public BeatManager BeatManager
    {
        set
        {
            beatManager = value;
            beatManager.OnBeat += OnBeat;
        }
    }

    Vector2 velocity = new Vector2();

    public Rigidbody enemyRigidBody;
    public BoxCollider boxCollider;

    public EnemyType Type;
    float previousHorizontal;
    public float typeMultiplier = 1f;
    public float moveMultiplier = 1f;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        DecreaseVelocity();
        
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
        Debug.DrawLine(boxCollider.transform.position, boxCollider.transform.position + Vector3.down * (boxCollider.size.y + 0.2f), Color.green);
        if (Physics.Raycast(boxCollider.transform.position, Vector3.down, boxCollider.size.y + 0.5f, EnemySettings.groundRaycastLayer.value))
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

        if (velocity.y <= 0 && !OnGround()) velocity.y -= EnemySettings.gravity;
        enemyRigidBody.velocity = new Vector3(0, velocity.y, 0) + localHorizontal;
    }

    public override void OnBeat()
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
                break;
        }
    }

    public void VerticalMove(float typeMultiplier)
    {
        velocity.y = EnemySettings.verticalBoost * typeMultiplier;
    }

    public void HorizontalMove(float sign)
    {
        velocity.x = EnemySettings.horizontalBoost * sign;
    }

    public void SingleBehaviour(float verticalMove, float horizontalMove)
    {
        if (OnGround())
        {
            if (verticalMove > 7f)
            {
                VerticalMove(1f);
            }
        }

        HorizontalMove(moveMultiplier);
    }

    public void DoubleBehaviour(float verticalMove, float horizontalMove)
    {
        if (OnGround())
        {
        }
    }
}
