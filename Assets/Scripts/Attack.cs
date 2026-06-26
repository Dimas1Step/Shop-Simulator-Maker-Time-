using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audio.Play();
    }
}
