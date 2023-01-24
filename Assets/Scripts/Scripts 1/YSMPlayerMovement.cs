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
            // 중력 적용하고 싶다.

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
                    if ((finalDir != Vector3.zero) && attackMoveControll)
                    {
                        transform.rotation = Quaternion.LookRotation(finalDir);
                    }
                }

                // 이동 애니메이션 적용
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
            // 사용자의 입력을 받아
            // 방향을 만들고
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
                // 절벽끝으로 오르면 애니매이션을 변화시키고 싶다.(절벽끝이라는 걸 출력하고싶다.)
                // 1. 앞 방향으로 레이를 쏜다.
                Ray rayUp = new Ray(transform.position + transform.up * 0.5f, transform.forward);
                RaycastHit hitInfoUp;
                // 2. 만약 부딪힌 것이 존재한다면
                if (Physics.Raycast(rayUp, out hitInfoUp, 5))
                {
                    // 부딪히면 벽을 바라보도록 하고싶다.
                    transform.rotation = Quaternion.LookRotation(-hitInfoUp.normal);
                    // 절벽위로 올라가는 bool변수를 false로 해놓는다
                    isClimb = false;
                    // normal상태로 바꾸는 bool 변수를 true 해놓는다.
                    isNormal = true;

                }
                else
                {
                    // 절벽위로 올라가는 bool변수를 true로 해놓는다
                    isClimb = true;
                    // normal상태로 바꾸는 bool 변수를 false 해놓는다.
                    isNormal = false;

                    // 3. 애니메이션을 바꾼다.(출력)
                    //print("절벽끝");
                    // 위로 올라가지 못하게 한다.
                    if (climbdir.y > 0)
                    {
                        climbdir.y = 0;
                    }
                    //print("위");
                    // 만약 스페이스바를 누르면 절벽위로 올라간다.
                    if (Input.GetButtonDown("Jump") && isClimb)
                    {
                        // 4. 절벽에서 평지로 이동하는 코루틴을 실행한다.
                        // 5. Normal상태로 바꾼다.
                        print("IE_ClimbEndUp");
                        StartCoroutine("IE_ClimbEndUp");
                        //StartCoroutine("IE_ClimbEndForward");

                    }
                }

                Ray rayLeft = new Ray(transform.position - transform.right, transform.forward);
                RaycastHit hitInfoLeft;
                // 2. 만약 부딪힌 것이 존재한다면
                if (Physics.Raycast(rayLeft, out hitInfoLeft, 5))
                {

                }
                else
                {
                    // 3. 애니메이션을 바꾼다.(출력)
                    //print("절벽끝");
                    // 위로 올라가지 못하게 한다.
                    if (climbdir.z > 0)
                    {
                        climbdir.z = 0;
                    }
                    // print("dir x : " + dir.x);
                    // print("왼쪽");
                    // 만약 스페이스바를 누르면 절벽위로 올라간다.
                    // 4. 절벽에서 평지로 이동하는 코루틴을 실행한다.
                }

                Ray rayRight = new Ray(transform.position + transform.right, transform.forward);
                RaycastHit hitInfoRight;
                // 2. 만약 부딪힌 것이 존재한다면
                if (Physics.Raycast(rayUp, out hitInfoRight, 5))
                {

                }
                else
                {
                    // 3. 애니메이션을 바꾼다.(출력)
                    //print("절벽끝");
                    // 위로 올라가지 못하게 한다.
                    if (climbdir.z < 0)
                    {
                        climbdir.z = 0;
                    }
                    //print("오른쪽");
                    // 만약 스페이스바를 누르면 절벽위로 올라간다.
                    // 4. 절벽에서 평지로 이동하는 코루틴을 실행한다.
                }

            }

            // 바닥에 닿아 있으면 noraml상태로 전환하고 싶다.
            if (_controller.collisionFlags == CollisionFlags.Below)
            {
                moveState = MoveState.Normal;
            }
            // 그 방향으로 이동하고 싶다.
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
                // normal상태로 전환
                // 코루틴 종료
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
        // 만약 LeftShift를 누르면 대쉬하고 싶다.
        // 필요속성 : 대쉬 스피드, 방향
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
        // 만약 스페이스바를 누르면 점프하고 싶다.
        // -> yVelocity에 jump 파워를 넣고싶다.
        // 점프를 두번 했다면 땅에 내려왔을 때 count값을 초기화 하자.
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
        if (_controller.isGrounded) // 바닥이면 카운트 초기화
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
