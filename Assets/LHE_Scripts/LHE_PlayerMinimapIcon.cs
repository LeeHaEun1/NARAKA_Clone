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
        // ���� ���� ��ġ��Ű��
        transform.rotation = minimapCam.transform.rotation;
    }
}
