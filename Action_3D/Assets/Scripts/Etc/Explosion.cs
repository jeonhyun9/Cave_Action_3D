using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    bool isBoom = false;
    float currTime = 0;

    private void Update()
    {
        if(currTime < 2)
        {
            currTime += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(currTime < 1)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                other.transform.GetComponent<EnemyFSM1>().HitDamage(100, true);
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("BOSS"))
            {
                other.transform.GetComponent<BossFSM>().BossHitDamage(100);
            }
        }
    }
}
