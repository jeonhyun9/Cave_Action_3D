using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OndisableSound : MonoBehaviour
{
    public AudioClip bloodSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        audioSource.PlayOneShot(bloodSound, 1.2f);
    }
}
