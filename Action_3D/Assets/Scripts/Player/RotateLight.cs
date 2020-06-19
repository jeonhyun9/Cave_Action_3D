using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour
{
    public GameObject rotateTarget;
    public float rotateSpeed;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        orbitAround();
    }

    private void orbitAround()
    {
        transform.RotateAround(rotateTarget.transform.position, Vector3.down, rotateSpeed * Time.deltaTime);
    }
}
