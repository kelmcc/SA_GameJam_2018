using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MovementBehaviour
{
    public LevelManager LevelManager;
    public BeatManager BeatManager;
    public BeatMultiplier BeatMultiplier;



    public BoxCollider boxCollider;

    public PlayerSettings PlayerSettings;

    Rigidbody playerRigidbody;

    float beatTimer;
    float leftTapTimer;
    float rightTapTimer;
    float upTapTimer;
    float downTapTimer;

    bool beatHit;

    float lastHorizontal;
    float lastVertical;

    float walkVelocity;

    float dontFallTime;

    private void Start()
    {
        BeatManager.OnBeat += OnBeat;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    Vector2 velocity = new Vector2();

    public void Update()
    {
        DecreaseVelocity();

        //get axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //=== BEAT MATCHING ===
        if (lastHorizontal == 0 && Mathf.Abs(horizontal) > 0)
        {
            if (horizontal > 0)
            {
                rightTapTimer = PlayerSettings.secondsLeeway;
            }
            else if (horizontal < 0)
            {
                leftTapTimer = PlayerSettings.secondsLeeway;
            }
        }

        if (lastVertical == 0 && Mathf.Abs(vertical) > 0)
        {
            if (vertical > 0)
            {
                upTapTimer = PlayerSettings.secondsLeeway;
            }
            else if (vertical < 0)
            {
                downTapTimer = PlayerSettings.secondsLeeway;
            }
        }

        if (beatTimer > 0)
        {
            if (upTapTimer > 0 && OnGround())
            {
                //Debug.Log("UP TO BEAT");
                beatHit = true;
                BeatMultiplier.Beat(PlayerSettings.beatInterval);
                velocity.y = PlayerSettings.verticalBoost;
                upTapTimer = 0;
            }
            else if (downTapTimer > 0)
            {
                //Debug.Log("DOWN TO BEAT");
                beatHit = true;
                BeatMultiplier.Beat(PlayerSettings.beatInterval);
                velocity.y = -PlayerSettings.verticalBoost;
                downTapTimer = 0;
            }

            if (leftTapTimer > 0)
            {
                //Debug.Log("LEFT TO BEAT");
                beatHit = true;
                BeatMultiplier.Beat(PlayerSettings.beatInterval);
                if (OnGround() && !PlayerSettings.groundDash)
                {
                    rightTapTimer = 0;
                }
                else
                {
                    velocity.x = -PlayerSettings.horizontalBoost;
                    leftTapTimer = 0;
                    dontFallTime = PlayerSettings.moveSidewaysNoFallTime;
                }
            }
            else if (rightTapTimer > 0)
            {
                beatHit = true;
                BeatMultiplier.Beat(PlayerSettings.beatInterval);
                if (OnGround() && !PlayerSettings.groundDash)
                {
                    //do nothing
                    rightTapTimer = 0;
                }
                else
                {
                    velocity.x = PlayerSettings.horizontalBoost;
                    //Debug.Log("RIGHT TO BEAT");
                    rightTapTimer = 0;
                    dontFallTime = PlayerSettings.moveSidewaysNoFallTime;
                }

            }
        }

        beatTimer -= Time.deltaTime;
        upTapTimer -= Time.deltaTime;
        leftTapTimer -= Time.deltaTime;
        rightTapTimer -= Time.deltaTime;


        lastHorizontal = horizontal;
        lastVertical = vertical;

        dontFallTime -= Time.deltaTime;

        //basic walk. no dt needed. added in fixed update
        walkVelocity = (horizontal * PlayerSettings.walkSpeed);
    }

    public void DecreaseVelocity()
    {
        if (velocity.x > 0)
        {
            velocity.x = Mathf.Max(0, velocity.x - (PlayerSettings.horizontalDecrese * Time.deltaTime));
        }
        else if (velocity.x < 0)
        {
            velocity.x = Mathf.Min(0, velocity.x + (PlayerSettings.horizontalDecrese * Time.deltaTime));
        }

        if (velocity.y > 0)
        {
            velocity.y = Mathf.Max(0, velocity.y - (PlayerSettings.verticalDecrese * Time.deltaTime));
        }
        else if (velocity.y < 0)
        {
            velocity.y = Mathf.Min(0, velocity.y + (PlayerSettings.verticalDecrese * Time.deltaTime));
        }
    }

    public bool OnGround()
    {
        //if (Physics.BoxCast(transform.position, new Vector3(collider.size.x, 0.5f, collider.size.y), 
        //	Vector3.down, collider.transform.rotation, collider.size.y + 5f, ~PlayerSettings.groundRaycastLayer.value))

        Debug.DrawLine(boxCollider.transform.position + (boxCollider.transform.forward * 1f), boxCollider.transform.position + Vector3.down * (boxCollider.size.y + 0.2f) + (boxCollider.transform.forward * 1f), Color.green);
		Debug.DrawLine(boxCollider.transform.position + (-boxCollider.transform.forward * 1f), boxCollider.transform.position + Vector3.down * (boxCollider.size.y + 0.2f) + (boxCollider.transform.forward * 1f), Color.green);
		if (Physics.Raycast(boxCollider.transform.position + (boxCollider.transform.forward * 1f), Vector3.down, boxCollider.size.y + 0.5f, PlayerSettings.groundRaycastLayer.value) ||
			Physics.Raycast(boxCollider.transform.position + (-boxCollider.transform.forward * 1f), Vector3.down, boxCollider.size.y + 0.5f, PlayerSettings.groundRaycastLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FixedUpdate()
    {
        //set rotation
        Vector3 goalVec = -(new Vector3(LevelManager.transform.position.x, transform.position.y, LevelManager.transform.position.z) - transform.position).normalized;
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.FromToRotation(transform.right, goalVec));

        //snap to circle
        Vector3 localHorizontal = transform.forward * velocity.x;
        Vector3 position = transform.position;
        LevelManager.SnapMovementToRadius(ref position, ref localHorizontal);
        Vector3 finalPosition = position;
        playerRigidbody.MovePosition(new Vector3(finalPosition.x, transform.position.y, finalPosition.z));

        if (velocity.y <= 0 && !OnGround() && dontFallTime <= 0)
        {
            velocity.y -= PlayerSettings.gravity;
        }
        playerRigidbody.velocity = new Vector3(0, velocity.y, 0) + localHorizontal;

        if (PlayerSettings.bascicMovement)
        {
            playerRigidbody.velocity += (transform.forward * walkVelocity);
        }
    }


    //stop the dude from falling off edges
    private void OnTriggerEnter(Collider other)
    {
        int layermask = PlayerSettings.edgeRaycastLayer.value;
        if (layermask == (layermask | (1 << other.gameObject.layer)))
        {
            velocity.x = 0;
        }
    }

	private void OnCollisionEnter(Collision collision)
	{
		DeathCube cube = collision.gameObject.GetComponent<DeathCube>();
		if (cube != null)
		{
			BeatMultiplier.AddLevelProgress(0.1f);
			AudioSource.PlayClipAtPoint(PlayerSettings.GotCubeAudio, Camera.main.transform.position, Random.Range(0.1f, 5f));
			Destroy(collision.gameObject);
		}
		else
		{
			EnemyBehaviour enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
			if(enemy == null)
			{
				enemy = collision.gameObject.transform.parent.GetComponent<EnemyBehaviour>();
			}
			if(enemy != null)
			{
				BeatMultiplier.RemoveLevelProggress();
				BeatManager.MuteFor(1);
			}
		}
	}

	public override void OnBeat()
    {
        if (!beatHit)
        {
            BeatMultiplier.MissedBeat();
        }

        beatTimer = PlayerSettings.secondsLeeway;
        beatHit = false;
    }
}
