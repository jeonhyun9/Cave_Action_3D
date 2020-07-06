using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    bool canAttack;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player") && canAttack == true)
        {
            other.gameObject.GetComponent<PlayerInput>().PlayerDamage(20,"KNOCKBACK");
            canAttack = false;
        }
    }

    private void OnEnable()
    {
        canAttack = true;
    }
    private void OnDisable()
    {
        canAttack = false;
    }
}
