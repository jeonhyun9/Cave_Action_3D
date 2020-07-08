using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjustment : MonoBehaviour
{
    public Slider volumeSlider;

    // Update is called once per frame
    void Update()
    {
        MasterVolumeAdjustment();
    }

    private void MasterVolumeAdjustment()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
