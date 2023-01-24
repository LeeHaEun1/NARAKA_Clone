using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy의 HP Slider가 항상 카메라에서 정면으로 바라보이도록 회전시키고싶다
public class LHE_HPSliderCanvasRotate : MonoBehaviour
{
    // 캔버스 회전 속도
    public float maxDegressDelta = 300;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q_hp = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        Vector3 hp_angle = Quaternion.RotateTowards(transform.rotation, q_hp, maxDegressDelta).eulerAngles;
        transform.rotation = Quaternion.Euler(0, hp_angle.y, 0);
    }
}
