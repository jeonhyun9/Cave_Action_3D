using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    int damage;
    
    private void OnTriggerEnter(Collider other)
    {
        //강공격이면 공격력 상승
        if (PlayerInput.Instance.isPowerAttack)
        {
            damage = 120;
            if (PlayerInput.Instance.isEnchanted) damage = 140 + (PlayerInput.Instance.fireCapacity * 10);
        }
        else
        {
            damage = 80;
            if (PlayerInput.Instance.isEnchanted) damage = 100 + (PlayerInput.Instance.fireCapacity * 10);
        }
        
        if (PlayerInput.Instance.state == PlayerInput.PlayerState.ATTACK)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                EnemyFSM1 enemy = other.transform.GetComponent<EnemyFSM1>();
                if(enemy.state != EnemyFSM1.EnemyState.DAMAGED && enemy.state != EnemyFSM1.EnemyState.DIE)
                {
                    enemy.HitDamage(Vector3.zero, transform.position, damage);
                }
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("BOSS"))
            {
                BossFSM boss = other.transform.GetComponent<BossFSM>();
                if(boss.GetCanHit()==true)
                {
                    boss.BossHitDamage(Vector3.zero, transform.position, damage);
                }
            }
        }
    }

    //콜리전엔터로 사용하는 방법

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (PlayerInput.Instance.state == PlayerInput.PlayerState.ATTACK)
    //    {
    //        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
    //        {
    //            print("충돌함");
    //            collision.collider.GetComponent<EnemyFSM1>().HitDamage(100);
    //            Vector3 pos = collision.contacts[0].point;
    //            Vector3 _normal = collision.contacts[0].normal;
    //            //충돌 시 방향 벡터 회전값
    //            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
    //
    //            //혈흔 효과 생성
    //            GameObject blood = Instantiate<GameObject>(PlayerInput.Instance.hitPrefab, pos, rot);
    //            Destroy(blood, 1.0f);
    //        }
    //    }
    //}
}


