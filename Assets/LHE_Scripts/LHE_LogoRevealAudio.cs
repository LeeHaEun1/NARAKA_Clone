using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_LogoRevealAudio : MonoBehaviour
{
    AudioSource logoAudio;

    // Start is called before the first frame update
    void Start()
    {
        logoAudio = GetComponent<AudioSource>();    
        logoAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
