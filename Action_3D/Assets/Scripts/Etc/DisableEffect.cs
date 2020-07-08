using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEffect : MonoBehaviour
{
    private float currTime = 0;
    public float disableTime = 3;

    private void OnEnable()
    {
        currTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if(currTime > disableTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
