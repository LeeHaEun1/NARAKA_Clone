using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_CompassRotate : MonoBehaviour
{
    RawImage compassImage;
    GameObject player;

    GameObject compassCam;


    // Start is called before the first frame update
    void Start()
    {
        compassImage = GetComponent<RawImage>();
        player = GameObject.Find("Player");
        compassCam = GameObject.FindGameObjectWithTag("CompassCamera");
    }

    // Update is called once per frame
    void Update()
    {
        //compassImage.uvRect = new Rect(player.transform.localEulerAngles.y / 360, 0, 1, 1);
        compassImage.uvRect = new Rect(compassCam.transform.localEulerAngles.y / 360, 0, 1, 1);
        //Vector3 forward = player.transform.forward;
        Vector3 forward = compassCam.transform.forward;
        forward.y = 0;

        //float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
    }
}
