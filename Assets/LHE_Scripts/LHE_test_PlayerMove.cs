using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_test_PlayerMove : MonoBehaviour
{
    GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.Find("Enemy");
    }

    public float moveSpeed = 10;
    public float rotateSpeed = 10;

    public float currentDistance;
    public float attackDist = 1.5f;

    Quaternion beforeWire;
    Quaternion afterWire;
    Vector3 beforeWireForward;
    // Update is called once per frame
    void Update()
    {
        // [이동]
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        if(!(h == 0 && v == 0))
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
        }


        // [공격]
        //currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
        if (/*(currentDistance < attackDist)*//* &&*/ Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage(1);
        }

        
        // [Wire]
        if (Input.GetButtonDown("Jump"))
        {
            print("wire");

            beforeWire = transform.rotation; // Quaternion
            beforeWireForward = transform.forward; // Vector3

            float angle = Vector3.Angle(beforeWireForward, wirePosition.transform.forward);
            print("wire angle is" + angle);

            afterWire = Quaternion.Euler(0, 180 - angle, 0);



            StartCoroutine("IeWire");
        } 
    }

    float currentTime = 0;
    public GameObject wirePosition;
    public float wireSpeed = 10;
    IEnumerator IeWire()
    {
        while (true)
        {
            if(currentTime < 100)
            { 
                currentTime += Time.deltaTime;

                // [회전]                
                //transform.rotation = Quaternion.Lerp(beforeWire, afterWire, rotateSpeed * Time.deltaTime);
                
                // [이동]
                transform.position = Vector3.Lerp(transform.position, wirePosition.transform.position, wireSpeed * Time.deltaTime);

                yield return null;
            }
            else
            {
                yield break;
            }
        }       
    }
}
