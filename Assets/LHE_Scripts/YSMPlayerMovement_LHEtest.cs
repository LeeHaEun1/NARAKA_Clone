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
            // �߷� �����ϰ� �ʹ�.
            yVelocity += gravity * Time.deltaTime;

            if (canMove == true)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                dir = new Vector3(h, 0, v);

                // dir ���͸� ī�޶� �������� �ϴ� ���� ���ͷ� ��ȯ�Ѵ�. 
                dir = Camera.main.transform.TransformDirection(dir);

                // �Ʒ����� ������ �ʵ��� y�� ������ 0���� �����Ѵ�.
                //dir.y = 0;
                dir.Normalize();

                Vector3 finalDir = dir;
                finalDir.y = 0;

                // �߷� ����
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
                    // ������ ���� ���������� �̵��Ѵ�.
                   

                    // �̵��� ������ �ٶ󺸵��� ȸ����Ų��.
                    if (finalDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(finalDir);
                    }
                }
            }
        }
        else if(moveState == MoveState.Climb)
        {
            // ������� �Է��� �޾�
            // ������ �����
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

        // �� �������� �̵��ϰ� �ʹ�.
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
        // ���� LeftShift�� ������ �뽬�ϰ� �ʹ�.
        // �ʿ�Ӽ� : �뽬 ���ǵ�, ����
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
        // ���� �����̽��ٸ� ������ �����ϰ� �ʹ�.
        // -> yVelocity�� jump �Ŀ��� �ְ�ʹ�.
        // ������ �ι� �ߴٸ� ���� �������� �� count���� �ʱ�ȭ ����.
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
