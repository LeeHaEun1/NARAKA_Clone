using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LHE_MorusMask : MonoBehaviour
{
    public Camera CMCam;
    public CinemachineVirtualCamera CMvcam2;
    public Transform maskPosition;

    float currentTime;
    public float sceneChangeTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CMCam.transform.position == CMvcam2.transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, maskPosition.position, Time.deltaTime);
        }

        float distance = Vector3.Distance(transform.position, maskPosition.position);
        if(distance < 0.1f)
        {
            currentTime += Time.deltaTime;
            if(currentTime > sceneChangeTime)
            {
                SceneManager.LoadScene("LHE_AfterGameUI_Victory");
            }
        }
    }
}
