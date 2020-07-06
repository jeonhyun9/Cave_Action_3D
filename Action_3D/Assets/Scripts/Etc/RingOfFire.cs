using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfFire : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    private float currTime = 0;

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
        if(currTime > 60)
        {
            currTime = 0;
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
        {
            other.transform.GetComponent<EnemyFSM1>().HitDamage(100, true);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("BOSS"))
        {
            other.transform.GetComponent<BossFSM>().BossHitDamage(10);
        }
    }
}
