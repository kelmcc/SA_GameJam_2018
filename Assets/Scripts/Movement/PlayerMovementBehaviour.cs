using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MovementBehaviour
{
	public LevelManager LevelManager;
	public BeatManager BeatManager;

	public PlayerSettings PlayerSettings;

	float beatTimer;
	float leftTapTimer;
	float rightTapTimer;
	float upTapTimer;
	float downTapTimer;

	float lastHorizontal;
	float lastVertical;

	private void Start()
	{
		BeatManager.OnBeat += OnBeat;
	}

	Vector2 velocity = new Vector2();

	public void Update()
	{
		if(velocity.x > 0)
		{
			velocity.x = Mathf.Max(0, velocity.x - (PlayerSettings.horizontalDecrese * Time.deltaTime));
		}
		else if (velocity.x < 0)
		{
			velocity.x = Mathf.Min(0, velocity.x + (PlayerSettings.horizontalDecrese * Time.deltaTime));
		}

		//set orentation
		transform.right = -(LevelManager.transform.position - transform.position).normalized;

		//get axes
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		//=== BEAT MATCHING ===
		if (lastHorizontal == 0 && Mathf.Abs(horizontal) > 0)
		{
			if(horizontal > 0)
			{
				rightTapTimer = PlayerSettings.secondsLeeway;
			}
			else if(horizontal < 0)
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

		if(beatTimer > 0)
		{
			if (upTapTimer > 0)
			{
				Debug.Log("UP TO BEAT");
				velocity.y = PlayerSettings.verticalBoost;
				upTapTimer = 0;
			}
			else if (downTapTimer > 0)
			{
				Debug.Log("DOWN TO BEAT");
				velocity.y = -PlayerSettings.verticalBoost;
				downTapTimer = 0;
			}

			if (leftTapTimer > 0)
			{
				Debug.Log("LEFT TO BEAT");
				velocity.x = -PlayerSettings.horizontalBoost;
				leftTapTimer = 0;
			}
			else if (rightTapTimer > 0)
			{
				velocity.x = PlayerSettings.horizontalBoost;
				Debug.Log("RIGHT TO BEAT");

				rightTapTimer = 0;
			}
		}

		beatTimer -= Time.deltaTime;
		upTapTimer -= Time.deltaTime;
		leftTapTimer -= Time.deltaTime;
		rightTapTimer -= Time.deltaTime;

		lastHorizontal = horizontal;
		lastVertical = vertical;

		//=== MOVE THE DUDE ===

		//basic walk
		float horDT = (horizontal * Time.deltaTime * PlayerSettings.walkSpeed) + velocity.x * Time.deltaTime;

		Vector3 localHorizontal = transform.forward * horDT;

		//snap to circle
		Vector3 position = transform.position;
		LevelManager.SnapMovementToRadius(ref position, ref localHorizontal);
		transform.position = position;
		transform.position += localHorizontal;
	}


	public override void OnBeat()
	{
		Debug.Log("PlayerBeat");
		beatTimer = PlayerSettings.secondsLeeway;
	}
}
