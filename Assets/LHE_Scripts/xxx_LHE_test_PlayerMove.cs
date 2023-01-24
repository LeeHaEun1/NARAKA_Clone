using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI Enemy테스트용
// 앞뒤좌우 이동
// 공격
// 와이어

public class xxx_LHE_test_PlayerMove: MonoBehaviour
{
    public float speed = 5;

    public GameObject wirePosition;
    public float wireSpeed = 5;

    float wireAngle;

    public static xxx_LHE_test_PlayerMove Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 이동
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            transform.forward = dir; // 몸체 회전
        }
        transform.position += dir * speed * Time.deltaTime;


        // 공격
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage(1);
        }

        
        // 와이어 액션
        if (Input.GetButtonDown("Jump"))
        {
            wireAngle = Vector3.Angle(transform.forward, wirePosition.transform.forward);
            StartCoroutine("IeWireAction");
            //transform.position = Vector3.Lerp(transform.position, wirePosition.transform.position, wireSpeed * Time.deltaTime);
        }
    }
    float curTime = 0;
    public float rotateSpeed = 10;
    public float maxDegreesDelta = 300;
    IEnumerator IeWireAction()
    {
        while(true)
        {
            if(curTime < 10)
            {
                curTime += Time.deltaTime;

                //Vector3 dir = wirePosition.transform.position - transform.position;
                //transform.forward = dir; // 몸체 회전


                //transform.rotation = Quaternion.Lerp(transform.rotation, wirePosition.transform.rotation, 1);


                //float angle = Vector3.Dot(transform.forward, wirePosition.transform.right);

                //float angle = Quaternion.FromToRotation(Vector3.up, wirePosition.transform.right - transform.forward).eulerAngles.z;

                //print(angle);
                //transform.Rotate(0, angle, 0);

                // 몸체 회전
                //Quaternion q_wire = Quaternion.LookRotation(-wirePosition.transform.position - transform.position);
                //Vector3 wire_angle = Quaternion.RotateTowards(transform.rotation, q_wire, maxDegreesDelta).eulerAngles;
                //transform.rotation = Quaternion.Euler(0, 90-wire_angle.y, 0);
                //transform.rotation = q_wire;


                // 이동
                transform.position = Vector3.Lerp(transform.position, wirePosition.transform.position, wireSpeed * Time.deltaTime);

                //float angle = Vector3.Angle(transform.forward, wirePosition.transform.forward);
                transform.rotation = Quaternion.Euler(0, 180 - wireAngle, 0);
                //print(wireAngle);
                yield return null;
            }
            else
            {
                yield break;
            }
        }

    }
}


