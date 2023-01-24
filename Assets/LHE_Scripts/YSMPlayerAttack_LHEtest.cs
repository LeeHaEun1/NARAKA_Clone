using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMPlayerAttack_LHEtest : MonoBehaviour
{
    // LHE 0803
    // LHE_EnemyMove에서 사용 위해 public으로 변경(기존: private)
    public RaycastHit hitInpo;

    private float curTime = 0;
    private float wireSmoothness = 5;
    CharacterController cc;
    Animator anim;

    // 필요속성 : 화살공장, 화살 풀, 화살 갯수
    public GameObject arrowFactory; // 화살 공장
    public GameObject wireFactory;

    public AttackState attackState;

    public enum AttackState
    {
        basicAttack,
        ultimate,
        arrow,
        sword,

        wire // LHE 0803
    }

    public static YSMPlayerAttack_LHEtest Instance;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();

        attackState = AttackState.arrow;
        anim = GetComponent<Animator>();

        
    }


    void Update()
    {
        if(attackState == AttackState.basicAttack)
        {

        }
        else if(attackState == AttackState.ultimate)
        {
            UltimateSkill();
        }
        else if(attackState == AttackState.arrow)
        {
            Bow();
        }
        else if(attackState == AttackState.sword)
        {

        }

        Wire();

        Debug.DrawRay(transform.position, transform.forward, Color.red);



        // LHE 0803
        // enemy 피격효과 테스트용으로 추가 -> 거리요소 추가해야함!!
        // 원거리/근거리 공격에 따른 hit여부나, 공격거리 내부에 있는지 여부 체크하는 것 추가 필요
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            //LHE_EnemyHP.Instance.AddDamage(1);

            LHE_EnemyFarHP.Instance.AddDamage(1);
        }
    }
    
    void Wire()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // LHE 0803
            attackState = AttackState.wire;

            Ray ray = new Ray(transform.position, Camera.main.transform.forward);

            // 와이어를 쏘고 만약 맞았다면
            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                // 와이어 생성 공장에서 와이어를 생성해 내 위치에 가져다 놓고 싶다.
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position;
                // 충돌체에서 이동 Wire코루틴 재생
                transform.rotation = Quaternion.LookRotation(hitInpo.transform.position - transform.position);
                //StartCoroutine("IE_Wire");

                // LHE test 0804
                print("hitInpo.normal" + hitInpo.normal);
            }
        }
    }
    IEnumerator IE_Wire()
    {
        YSMPlayerMovement_LHEtest.Instance.canGravity = false; //중력 비활성
        while (true)
        {
            if (curTime < 1)
            {
                curTime += Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * 100);

                // LHE 0803
                // 플레이어 state가 wire에 머물러있으면 enemy상태도 wire로 변하는 현상 발생
                // -> 해결 위해 플레이어가 hitpoint 도달하면 attackstate변경해주기
                float distance = Vector3.Distance(transform.position, hitInpo.point);
                if(distance < 0.1f)
                {
                    transform.position = hitInpo.point;
                    attackState = AttackState.arrow;
                }

                // 와이어 쏜 방향으로 회전하고 싶다.
                // transform.eulerAngles = new Vector3(0, 0, hitInpo.transform.position.z);
                yield return null;
            }
            else
            {
                YSMPlayerMovement_LHEtest.Instance.canGravity = true; // 중력 활성

                curTime = 0;

                yield break;
            }
        }
    }

    void UltimateSkill()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                // 지점까지 이동 코루틴 
                StartCoroutine("IE_UltimateSkill");

            }
        }
    }

    float ultSmoothness = 10;
    IEnumerator IE_UltimateSkill()
    {
        YSMPlayerMovement_LHEtest.Instance.canMove = false;
        while (true)
        {
            if (curTime < 0.5f)
            {
                curTime += Time.deltaTime;

                /*transform.position = Vector3.Lerp(transform.position, transform.position 
                    + transform.forward *20, Time.deltaTime*10);*/

                if(hitInpo.distance < 10)
                {
                    //transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * ultSmoothness);
                    cc.Move(transform.forward * 20 * Time.deltaTime);
                }
                /*else
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 5, Time.deltaTime * ultSmoothness);
                }*/

                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);

                YSMPlayerMovement_LHEtest.Instance.canMove = true;

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
            anim.SetTrigger("Bow");

        }
        // 만약 왼쪽 버튼을 떼면 원래 위치로 돌아가고 싶다
        // 화살이 나가게 하고싶다.
        if (Input.GetButtonUp("Fire1"))
        {
            // 카메라 원래 위치
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            // 화살을 생성해서 내 앞위치에 놓고싶다.
            GameObject arrow = Instantiate(arrowFactory);
            arrow.transform.forward = Camera.main.transform.forward;
            arrow.transform.position = transform.position + new Vector3(0.5f,0,0);


        }
    }
}
