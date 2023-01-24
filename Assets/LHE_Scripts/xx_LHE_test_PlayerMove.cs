using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [for AI Enemy Test]
// 이동(adws)
// 공격
// 와이어 -> 벽타기

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

    // Test 용도: 현재 player와의 거리
    public float currentDistance;
    public float attackDist = 1.5f;

    Quaternion beforeWire;
    Vector3 beforeWireForward;
    public GameObject wirePosition;

    public float moveSpeed = 5;
    // Update is called once per frame
    void Update()
    {
        // Test 용도: 현재 enemy와의 거리
        // 공격상태 전환 때 공용으로 쓰기 위해 일단 if 해제..
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

        // [이동 State]
        // adws 클릭 시 이동 상태로 전환
        //if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        //{
        //    state = State.Move;
        //}
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        transform.forward = dir; // 몸체 회전
        transform.position += dir * moveSpeed * Time.deltaTime;

        // [공격 State]
        // comma 클릭 시 공격 상태로 전환
        if ((currentDistance < attackDist) && Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage(1);
        }

        // [Wire State]
        // spacebar 클릭 시 와이어액션 상태로 전환
        if (Input.GetButtonDown("Jump"))
        {
            //spacebar클릭 시의 플레이어의 forward<-> wirePosition의 forward의 각(x) 을 구하고


            state = State.Wire;
        }
    }

    //private void StateIdle()
    //{
    //    // 일단 Update거 가져옴..

    //    // [이동 State]
    //    // adws 클릭 시 이동 상태로 전환
    //    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
    //    {
    //        state = State.Move;
    //    }

    //    // [공격 State]
    //    // comma 클릭 시 공격 상태로 전환
    //    if ((currentDistance < attackDist) && Input.GetKeyDown(KeyCode.Comma))
    //    {
    //        state = State.Attack;
    //    }

    //    // [Wire State]
    //    // spacebar 클릭 시 와이어액션 상태로 전환
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        //spacebar클릭 시의 플레이어의 forward<-> wirePosition의 forward의 각(x) 을 구하고
    //        //beforeWire = transform.rotation; // Quaternion
    //        //beforeWireForward = transform.forward; // Vector3
    //        // ↑ 맨 처음 스페이스바 눌렀을 때의 각도만 저장된다(이동 후 update X)
            
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

    //    transform.forward = dir; // 몸체 회전
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
        // [1] 벽을 정면으로 바라보도록 몸체 회전
        // spacebar클릭 시의 플레이어의 forward <-> wirePosition의 forward의 각(x)을 구하고
        // (180-x)만큼 몸체 회전
        beforeWire = transform.rotation; // Quaternion
        beforeWireForward = transform.forward; // Vector3
        
        float angle = Vector3.Angle(beforeWireForward, wirePosition.transform.forward);
        print("wire angle is" + angle);

        afterWire = Quaternion.Euler(0, 180 - angle, 0);
        transform.rotation = Quaternion.Lerp(beforeWire, afterWire, rotateSpeed * Time.fixedDeltaTime);

        // [2] wirePosition으로 위치 이동
    }
}
