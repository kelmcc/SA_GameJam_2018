using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutScreen : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private float timer;
    public float idleTimeout;
    private float warningTimer;
    public float warningDuration;

    [SerializeField]
    GameManager _manager;

    // Use this for initialization
    void Start()
    {
        timer = 0f;
        warningTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Input.anyKeyDown)
        {
            timer = 0f;
            warningTimer = 0f;

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Out") && !_animator.IsInTransition(0))
            {
                _animator.SetTrigger("TurnOff");
            }
        }

        if (timer > idleTimeout)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("In") && !_animator.IsInTransition(0))
            {
                _animator.SetTrigger("TurnOn");
            }

            warningTimer += Time.deltaTime;

            if (warningTimer > warningDuration)
            {
                warningTimer = 0f;
                timer = 0f;
                _manager.FinishGame();
            }
        }
    }
}
