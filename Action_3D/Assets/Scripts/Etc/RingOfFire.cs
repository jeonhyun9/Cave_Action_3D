using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfFire : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    private float currTime = 0;
    private bool canDamage = true;
    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();   
    }

    private void OnEnable()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }
    }

    private void Update()
    {
        currTime += Time.deltaTime;
        if(currTime > 20)
        {
            currTime = 0;
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
        {
            other.transform.GetComponent<EnemyFSM1>().HitDamage(100, true);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BOSS"))
        {
            BossFireRingDamage(other);
        }
    }

    private void BossFireRingDamage(Collider other)
    {
        if(canDamage)
        {
            other.transform.GetComponent<BossFSM>().BossHitDamage(10);
            canDamage = false;
            StartCoroutine(WaitHalfSecond());
        }
    }

    IEnumerator WaitHalfSecond()
    {
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }
}
