using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LHE_CanvasManager_Victory : MonoBehaviour
{
    public CanvasGroup victoryGroup;
    public Canvas victory;

    AudioSource victoryAudio;
    public VideoPlayer victoryVideo;

    // Start is called before the first frame update
    void Start()
    {
        victoryGroup.enabled = true;
        victory.enabled = true;
     
        victoryAudio = victory.gameObject.GetComponent<AudioSource>();
        
        //victoryAudio.Play();
        victoryVideo.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
