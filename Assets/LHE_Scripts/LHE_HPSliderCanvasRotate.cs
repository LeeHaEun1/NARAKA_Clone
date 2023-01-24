using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy�� HP Slider�� �׻� ī�޶󿡼� �������� �ٶ��̵��� ȸ����Ű��ʹ�
public class LHE_HPSliderCanvasRotate : MonoBehaviour
{
    // ĵ���� ȸ�� �ӵ�
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
