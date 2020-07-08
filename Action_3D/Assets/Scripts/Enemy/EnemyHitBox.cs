using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public bool canAttack;
    public bool isTypeMiniBoss;

    private void OnTriggerEnter(Collider other)
    {
        if(isTypeMiniBoss)
        {
            if (other.name.Contains("Player") && canAttack == true)
            {
                other.gameObject.GetComponent<PlayerInput>().PlayerDamage(10,"KNOCKBACK");
                canAttack = false;
            }
        }
        else
        {
            if (other.name.Contains("Player") && canAttack == true)
            {
                other.gameObject.GetComponent<PlayerInput>().PlayerDamage(10);
                canAttack = false;
            }
        }
        
        
    }

}
