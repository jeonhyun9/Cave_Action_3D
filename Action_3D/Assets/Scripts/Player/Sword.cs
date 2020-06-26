using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject hitPrefab;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(PlayerInput.Instance.state == PlayerInput.PlayerState.ATTACK)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                other.transform.GetComponent<EnemyFSM1>().HitDamage(100);
                GameObject hit = Instantiate(hitPrefab);
                hit.transform.position = other.transform.position;
            }
        }
    }

}


