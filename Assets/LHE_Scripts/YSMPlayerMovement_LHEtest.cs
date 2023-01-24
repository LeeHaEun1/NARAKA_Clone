using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMPlayerMovement_LHEtest : MonoBehaviour
{
    Animator _animator;
    CharacterController _controller;

    Vector3 dir;

    private float h;
    private float v;
    private float gravity = -9.81f;
    private float yVelocity;

    public bool canGravity = true;
    public bool canMove = true;

    public int jumpCount = 0;
    public float jumpPower = 5;
    public float moveSpeed = 5;
    public float smoothness = 10;

    public static YSMPlayerMovement_LHEtest Instance;
    private void Awake()
    {
        Instance = this;
    }

    public MoveState moveState;

    public enum MoveState
    {
        Normal,
        Climb
    }
 

    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _controller = this.GetComponent<CharacterController>();
        moveState = MoveState.Normal;
    }

    void Update()
    {
        Move();
        Jump();
    }

    private void LateUpdate()
    {
        Dash();
    }

    void Move()
    {
        if (moveState == MoveState.Normal)
        {
            // 중력 적용하고 싶다.
            yVelocity += gravity * Time.deltaTime;

            if (canMove == true)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                dir = new Vector3(h, 0, v);

                // dir 벡터를 카메라를 기준으로 하는 로컬 벡터로 전환한다. 
                dir = Camera.main.transform.TransformDirection(dir);

                // 아래쪽을 향하지 않도록 y축 방향을 0으로 조정한다.
                //dir.y = 0;
                dir.Normalize();

                Vector3 finalDir = dir;
                finalDir.y = 0;

                // 중력 적용
                if (canGravity == true)
                {
                    dir.y = yVelocity;
                }
                else
                {
                    dir.y = 0;
                }

                if (dir.magnitude > 0.5f)
                {
                    // 위에서 만든 방향쪽으로 이동한다.
                   

                    // 이동한 방향을 바라보도록 회전시킨다.
                    if (finalDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(finalDir);
                    }
                }
            }
        }
        else if(moveState == MoveState.Climb)
        {
            // 사용자의 입력을 받아
            // 방향을 만들고
            /*if(Input.GetKeyDown(KeyCode.W))
            {
                float v = Input.GetAxis("Vertical");
                dir = new Vector3(0, 0, v);
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                float h = Input.GetAxis("Horizontal");
                dir = new Vector3(h, 0, 0);
            }*/
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            dir = new Vector3(0, v, -h);
            dir.Normalize();

            print("Climb");
        }

        // 그 방향으로 이동하고 싶다.
        _controller.Move(dir * moveSpeed * Time.deltaTime);

    }
    private float dashSpeed = 100;
    private float curTime = 0;
    void Dash()
    {
        StartCoroutine("IE_Dash");
    }
    IEnumerator IE_Dash()
    {
        // 만약 LeftShift를 누르면 대쉬하고 싶다.
        // 필요속성 : 대쉬 스피드, 방향
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            canMove = false;
            while (true)
            {
                if (curTime < 0.02f)
                {
                    curTime += Time.deltaTime;
                    transform.position += transform.forward * dashSpeed * Time.deltaTime;
                    //transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * dashSpeed);

                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(0.01f);
                    canMove = true;

                    curTime = 0;

                    yield break;
                }
            }
        }
    }

    
    private void Jump()
    {
        // 만약 스페이스바를 누르면 점프하고 싶다.
        // -> yVelocity에 jump 파워를 넣고싶다.
        // 점프를 두번 했다면 땅에 내려왔을 때 count값을 초기화 하자.
        if(Input.GetButtonDown("Jump"))
        {
            if(jumpCount < 1 )
            {
                yVelocity = jumpPower;
                jumpCount++;
                print("Jump");
            }
        }
        if (transform.position.y <= 1.5f)
        {
            jumpCount = 0;
        }
    }


   
}
