using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] bloodSound;
    public int RandomRange;
    public float volume = 1.0f;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(bloodSound[UnityEngine.Random.Range(0, RandomRange)], volume);
    }
}
