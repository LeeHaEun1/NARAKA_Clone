using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카메라가 보는 방향으로 나아가고 싶다.
// 필요속성 : 방향 , 속도 , 
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
