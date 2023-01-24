using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMPlayerMovement : MonoBehaviour
{
    private float dashSpeed = 100;
    private float curTime = 0;

    Animator anim;
    CharacterController _controller;

    public Vector3 dir;
    public Vector3 finalDir;
    public Vector3 climbdir;
    public GameObject dashGage;

    private float h;
    private float v;
    private float gravity = -9.81f;
    public float yVelocity;

    private bool isJump = true;

    public bool canGravity = true;
    public bool canMove = true;

    private bool isClimb = true;
    private bool isNormal = true;


    public int jumpCount = 0;
    public float jumpPower = 5;
    public float moveSpeed = 7;
    public float smoothness = 10;


    public static YSMPlayerMovement Instance;
    private void Awake()
    {
        Instance = this;
    }

    public MoveState moveState;

    public enum MoveState
    {
        Normal,
        Climb,
        ClimbEnd
    }

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        _controller = this.GetComponent<CharacterController>();
        moveState = MoveState.Normal;

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    void Update()
    {
        Move();
        Jump();
        Debug.DrawRay(transform.position + transform.up * 0.5f, transform.forward, Color.red);
        Debug.DrawRay(transform.position - transform.right, transform.forward, Color.green);
        Debug.DrawRay(transform.position + transform.right, transform.forward, Color.blue);
    }

    private void LateUpdate()
    {
        Dash();
    }

    public bool attackMoveControll = true;
    int result;

    void Move()
    {

        if (_controller.collisionFlags == CollisionFlags.Below)
        {
            moveState = MoveState.Normal;

        }
        else
        {

        }

        if (canGravity == true)
        {
            yVelocity += gravity * Time.deltaTime;
        }

        if (moveState == MoveState.Normal)
        {

            /*if (canGravity == true)
            {
                yVelocity += gravity * Time.deltaTime;
            }*/
            // �߷� �����ϰ� �ʹ�.

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
                anim.SetFloat("Move", dir.magnitude);

                anim.SetFloat("SwordMove", dir.magnitude);

                finalDir = dir;
                finalDir.y = 0;
                dir.y = 0;

                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hitInfo;
                int layer = 1 << gameObject.layer;
                if (Physics.Raycast(ray, out hitInfo, 2f, ~layer) == false)
                {
                    anim.SetBool("IsInAir", true);
                }

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
                    if ((finalDir != Vector3.zero) && attackMoveControll)
                    {
                        transform.rotation = Quaternion.LookRotation(finalDir);
                    }
                }

                // �̵� �ִϸ��̼� ����
                /*if (_controller.collisionFlags == CollisionFlags.Sides && )
                {
                    moveState = MoveState.Climb;
                }*/

            }
            print("normal");
            _controller.Move(dir * moveSpeed * Time.deltaTime);


        }
        else if (moveState == MoveState.Climb)
        {
            dir = new Vector3(0,0,0);
            print("dir.magnitude111 : " + dir.magnitude);
            print("climb");
            //print("climb");
            // ������� �Է��� �޾�
            // ������ �����
            /*if (Input.GetKeyDown(KeyCode.W))
            {
                float v = Input.GetAxis("Vertical");
                dir = new Vector3(0, 0, v);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                float h = Input.GetAxis("Horizontal");
                dir = new Vector3(h, 0, 0);
            }*/
            jumpCount = -1;
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            climbdir = new Vector3(h, v, 0);
            climbdir = transform.TransformDirection(climbdir);
            climbdir.Normalize();
            print("dir.magnitude : " + dir.magnitude);

            if(Input.GetKey(KeyCode.W))
            {
                result = 3;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                result = 2;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                result = 4;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                result = 1;
            }
            else
            {
                result = 0;
            }
            print("result : " + result);
            //anim.SetInteger("intClimbMove", result);
            anim.SetFloat("ClimbMove", result);
            //anim.SetFloat("ClimbMove", dir.magnitude);

            if (Input.GetButtonDown("Jump") && isNormal)
            {
                moveState = MoveState.Normal;
                print("ClimbToNormal");

            }
            else
            {
                // ���������� ������ �ִϸ��̼��� ��ȭ��Ű�� �ʹ�.(�������̶�� �� ����ϰ�ʹ�.)
                // 1. �� �������� ���̸� ���.
                Ray rayUp = new Ray(transform.position + transform.up * 0.5f, transform.forward);
                RaycastHit hitInfoUp;
                // 2. ���� �ε��� ���� �����Ѵٸ�
                if (Physics.Raycast(rayUp, out hitInfoUp, 5))
                {
                    // �ε����� ���� �ٶ󺸵��� �ϰ�ʹ�.
                    transform.rotation = Quaternion.LookRotation(-hitInfoUp.normal);
                    // �������� �ö󰡴� bool������ false�� �س��´�
                    isClimb = false;
                    // normal���·� �ٲٴ� bool ������ true �س��´�.
                    isNormal = true;

                }
                else
                {
                    // �������� �ö󰡴� bool������ true�� �س��´�
                    isClimb = true;
                    // normal���·� �ٲٴ� bool ������ false �س��´�.
                    isNormal = false;

                    // 3. �ִϸ��̼��� �ٲ۴�.(���)
                    //print("������");
                    // ���� �ö��� ���ϰ� �Ѵ�.
                    if (climbdir.y > 0)
                    {
                        climbdir.y = 0;
                    }
                    //print("��");
                    // ���� �����̽��ٸ� ������ �������� �ö󰣴�.
                    if (Input.GetButtonDown("Jump") && isClimb)
                    {
                        // 4. �������� ������ �̵��ϴ� �ڷ�ƾ�� �����Ѵ�.
                        // 5. Normal���·� �ٲ۴�.
                        print("IE_ClimbEndUp");
                        StartCoroutine("IE_ClimbEndUp");
                        //StartCoroutine("IE_ClimbEndForward");

                    }
                }

                Ray rayLeft = new Ray(transform.position - transform.right, transform.forward);
                RaycastHit hitInfoLeft;
                // 2. ���� �ε��� ���� �����Ѵٸ�
                if (Physics.Raycast(rayLeft, out hitInfoLeft, 5))
                {

                }
                else
                {
                    // 3. �ִϸ��̼��� �ٲ۴�.(���)
                    //print("������");
                    // ���� �ö��� ���ϰ� �Ѵ�.
                    if (climbdir.z > 0)
                    {
                        climbdir.z = 0;
                    }
                    // print("dir x : " + dir.x);
                    // print("����");
                    // ���� �����̽��ٸ� ������ �������� �ö󰣴�.
                    // 4. �������� ������ �̵��ϴ� �ڷ�ƾ�� �����Ѵ�.
                }

                Ray rayRight = new Ray(transform.position + transform.right, transform.forward);
                RaycastHit hitInfoRight;
                // 2. ���� �ε��� ���� �����Ѵٸ�
                if (Physics.Raycast(rayUp, out hitInfoRight, 5))
                {

                }
                else
                {
                    // 3. �ִϸ��̼��� �ٲ۴�.(���)
                    //print("������");
                    // ���� �ö��� ���ϰ� �Ѵ�.
                    if (climbdir.z < 0)
                    {
                        climbdir.z = 0;
                    }
                    //print("������");
                    // ���� �����̽��ٸ� ������ �������� �ö󰣴�.
                    // 4. �������� ������ �̵��ϴ� �ڷ�ƾ�� �����Ѵ�.
                }

            }

            // �ٴڿ� ��� ������ noraml���·� ��ȯ�ϰ� �ʹ�.
            if (_controller.collisionFlags == CollisionFlags.Below)
            {
                moveState = MoveState.Normal;
            }
            // �� �������� �̵��ϰ� �ʹ�.
            _controller.Move(climbdir * moveSpeed * Time.deltaTime);
        }

    }

    IEnumerator IE_ClimbEndUp()
    {
        print("Back");
        canGravity = false;
        isJump = false;

        //transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward, Time.deltaTime);
        transform.position = transform.position - transform.forward * 0.5f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.up * 2;
        while (true)
        {
            if (curTime < 0.15f)
            {

                curTime += Time.deltaTime;

                //transform.position = Vector3.Lerp(transform.position, transform.position + transform.up * 2, Time.deltaTime * 20);

                transform.position += transform.up * 30 * Time.deltaTime;

                yield return null;
            }
            else
            {
                curTime = 0;

                StartCoroutine("IE_ClimbEndForward");

                yield break;
            }
        }

    }
    IEnumerator IE_ClimbEndForward()
    {
        canGravity = false;
        //jumpCount = 3;

        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward;
        while (true)
        {
            if (curTime < 0.1f)
            {
                print("forword");
                curTime += Time.deltaTime;

                // transform.position = Vector3.Lerp(startPos, endPos, Time.deltaTime * 20);
                transform.position += transform.forward * 20 * Time.deltaTime;
                yield return null;
            }
            else
            {
                canGravity = true;
                curTime = 0;
                moveState = MoveState.Normal;
                //jumpCount = 0;
                isJump = true;
                // normal���·� ��ȯ
                // �ڷ�ƾ ����
                yield break;
            }
        }
    }
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (YSMDashGage.Instance.cooldownTimer >= 1)
            {
                StartCoroutine("IE_Dash");
                anim.SetTrigger("Dash");
            }
        }
    }

    IEnumerator IE_Dash()
    {
        // ���� LeftShift�� ������ �뽬�ϰ� �ʹ�.
        // �ʿ�Ӽ� : �뽬 ���ǵ�, ����
        YSMDashGage.Instance.UseSpell();
        YSMDashGage.Instance.cooldownTimer--;

        canMove = false;
        while (true)
        {
            if (curTime < 0.02f)
            {
                curTime += Time.deltaTime;
                _controller.Move(transform.forward * dashSpeed * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * 10 * dashSpeed);

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

    public int animCount = 0;
    public void Jump()
    {
        // ���� �����̽��ٸ� ������ �����ϰ� �ʹ�.
        // -> yVelocity�� jump �Ŀ��� �ְ�ʹ�.
        // ������ �ι� �ߴٸ� ���� �������� �� count���� �ʱ�ȭ ����.
        if (Input.GetButtonDown("Jump") && isJump)
        {
           // moveState = MoveState.Normal;
            //anim.SetTrigger("Jump22");
            print("JumpCount : " + jumpCount);
            
            anim.SetFloat("ClimbMove", 0);

            if (jumpCount < 1)
            {
                jumpCount++;
                animCount++;

                print("JumpCount : " + jumpCount);
                yVelocity = jumpPower;
            }
            if (/*jumpCount >= 1 ||*/ animCount == 2)
            {
                print("11111111111111");
                //anim.SetInteger("Jump2", animCount);
                anim.SetTrigger("Jump22");
                animCount = 0;

            }
            print("JumpCount : " + jumpCount);
           // print("animCount : " + animCount);

        }
        if (_controller.isGrounded) // �ٴ��̸� ī��Ʈ �ʱ�ȭ
        {
            jumpCount = 0;
            anim.SetBool("IsInAir", false);
        }

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name.Contains("Wall"))
        {
            dir = new Vector3(0, 0, 0);
            anim.SetFloat("Move", dir.magnitude);
            anim.SetFloat("SwordMove", dir.magnitude);

            moveState = MoveState.Climb;
            
        }
    }
}
