using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    int damage;
    // Start is called before the first frame update
    //private void Update()
    //{
    //    if (PlayerInput.Instance.isEnchanted) damage = 100;
    //    else damage = 80;
    //    Debug.DrawRay(this.transform.position, this.transform.forward * 0.4f, Color.green);
    //    RaycastHit hit;
    //    //에너미 레이어와 보스 레이어만 검출한다.
    //    if (Physics.Raycast(transform.position, this.transform.forward, out hit, 0.4f, 1 << 8 | 1 << 12))
    //    {
    //        if(PlayerInput.Instance.isSwordOn)
    //        {
    //            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BOSS"))
    //            {
    //                hit.collider.gameObject.transform.GetComponent<BossFSM>().BossHitDamage(hit.point, hit.normal, damage);
    //            }
    //            else
    //            {
    //                hit.collider.gameObject.transform.GetComponent<EnemyFSM1>().HitDamage(hit.point, hit.normal, damage);
    //            }
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerInput.Instance.isPowerAttack)
        {
            damage = 100;
            if (PlayerInput.Instance.isEnchanted) damage = 100 + (PlayerInput.Instance.fireCapacity * 10);
        }
        else
        {
            damage = 80;
            if (PlayerInput.Instance.isEnchanted) damage = 80 + (PlayerInput.Instance.fireCapacity * 10);
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


