﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	public GameObject Reticule;
	public PlayerSettings playerSettings;

	public bool MouseInput = true;

	public Transform Up;
	public Transform Down;
	public Transform Right;
	public Transform Left;

	protected LevelManager levelManager;
	private BeatManager beatManager;
	private Transform reticuleT;

	public Projectile Projectile;
	public LineRenderer LineRenderer;

	float fireTimer;
	float beatTimer;

	Vector3 lastDirection = Vector3.left;

	protected void BaseStart()
	{
		levelManager = FindObjectOfType<LevelManager>();
		beatManager = FindObjectOfType<BeatManager>();
		beatManager.OnBeat += OnBeat;
		reticuleT = Instantiate(Reticule).transform;
		reticuleT.gameObject.SetActive(false);
		//Cursor.visible = false;
	}

	private void OnBeat(long beatCount)
	{
		beatTimer = playerSettings.secondsLeeway;
	}

	public abstract void Fire(Vector3 direction, float intensity);

	public virtual void UpdateAim()
	{
		Vector3 screenPosition = Input.mousePosition;
		bool fire = false;
		if (!MouseInput)
		{
			float horizontal = Input.GetAxis("ShootHorizontal");
			float vertical = Input.GetAxis("ShootVertical");

			Vector3 target = transform.position;
			if (horizontal > 0)
			{
				if(vertical > 0)
				{
					target = Vector3.Lerp(Right.position, Up.position, 0.5f);
				}
				else if(vertical < 0)
				{
					target = Vector3.Lerp(Right.position, Down.position, 0.5f);
				}
				else
				{
					target = Right.position;
				}
				fire = true;
			}
			else if (horizontal < 0)
			{
				if (vertical > 0)
				{
					target = Vector3.Lerp(Left.position, Up.position, 0.5f);
				}
				else if (vertical < 0)
				{
					target = Vector3.Lerp(Left.position, Down.position, 0.5f);
				}
				else
				{
					target = Left.position;
				}
				fire = true;
			}
			else if(vertical > 0)
			{
				if (horizontal > 0)
				{
					target = Vector3.Lerp(Up.position, Right.position, 0.5f);
				}
				else if (horizontal < 0)
				{
					target = Vector3.Lerp(Up.position, Left.position, 0.5f);
				}
				else
				{
					target = Up.position;
				}
				fire = true;
			}
			else if (vertical < 0)
			{
				if (horizontal > 0)
				{
					target = Vector3.Lerp(Down.position, Right.position, 0.5f);
				}
				else if (horizontal < 0)
				{
					target = Vector3.Lerp(Down.position, Left.position, 0.5f);
				}
				else
				{
					target = Down.position;
				}
				fire = true;
			}
			else
			{
				fire = false;
				fireTimer = 0;
			}
			screenPosition = Camera.main.WorldToScreenPoint(target);
		}

	
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);

		Vector3 closestPoint = FindClosestPointOnLevel(ray);

		Debug.DrawRay(ray.origin, ray.direction * 100);

		if (closestPoint != Vector3.zero)
		{
			Debug.DrawLine(closestPoint, Camera.main.transform.position, Color.green);
			Debug.DrawLine(closestPoint, closestPoint + Vector3.up * 4, Color.yellow);
			Debug.DrawLine(closestPoint, closestPoint + -Vector3.up * 4, Color.yellow);

			lastDirection = (closestPoint - transform.position + (Vector3.up * 1f)).normalized;

			//DrawAim(transform.position + (1 * Vector3.up), closestPoint);

			if (Input.GetButton("Fire") || fire)
			{
				fireTimer = playerSettings.secondsLeeway;
			}
		}
		else
		{
			HideAim();
		}

		//DrawAim(transform.position, closestPoint);
	}

	protected void BaseUpdate()
	{
		beatTimer -= Time.deltaTime;
		fireTimer -= Time.deltaTime;


		if(beatTimer > 0 && fireTimer > 0)
		{
			Fire(lastDirection, 3);
			beatTimer = 0;
			fireTimer = 0;
		}
	}

	//raymarch to level
	private Vector3 FindClosestPointOnLevel(Ray ray)
	{
		float step = 50f;
		float distance = 0;
		float difference = Mathf.Infinity;
		float epsilon = 0.2f;
		float maxAcceptableDistance = 2000f;

		Vector3 point = Vector3.zero;
		while (difference > epsilon)
		{
			if (difference > 0)
			{
				distance += step;
			}
			else
			{
				distance -= step;
			}

			point = ray.GetPoint(distance);
			Vector3 levelPos = new Vector3(levelManager.transform.position.x, point.y, levelManager.transform.position.z);
			difference = Vector3.Distance(point, levelPos) - levelManager.Radius;

			if (Mathf.Abs(difference) < step)
			{
				step /= 2;
			}

			//will occur if we missed the level entirley
			if (step < epsilon || distance > maxAcceptableDistance)
			{
				point = Vector3.zero;
				break;
			}
		}

		//returns zero if never intersects
		return point;
	}

	private void DrawAim(Vector3 start, Vector3 end)
	{
		reticuleT.gameObject.SetActive(true);
		reticuleT.transform.position = end;
		reticuleT.transform.forward = -(end - Camera.main.transform.position).normalized;

		/*
		List<Vector3> points = new List<Vector3>();

		float distance = Vector3.Distance(start, end);
		float divideDistance = 5f;
		float distancePerPoint = distance / divideDistance;

		for (float d = 10; d < distance; d += distancePerPoint)
		{
			Vector3 offset = (end - start) * d/distance;
			Vector3 point = start + offset;
			levelManager.SnapPositionToRadius(ref point);
			points.Add(point);
		}

		points.Add(end);

		//LineRenderer.enabled = true;

		LineRenderer.startWidth = 0;
		LineRenderer.endWidth = 0.25f;

		LineRenderer.SetPositions(points.ToArray());
		*/
	}

	private void HideAim()
	{
		//LineRenderer.enabled = false;
	}
}
