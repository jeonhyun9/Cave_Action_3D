using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TailLight : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public GameObject fireParticle;
    public GameObject sword;
    public float TraceDistance = 1f;
    public float TraceSpeed;
    public float rotateSpeed;
    private float nonCastingTraceSpeed = 5f;
    private float castingTraceSpeed = 1.5f;
    private bool isCasting = false;
    private float currTime;
    private NavMeshAgent agent;
 
    void Update()
    {
        if(PlayerInput.Instance.state == PlayerInput.PlayerState.START_CASTING)
        {
            if (PlayerInput.Instance.isEnchanted == false)
            {
                isCasting = true;
                currTime += Time.deltaTime;
                Vector3 dir = (sword.transform.position - this.transform.position);
                transform.Translate(dir * TraceSpeed * Time.deltaTime);
            }
            else fireParticle.SetActive(false);
        }

        if (PlayerInput.Instance.state != PlayerInput.PlayerState.START_CASTING)
        {
            if (fireParticle.activeSelf == false && PlayerInput.Instance.isEnchanted==false)
            {
                isCasting = false;
                fireParticle.SetActive(true);
            }
            transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.1f);
        }
    }

    private void LateUpdate()
    {
        Vector3 tempDir = Vector3.zero;
        tempDir = transform.position;
        tempDir.y = 1;
        transform.position = tempDir;

        if(!isCasting)
        {
            TraceSpeed = nonCastingTraceSpeed;
        }
        else
        {
            TraceSpeed = castingTraceSpeed;
        }
    }
}
