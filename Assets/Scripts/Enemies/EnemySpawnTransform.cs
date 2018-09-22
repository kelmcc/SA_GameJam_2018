using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//voting that these should be speakers
public class EnemySpawnTransform : MonoBehaviour
{

    //turn off if there are enough Dancers around it
    public bool IsActivated = true;
    public int enemyCount = 0;

    public float sphereCastRadius = 5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 p1 = transform.position + new Vector3(0f, 0.01f, 0f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCastRadius, transform.forward, 10f);
        enemyCount = 0;

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    enemyCount++;
                }
            }
        }

        if (enemyCount>10f && IsActivated)
        {
            IsActivated = false;
        }
        
        if (enemyCount<10f && !IsActivated)
        {
            IsActivated = true;
        }
        
    }
}
