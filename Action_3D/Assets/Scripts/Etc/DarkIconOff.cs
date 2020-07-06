using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkIconOff : MonoBehaviour
{
    public int value;
    // Update is called once per frame
    void Update()
    {
        if(PlayerInput.Instance.fireCapacity > value)
        {
            this.gameObject.SetActive(false);
        }
    }
}
