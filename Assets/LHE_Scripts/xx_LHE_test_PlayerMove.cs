using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [for AI Enemy Test]
// �̵�(adws)
// ����
// ���̾� -> ��Ÿ��

public class xx_LHE_test_PlayerMove : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Attack,
        Wire,
        //Climb,
        //Die
    }
    public State state;        

    GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;

        enemy = GameObject.Find("Enemy");
    }

    // Test �뵵: ���� player���� �Ÿ�
    public float currentDistance;
    public float attackDist = 1.5f;

    Quaternion beforeWire;
    Vector3 beforeWireForward;
    public GameObject wirePosition;

    public float moveSpeed = 5;
    // Update is called once per frame
    void Update()
    {
        // Test �뵵: ���� enemy���� �Ÿ�
        // ���ݻ��� ��ȯ �� �������� ���� ���� �ϴ� if ����..
        //if (enemy)
        //{
        //    currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
        //}

        // FSM
        switch (state)
        {
            //case State.Idle:
            //    StateIdle();
            //    break;
            //case State.Move:
            //    StateMove();
            //    break;
            ////case State.Dash:
            ////    StateDash();
            ////    break;
            //case State.Attack:
            //    StateAttack();
            //    break;
            //case State.Run:
            //    StateRun();
            //    break;
            //case State.AfterRunIdle:
            //    StateAfterRunIdle();
            //    break;
            //case State.AfterRunAttack:
            //    StateAfterRunAttack();
            //    break;
            case State.Wire:
                StateWire();
                break;
            //case State.Die:
            //    StateDie();
            //    break;
        }

        // [�̵� State]
        // adws Ŭ�� �� �̵� ���·� ��ȯ
        //if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        //{
        //    state = State.Move;
        //}
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        transform.forward = dir; // ��ü ȸ��
        transform.position += dir * moveSpeed * Time.deltaTime;

        // [���� State]
        // comma Ŭ�� �� ���� ���·� ��ȯ
        if ((currentDistance < attackDist) && Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage(1);
        }

        // [Wire State]
        // spacebar Ŭ�� �� ���̾�׼� ���·� ��ȯ
        if (Input.GetButtonDown("Jump"))
        {
            //spacebarŬ�� ���� �÷��̾��� forward<-> wirePosition�� forward�� ��(x) �� ���ϰ�


            state = State.Wire;
        }
    }

    //private void StateIdle()
    //{
    //    // �ϴ� Update�� ������..

    //    // [�̵� State]
    //    // adws Ŭ�� �� �̵� ���·� ��ȯ
    //    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
    //    {
    //        state = State.Move;
    //    }

    //    // [���� State]
    //    // comma Ŭ�� �� ���� ���·� ��ȯ
    //    if ((currentDistance < attackDist) && Input.GetKeyDown(KeyCode.Comma))
    //    {
    //        state = State.Attack;
    //    }

    //    // [Wire State]
    //    // spacebar Ŭ�� �� ���̾�׼� ���·� ��ȯ
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        //spacebarŬ�� ���� �÷��̾��� forward<-> wirePosition�� forward�� ��(x) �� ���ϰ�
    //        //beforeWire = transform.rotation; // Quaternion
    //        //beforeWireForward = transform.forward; // Vector3
    //        // �� �� ó�� �����̽��� ������ ���� ������ ����ȴ�(�̵� �� update X)
            
    //        //float angle = Vector3.Angle(transform.forward, wirePosition.transform.forward);
    //        //print("wire angle is" + angle);

    //        state = State.Wire;
    //    }
    //}

    //public float moveSpeed = 5;
    //private void StateMove()
    //{
    //    float h = Input.GetAxis("Horizontal");
    //    float v = Input.GetAxis("Vertical");

    //    Vector3 dir = new Vector3(h, 0, v);
    //    dir.Normalize();

    //    transform.forward = dir; // ��ü ȸ��
    //    transform.position += dir * moveSpeed * Time.deltaTime;
    //}

    //private void StateAttack()
    //{
    //    LHE_EnemyHP.Instance.AddDamage(1);
    //    state = State.Idle;
    //}

    Quaternion afterWire;    
    public float rotateSpeed;
    private void StateWire()
    {
        // [1] ���� �������� �ٶ󺸵��� ��ü ȸ��
        // spacebarŬ�� ���� �÷��̾��� forward <-> wirePosition�� forward�� ��(x)�� ���ϰ�
        // (180-x)��ŭ ��ü ȸ��
        beforeWire = transform.rotation; // Quaternion
        beforeWireForward = transform.forward; // Vector3
        
        float angle = Vector3.Angle(beforeWireForward, wirePosition.transform.forward);
        print("wire angle is" + angle);

        afterWire = Quaternion.Euler(0, 180 - angle, 0);
        transform.rotation = Quaternion.Lerp(beforeWire, afterWire, rotateSpeed * Time.fixedDeltaTime);

        // [2] wirePosition���� ��ġ �̵�
    }
}
