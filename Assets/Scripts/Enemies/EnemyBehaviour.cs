using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MovementBehaviour
{
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

    Rigidbody enemyRigidBody;

    float walkVelocity;

    public BoxCollider boxCollider;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        DecreaseVelocity();

        //basic walk. no dt needed. added in fixed update
        walkVelocity = (EnemySettings.walkSpeed);
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
        //if (Physics.BoxCast(transform.position, new Vector3(collider.size.x, 0.5f, collider.size.y), 
        //	Vector3.down, collider.transform.rotation, collider.size.y + 5f, ~PlayerSettings.groundRaycastLayer.value))

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

    //stop the dude from falling off edges
    private void OnTriggerEnter(Collider other)
    {
        int layermask = EnemySettings.edgeRaycastLayer.value;
        if (layermask == (layermask | (1 << other.gameObject.layer)))
        {
            velocity.x = 0;
        }
    }

    float previousHorizontal;
    public override void OnBeat()
    {
        float horizontalMove = Random.Range(0f, 10f);
        float verticalMove = Random.Range(0f, 10f);


        if (OnGround())
        {
            if (verticalMove > 7f)
            {
                velocity.y = EnemySettings.verticalBoost;
            }

            if (horizontalMove < 5f)
            {
                velocity.x = -EnemySettings.horizontalBoost;
            }
            else
            {
                velocity.x = EnemySettings.horizontalBoost;
            }
            
            previousHorizontal = velocity.x;
        }
        else
        {
            if (previousHorizontal < 0)
                velocity.x = -EnemySettings.horizontalBoost;
            else
                velocity.x = EnemySettings.horizontalBoost;
        }
    }
}
