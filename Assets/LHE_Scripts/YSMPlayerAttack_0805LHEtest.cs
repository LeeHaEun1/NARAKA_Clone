using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class YSMPlayerAttack_0805LHEtest : MonoBehaviour
{
    public RaycastHit hitInpo;
    public Vector3 offset;

    private float curTime = 0;
    private float wireSmoothness = 5;

    private bool canWire = false;
    private bool firstQ = true;
    private bool secondQ = true;

    CharacterController cc;
    Animator anim;

    // 필요속성 : 화살공장, 화살 풀, 화살 갯수
    public GameObject arrowFactory; // 화살 공장
    public GameObject wireFactory;

    //public Transform bowFirePos;
    public Transform arrowImpact;
    ParticleSystem arrowPS;

    public GameObject bowAim;
    public GameObject basicAim;

    public AttackState attackState;

    public enum AttackState
    {
        basicAttack,
        ultimate,
        bow,
        sword,

        wire // [LHE 0803] AI enemy들의 player 상태 인지 위해 추가
    }

    public static YSMPlayerAttack_0805LHEtest Instance;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();

        attackState = AttackState.basicAttack;
        anim = GetComponent<Animator>();

        arrowPS = arrowImpact.GetComponent<ParticleSystem>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            attackState = AttackState.basicAttack;
            print("basicAttack");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            attackState = AttackState.bow;
            print("arrow");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            attackState = AttackState.sword;
            print("sword");
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            attackState = AttackState.ultimate;
            print("ultimate");
        }

        switch (attackState)
        {
            case AttackState.basicAttack:

                break;

            case AttackState.bow:
                Bow();

                break;

            case AttackState.sword:

                break;

            case AttackState.ultimate:
                UltimateSkill();
                break;
        }

        Wire();

        //Debug.DrawRay(transform.position, transform.forward, Color.red);



        // [LHE 0803]
        // enemy 피격효과 테스트용으로 추가 -> 거리요소 추가해야함!!
        // 원거리/근거리 공격에 따른 hit여부나, 공격거리 내부에 있는지 여부 체크하는 것 추가 필요
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage3(1);

            //LHE_EnemyFarHP.Instance.AddDamage(1);
        }
    }

    Vector3 currentPos;
    void Wire()
    {
        // 와이어 q클릭시 카메라 확대 축소
        if (Input.GetKeyDown(KeyCode.Q) && firstQ == true)
        {
            print("JoomIn");
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            canWire = true;
            firstQ = false;
            secondQ = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && secondQ == false)
        {
            print("JoomOut");

            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;
            canWire = false;
            secondQ = true;
            firstQ = true;
        }

        if (Input.GetButtonDown("Fire1") && canWire == true)
        {
            // [LHE 0803]
            // AI enemy들의 player 상태 인지 위해 추가
            attackState = AttackState.wire;


            currentPos = transform.position;
            transform.rotation = Quaternion.LookRotation(-hitInpo.normal);

            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;
            //StartCoroutine("IE_CameraMoveDelay");

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 와이어를 쏘고 만약 맞았다면
            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                //공중에 있을때
                if (currentPos.y >= 1)
                {
                    transform.position = currentPos;
                    // 그상태를 유지하고 중력을 비활성하고싶다.
                    YSMPlayerMovement.Instance.canGravity = false; //중력 비활성
                }
                else // 땅에 있을때
                {

                }
                // 와이어 생성 공장에서 와이어를 생성해 내 위치에 가져다 놓고 싶다.
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position + offset;
                // 충돌체에서 이동 Wire코루틴 재생
                transform.rotation = Quaternion.LookRotation(hitInpo.transform.position - transform.position);
                StartCoroutine("IE_WireRot");

                // [LHE 0806]
                print("player startcoroutine IE_WireRot");

                //YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 0.5f;

            }
        }


    }

    IEnumerator IE_CameraMoveDelay()
    {
        YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

        yield return new WaitForSeconds(1f);

        YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 0.5f;

        /*while (true)
        {
            if(curTime < 1f)
            {
                curTime += Time.deltaTime;

                YSMCameraMovement.Instance.maxDistance += Time.deltaTime;

                yield return null;
            }
            else
            {
                curTime = 0;

                yield break;
            }
        }*/

    }
    IEnumerator IE_Wire()
    {
        while (true)
        {
            YSMPlayerMovement.Instance.canGravity = false; //중력 비활성


            // [LHE 0806]
            // 플레이어 state가 wire에 머물러있으면 enemy상태도 지속해서 wire로 변하는 현상 발생
            // -> 해결 위해 wire trigger되면 attackstate 바로 변경해주기
            attackState = AttackState.basicAttack;


            if (curTime < 1)
            {
                curTime += Time.deltaTime;

                //transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * 10);
                cc.Move((hitInpo.point - transform.position) * 5 * Time.deltaTime);

                // *** 0806 여기 쓰면 벽에 붙어도 상태변경 안됨..
                // [LHE 0803]
                // 플레이어 state가 wire에 머물러있으면 enemy상태도 지속해서 wire로 변하는 현상 발생
                // -> 해결 위해 플레이어가 hitpoint 도달하면 attackstate변경해주기
                //float distance = Vector3.Distance(transform.position, hitInpo.point);
                //if (distance < 0.1f)
                //{
                //    transform.position = hitInpo.point;
                //    attackState = AttackState.basicAttack; // [LHE 0805] arrow -> basicAttack
                //}


                // 와이어 쏜 방향으로 회전하고 싶다.
                yield return null;
            }
            else
            {
                YSMPlayerMovement.Instance.canGravity = true; // 중력 활성

                curTime = 0;

                firstQ = true;

                yield break;
            }
        }
    }

    IEnumerator IE_WireRot()
    {
        while (true)
        {
            if (curTime < 0.5f)
            {
                curTime += Time.deltaTime;

                transform.Rotate(new Vector3(10f, 20f, 30f) * Time.deltaTime * 20);

                yield return null;
            }
            else
            {
                transform.up = new Vector3(0, 1, 0);
                transform.rotation = Quaternion.LookRotation(-hitInpo.normal);

                curTime = 0;

                yield break;
            }
        }

    }

    void UltimateSkill()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            YSMCameraMovement.Instance.slowCam = false;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                // 지점까지 이동 코루틴 
                YSMPlayerMovement.Instance.canGravity = false;
                StartCoroutine("IE_UltimateSkill");

            }
            else
            {
                YSMPlayerMovement.Instance.canGravity = false;

                StartCoroutine("IE_UltimateSkill");
            }
        }
    }

    float ultSmoothness = 10;
    IEnumerator IE_UltimateSkill()
    {
        YSMPlayerMovement.Instance.canMove = false;
        while (true)
        {
            if (curTime < 0.5f)
            {
                curTime += Time.deltaTime;

                /*transform.position = Vector3.Lerp(transform.position, transform.position 
                    + transform.forward *20, Time.deltaTime*10);*/

                //transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * ultSmoothness);
                cc.Move(Camera.main.transform.forward * 50 * Time.deltaTime);
                /*else
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 5, Time.deltaTime * ultSmoothness);
                }*/
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);

                YSMPlayerMovement.Instance.canMove = true;
                YSMPlayerMovement.Instance.canGravity = true;
                //내려올 때 slowCam = false 여야함 (수정필요)
                YSMCameraMovement.Instance.slowCam = true;

                curTime = 0;

                yield break;
            }
        }
    }

    void Bow()
    {
        // 만약 왼쪽 버튼을 누르면 화살을 날리고 싶다.
        // -> 
        // 왼쪽 버튼을 누르면 카메라를 앞으로 이동시키고 싶다.

        if (Input.GetButtonDown("Fire1"))
        {
            // 카메라 앞으로 이동시키기
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            bowAim.SetActive(true);
            basicAim.SetActive(false);

            //anim.SetTrigger("Bow");
        }
        if (Input.GetButton("Fire1"))
        {
            transform.forward = Camera.main.transform.forward;
        }
        // 만약 왼쪽 버튼을 떼면 원래 위치로 돌아가고 싶다
        // 화살이 나가게 하고싶다.
        if (Input.GetButtonUp("Fire1"))
        {
            bowAim.SetActive(false);
            basicAim.SetActive(true);
            // 카메라 원래 위치
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            /* // 화살을 생성해서 내 앞위치에 놓고싶다.
             GameObject arrow = Instantiate(arrowFactory);
             arrow.transform.position = transform.position;*/

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                arrowImpact.position = hitInfo.point;
                // 파편이 튀는 방향을 부딪힌 지점이 향하는 방향과 일치시켜주자.
                arrowImpact.forward = hitInfo.normal;
                arrowPS.Stop();
                arrowPS.Play();
            }
        }
    }
}
