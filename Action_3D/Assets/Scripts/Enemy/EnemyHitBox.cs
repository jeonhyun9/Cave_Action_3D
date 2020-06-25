using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Player"))
        {
            other.gameObject.GetComponent<PlayerInput>().PlayerDamage(10);
        }
    }
}
