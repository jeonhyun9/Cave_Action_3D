using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public enum EnemyState
    {
        IDLE,
        MOVE,
        TRACE,
        HIT,
        DIE,
    }

    public float HP = 100;
    EnemyState state;
    Rigidbody rigidBody;
    Animator EnemyAnim;
    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.IDLE;
        EnemyAnim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Sword") && state != EnemyState.HIT)
        {
            EnemyAnim.SetTrigger("HIT");
            int rand = UnityEngine.Random.Range(1, 3);
            EnemyAnim.SetInteger("HITNUM", rand);
            StartCoroutine(HitCoroutine());
        }
    }
    //// Update is called once per frame
    //private void OnCollisionEnter(Collision collision)
    //{
    //    
    //}

    IEnumerator HitCoroutine()
    {
        state = EnemyState.HIT;
        HP -= 10;
        print("적 HP : " + HP);
        yield return new WaitForSeconds(0.1f);
        state = EnemyState.IDLE;
    }
}
