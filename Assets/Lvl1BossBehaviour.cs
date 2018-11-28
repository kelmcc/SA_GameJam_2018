using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl1BossBehaviour : MonoBehaviour
{

    PlayerMovementBehaviour player;
    BeatMultiplier multiplier;

    public ParticleAnticipation Anticipation;

    public Transform trackBeforeLevel;

    public float vertMoveSpeed = 1f;

    BeatManager beatManager;

    public Transform LeftHand;
    public Transform RightHand;

    private int beatCount = 0;

    public Vector3 leftLerpPoint;
    public Vector3 rightLerpPoint;

    public Vector3 leftOldLerpPoint;
    public Vector3 rightOldLerpPoint;

    public Transform leftIdleTrans;
    public Transform rightIdleTrans;

    int currentHand = 0;

    public AudioClip[] MainEnemyGroan;
    public AudioSource VoicePlayer;
    public AudioClip[] HandSounds;
    public AudioSource HandPlayer;

    // Use this for initialization
    void Start()
    {
        beatManager = FindObjectOfType<BeatManager>();
        beatManager.OnBeat += OnBeat;
    }

    float beatTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovementBehaviour>();
        }
        if (multiplier == null)
        {
            multiplier = FindObjectOfType<BeatMultiplier>();
        }

        transform.forward = (new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position);

        if (multiplier.CurrentBeatKeeperLevel != 2)
        {
            transform.position = new Vector3(transform.position.x, trackBeforeLevel.position.y, transform.position.z);
            return;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        }

        //else if (currentHand == 0) // swapped around so we move back when not active
        {
            //rightLerpPoint = rightIdleTrans.position;
        }
        //else
        {
            //leftLerpPoint = leftIdleTrans.position;
        }

        RightHand.transform.position = Vector3.Lerp(leftOldLerpPoint, rightLerpPoint, Mathf.Clamp01(beatTime * beatManager.Bps * 2));
        LeftHand.transform.position = Vector3.Lerp(rightOldLerpPoint, leftLerpPoint, Mathf.Clamp01(beatTime * beatManager.Bps * 2));

        RightHand.transform.forward = (RightHand.transform.position - new Vector3(transform.position.x, RightHand.transform.position.y, transform.position.z)).normalized;
        LeftHand.transform.forward = (LeftHand.transform.position - new Vector3(transform.position.x, LeftHand.transform.position.y, transform.position.z)).normalized;

        beatTime += Time.deltaTime;
    }

    public void OnBeat(long unusedBeatCount)
    {
        if (multiplier == null)
        {
            multiplier = FindObjectOfType<BeatMultiplier>();
        }
        if (multiplier.CurrentBeatKeeperLevel != 2)
        {
            return;
        }

        //attack
        if (beatCount == 0)
        {
            beatTime = 0;
            currentHand = Random.Range(0, 2);
            if (currentHand == 0)
            {
                leftOldLerpPoint = leftIdleTrans.position;
                leftLerpPoint = player.transform.position;

                rightOldLerpPoint = rightIdleTrans.position;
                rightLerpPoint = rightIdleTrans.position;

                Anticipation.Show(leftLerpPoint);
            }
            else
            {
                rightOldLerpPoint = rightIdleTrans.position;
                rightLerpPoint = player.transform.position;

                leftOldLerpPoint = leftIdleTrans.position;
                leftLerpPoint = leftIdleTrans.position;

                Anticipation.Show(rightLerpPoint);
            }
            int precentage = Random.Range(0, 10);

            //determine 10%
            if (precentage <= 2)
            {
                VoicePlayer.PlayOneShot(MainEnemyGroan[Random.Range(0, MainEnemyGroan.Length)], 0.7f);
            }
            HandPlayer.PlayOneShot(HandSounds[Random.Range(0, HandSounds.Length)], 0.7f);

        }
        else if (beatCount == 3)
        {
            beatTime = 0;
            if (currentHand == 0)
            {
                leftOldLerpPoint = leftLerpPoint;
                leftLerpPoint = leftIdleTrans.position;
            }
            else
            {
                rightOldLerpPoint = rightLerpPoint;
                rightLerpPoint = rightIdleTrans.position;
            }
        }

        beatCount++;
        if (beatCount > 3)
        {
            beatCount = 0;
        }
    }


    private void OnDestroy()
    {
        beatManager.OnBeat -= OnBeat;
    }
}
