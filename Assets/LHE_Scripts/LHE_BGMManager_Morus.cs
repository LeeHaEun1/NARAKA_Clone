using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_BGMManager_Morus : MonoBehaviour
{
    AudioSource bgAudio;

    // ½Â¸® ¾À (morus-victory)¿¡¸¸ ÇÊ¿ä
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        bgAudio = GetComponent<AudioSource>();
        bgAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
