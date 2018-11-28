using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementBehaviour : MovementBehaviour
{
    public LevelManager LevelManager;
    public BeatManager BeatManager;
    public BeatMultiplier BeatMultiplier;
	public UIRoot UIRoot;

	private CubePool cubePool;

    public CapsuleCollider capsuleCollider;

    public PlayerSettings PlayerSettings;

	public GameObject wing1;
	public GameObject wing2;

	public GameObject halo;

	Rigidbody playerRigidbody;

	public Image ProgressImage;

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
		cubePool = FindObjectOfType<CubePool>();
	}

    Vector2 velocity = new Vector2();

    public void Update()
    {

		if (BeatMultiplier.CurrentBeatKeeperLevel > 0)
		{
			wing1.SetActive(true);
			wing2.SetActive(true);
		}
		else
		{
			wing1.SetActive(false);
			wing2.SetActive(false);
		}

		if(BeatMultiplier.CurrentBeatKeeperLevel > 1)
		{
			halo.SetActive(true);
		}
		else
		{
			halo.SetActive(false);
		}

		DecreaseVelocity();

		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(vKey))
			{
				//your code here
				Debug.Log("Key pressed: " + vKey);
			}
		}

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
		downTapTimer -= Time.deltaTime;

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
		if(BeatMultiplier.CurrentBeatKeeperLevel > 0)
		{
			//fly
			return true;
		}


        Debug.DrawLine(capsuleCollider.transform.position + (capsuleCollider.transform.forward * 1f), capsuleCollider.transform.position + Vector3.down * (capsuleCollider.height + 0.2f) + (capsuleCollider.transform.forward * 1f), Color.green);
		Debug.DrawLine(capsuleCollider.transform.position + (-capsuleCollider.transform.forward * 1f), capsuleCollider.transform.position + Vector3.down * (capsuleCollider.height + 0.2f) + (capsuleCollider.transform.forward * 1f), Color.green);
		if (Physics.Raycast(capsuleCollider.transform.position + (capsuleCollider.transform.forward * 1f), Vector3.down, capsuleCollider.height + 0.5f, PlayerSettings.groundRaycastLayer.value) ||
			Physics.Raycast(capsuleCollider.transform.position + (-capsuleCollider.transform.forward * 1f), Vector3.down, capsuleCollider.height + 0.5f, PlayerSettings.groundRaycastLayer.value))
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

		layermask = PlayerSettings.bossRaycastLayer.value;
		if (layermask == (layermask | (1 << other.gameObject.layer)))
		{
            
			BeatMultiplier.SubtractLevelProgress(15);
			BeatManager.MuteFor(1);
			UIRoot.ShowOverlayFor(1.5f);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		DeathCube cube = collision.gameObject.GetComponent<DeathCube>();
		if (cube != null)
		{
			BeatMultiplier.AddLevelProgress(0.5f);
			AudioSource.PlayClipAtPoint(PlayerSettings.GotCubeAudio, Camera.main.transform.position, Random.Range(0.1f, 0.5f));
			cubePool.RecycleCube(cube);
		}
		else
		{
			EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
			if(enemy == null)
			{
				enemy = collision.gameObject.transform.parent.GetComponent<EnemyBase>();
			}
			if(enemy != null)
			{
				BeatMultiplier.SubtractLevelProgress(15);
				BeatManager.MuteFor(1);
				UIRoot.ShowOverlayFor(1.5f);
			}
		}
	}

	public override void OnBeat(long beatCount)
    {
        if (!beatHit)
        {
            BeatMultiplier.MissedBeat();
        }

        beatTimer = PlayerSettings.secondsLeeway;
        beatHit = false;
    }
}
