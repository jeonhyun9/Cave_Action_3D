using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerInput.Instance.state == PlayerInput.PlayerState.ATTACK)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                other.transform.GetComponent<EnemyFSM1>().HitDamage(100,this.transform.position);
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


