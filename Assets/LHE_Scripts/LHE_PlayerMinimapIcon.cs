using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_PlayerMinimapIcon : MonoBehaviour
{
    GameObject minimapCam;

    // Start is called before the first frame update
    void Start()
    {
        minimapCam = GameObject.Find("MinimapCamera");    
    }

    // Update is called once per frame
    void Update()
    {
        // 방향 지속 일치시키기
        transform.rotation = minimapCam.transform.rotation;
    }
}
