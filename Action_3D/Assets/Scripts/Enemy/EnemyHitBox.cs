using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public bool canAttack;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player") && canAttack == true)
        {
            other.gameObject.GetComponent<PlayerInput>().PlayerDamage(10);
            canAttack = false;
        }
    }

}
