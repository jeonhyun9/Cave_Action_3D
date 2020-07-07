using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    float currTime = 0;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        currTime = 0;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }
    }

    private void Update()
    {
        if (currTime < 4)
        {
            currTime += Time.deltaTime;
        }
        else this.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currTime < 1)
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
