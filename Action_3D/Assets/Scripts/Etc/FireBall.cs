using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 5f;
    private float currTime;
    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > 5f)
        {
            var _explosion = PoolingManager.Instance.GetFireExplosion();
            if (_explosion != null)
            {
                _explosion.transform.position = this.transform.position;
                _explosion.SetActive(true);
            }
            currTime = 0;
            this.gameObject.SetActive(false);
        }
        this.transform.Translate(new Vector3(0,0,1) *speed * Time.deltaTime);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("ENEMY"))
        {
            if(other.transform.GetComponent<EnemyFSM1>().state != EnemyFSM1.EnemyState.DIE
                && other.transform.GetComponent<EnemyFSM1>().state != EnemyFSM1.EnemyState.DIE_FIRE)
            {
                var _explosion = PoolingManager.Instance.GetFireExplosion();
                if (_explosion != null)
                {
                    _explosion.transform.position = this.transform.position;
                    _explosion.SetActive(true);
                }
                currTime = 0;
                this.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("BOSS"))
        {
            var _explosion = PoolingManager.Instance.GetFireExplosion();
            if (_explosion != null)
            {
                _explosion.transform.position = this.transform.position;
                _explosion.SetActive(true);
            }
            currTime = 0;
            this.gameObject.SetActive(false);
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("PLAYER"))
        {
            var _explosion = PoolingManager.Instance.GetFireExplosion();
            if (_explosion != null)
            {
                _explosion.transform.position = this.transform.position;
                _explosion.SetActive(true);
            }
            currTime = 0;
            this.gameObject.SetActive(false);
        }
    }
}
