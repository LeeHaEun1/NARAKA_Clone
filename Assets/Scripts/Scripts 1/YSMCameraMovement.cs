using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMCameraMovement : MonoBehaviour
{
    public Transform objectTofollow; // 따라갈 오브젝트
    public float followSpeed = 20; // 따가라 스피드
    public float slowCamSpeed = 30;
    public float sensitinity = 100; // 마우스 감도
    public float clampAngle = 70;// 제한각도

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public Vector3 offset;

    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10;

    public bool canCmaFollow = true;
    public bool Joom = true;
    public bool slowCam = true;

    public static YSMCameraMovement Instance;
    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        slowCam = true;
    }

    // Update is called once per frame
    void Update()
    {
        rotX += -(Input.GetAxis("Mouse Y")) * sensitinity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitinity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * smoothness);

    }
    private void LateUpdate()
    {
        if (canCmaFollow == true)
        {
            //카메라 느리게
            if (slowCam == true)
            {
                transform.position = Vector3.Lerp(transform.position, objectTofollow.position, followSpeed * Time.deltaTime);
            }
            else if (slowCam == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, slowCamSpeed * Time.deltaTime);
            }

            finalDir = transform.TransformPoint(dirNormalized * maxDistance);

            int layer = 1 << LayerMask.NameToLayer("Player");
            int sword = 1 << LayerMask.NameToLayer("Sword");
            int layerMask = layer + sword;

            RaycastHit hit;

            if (Physics.Linecast(transform.position, finalDir, out hit, ~layerMask))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }
            realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, (dirNormalized + offset) * finalDistance, Time.deltaTime * smoothness);
        }
    }
}
