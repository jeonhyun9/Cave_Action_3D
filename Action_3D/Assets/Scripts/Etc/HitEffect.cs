using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private Transform tr;
    private ParticleSystem particle;

    private void Awake()
    {
        //컴포넌트 할당
        tr = GetComponent<Transform>();
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        particle.Clear();
        particle.Simulate(0.0f,true,true);
        particle.Play(true);
        StartCoroutine(ParticleOff());
    }

    private void OnDisable()
    {
        if(transform.parent.transform.name.Contains("Prefab"))
        {
            tr.position = Vector3.zero;
        }
        particle.Play(false);
    }

    IEnumerator ParticleOff()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }
}
