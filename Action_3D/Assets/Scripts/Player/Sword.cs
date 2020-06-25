using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
        {
            other.transform.GetComponent<EnemyFSM1>().HitDamage(10);
            this.transform.gameObject.SetActive(false);
        }
    }

}


