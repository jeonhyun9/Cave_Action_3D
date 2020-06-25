using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TailLight : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public float TraceDistance = 0f;
    public float TraceSpeed = 0.5f;
    private NavMeshAgent agent;


    void Start()
    {
        
    }

    void Update()
    {
        if(Vector3.Distance(target.transform.position, this.transform.position) > TraceDistance)
        {
            Vector3 dir = (target.transform.position - this.transform.position).normalized;
            transform.Translate(dir * TraceSpeed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        Vector3 orgPos = transform.position;
        orgPos.y = 1;
        transform.position = orgPos;
    }
}
