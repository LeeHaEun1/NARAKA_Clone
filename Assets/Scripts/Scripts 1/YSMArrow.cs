using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ī�޶� ���� �������� ���ư��� �ʹ�.
// �ʿ�Ӽ� : ���� , �ӵ� , 
public class YSMArrow : MonoBehaviour
{
    Vector3 dir;
    public float speed = 10;
    //public Transform player;

    void Start()
    {
        //dir = player.transform.forward;
    }

    void Update()
    {
        transform.position += dir * 10 * Time.deltaTime;
    }
}
