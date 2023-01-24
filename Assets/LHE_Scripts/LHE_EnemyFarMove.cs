using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [AI Enemy(2)] : 원거리 공격 enemy
// 1. FSM
// 이동 시 항상 이동방향과 에너미 앞 방향 일치하도록 몸체 회전
// *** 단순 거리가 아니라 스크린 상의 좌표로 하는 게 나으려나..?
// Idle
// 앞뒤좌우 -> 이동방향으로 몸체 회전
// 플레이어가 일정거리 안에 들어오면 자동으로 aim맞춰 공격(몸체도 그만큼 회전)
// 대쉬
// 도망..(체력이 일정 수치 이하가 되면 -> 플레이어 체력도 고려(나와 비교))
// (점프)
// (와이어 액션)
// (구르기)

// 적에서 중요한 것은 "피격감"
// -> 플레이어의 무기별로 다르게 구성? damage 별로??

// NavMeshAgent

// 상태 별 Animation

public class LHE_EnemyFarMove : MonoBehaviour
{
    // Singleton
    public static LHE_EnemyFarMove Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // FSM
    public enum State
    {
        Idle,
        Move,
        Dash,
        Attack,
        Run,
        AfterRunIdle,
        AfterRunMove, //ck
        AfterRunAttack,
        Wire,
        Climb,
        ClimbEnd,
        ClimbEndWalk, //ck
        AfterClimbIdle, //ck
        Attacked,
        Attacked2,
        Attacked3,
        Die
    }
    public State state;

    CharacterController cc;
    //NavMeshAgent agent;
    public Animator anim;

    // [Effect]
    AudioSource arrowSound;

    Vector3 originPosition;
    GameObject player;
    bool playerInSight; // 시야각 체크
    bool canAttack; // 공격 가능 여부
    bool isDead; // 사망 여부

    // Start is called before the first frame update
    void Start()
    {
        // Character Controller
        cc = GetComponent<CharacterController>();
        // Nav Mesh Agent
        //agent = GetComponent<NavMeshAgent>();
        // Animator
        anim = GetComponentInChildren<Animator>();

        // [effect]
        arrowSound = GetComponent<AudioSource>();

        // 초기 위치
        originPosition = transform.position;
        // 초기 상태
        state = State.Idle;

        // 플레이어 인지, 플레이어 HP
        player = GameObject.Find("Player");

        // 초기 시야각 = false
        playerInSight = false;
        // 초기 공격 가능 여부 = true;
        canAttack = true;
        // 초기 사망 여부
        isDead = false;
    }

    // 공용: Attacked, StateAttack
    public float currentTime = 0;
    // Test 용도: 현재 player와의 거리
    public float currentDistance;
    // 플레이어가 RAY 쏜 WALL의 높이정보
    float rayWallHeight;
    // Update is called once per frame
    void Update()
    {
        // Test 용도: 현재 player와의 거리
        if (player)
        {
            currentDistance = Vector3.Distance(transform.position, player.transform.position);
        }

        print("far enemy state : " + state);
        // FSM
        switch (state)
        {
            case State.Idle:
                StateIdle();
                break;
            case State.Move:
                StateMove();
                break;
            case State.Dash:
                StateDash();
                break;
            case State.Attack:
                StateAttack();
                break;
            case State.Run:
                StateRun();
                break;
            case State.AfterRunIdle:
                StateAfterRunIdle();
                break;
            case State.AfterRunMove:
                StateAfterRunMove(); //ck
                break;
            case State.AfterRunAttack:
                StateAfterRunAttack();
                break;
            case State.Wire:
                StateWire();
                break;
            case State.Climb:
                StateClimb();
                break;
            case State.ClimbEndWalk:
                StateClimbEndWalk(); //ck
                break;
            case State.AfterClimbIdle:
                StateAfterClimbIdle(); //ck
                break;
            case State.Attacked:
                StateAttacked();
                break;
            case State.Attacked2:
                StateAttacked2();
                break;
            case State.Attacked3:
                StateAttacked3();
                break;
            case State.Die:
                StateDie();
                break;
        }


        // [Field Of View]
        // 좌우 도합 180도
        playerInSight = CheckPlayerAngle(player.transform.position);
        // Draw: 길이 20의 좌우 시야각 한계 표시
        //Debug.DrawRay(transform.position, 20 * new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);


        // [Wire]
        // 어떤 상태이든 Player가 Wire 이동하면 근처 벽으로 따라서 Wire Action
        // *** Run과 afterRun 상태들은 제외할까..??
        if (YSMPlayerAttack.Instance.enemyCanWire == true && YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag == "Wall")
        {
            rayWallHeight = YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.transform.position.y; // 부모의 y
            state = State.Wire;
        }


        // [Die]
        // 어떤 상태이든 체력 0되면 Die
        if (state != State.Die && LHE_EnemyFarHP.Instance.HP <= 0)
        {
            //isDead = true;
            //if(isDead == true)
            //{
            //    anim.enabled = false;
            //    anim.enabled = true;
            //    anim.SetTrigger("Die");
            //    isDead = false;
            //}
            StopAllCoroutines();
            //anim.enabled = false;
            //anim.enabled = true;
            state = State.Die;
            anim.SetTrigger("Die");
            //anim.Play("Die",-1,0);
        }
    }


    [Header("[ Field of View Settings ]")]
    public float sightDegree = 90;
    // [Field Of View]
    private bool CheckPlayerAngle(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        float checkDegree = Vector3.Angle(transform.forward, dir);
        //print(checkDegree); // Test
        if (checkDegree <= sightDegree)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // [피격]
    // 어떤 상태이든 Player가 공격하면 뒤로 약간 밀려나는 피격 효과
    // *** 나의 LHE_EnemyHP의 AddDamage가 호출되는 순간으로 수정할 수는 없나??
    // *** 공격 당하는 동안도 거리 때문에 Attack 상태라서 밀려난 다음 다시 Player방향으로 돌아온다
    //     -> 체력이 3이상 차이나게 되면 안 돌아옴
    [Header("[ Attacked Settings ]")]
    public float attackedSpeed = 5;
    public float attackedTime = 5;
    public float attackedDist = 5;
    // [피격1] 밀려났다가 되돌아오기
    private void StateAttacked()
    {
        // ************dir = enemy위치- player위치 구하고 dir.y=0 한 방향으로 수정 // 0810
        // [피격 방향 구하기]
        Vector3 attackedDir = transform.position - player.transform.position;
        attackedDir.Normalize();
        attackedDir.y = 0;

        //Vector3 attackedPosition = transform.position - transform.forward * attackedDist;
        // 0809 피격 방향 수정
        //Vector3 attackedPosition = transform.position + player.transform.forward * attackedDist;
        Vector3 attackedPosition = transform.position + attackedDir * attackedDist;

        // 2초 동안 Lerp??
        if (currentTime < attackedTime)
        {
            canAttack = false;
            currentTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(transform.position, attackedPosition, attackedSpeed * Time.deltaTime);
            //anim.SetTrigger("Attacked1");
            //print("attacked1 anim played");
        }
        else
        {
            canAttack = true;
            if (LHE_EnemyFarHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
            {
                state = State.AfterRunAttack;
            }
            else
            {
                state = State.Attack;
            }
            currentTime = 0;
        }
    }

    // [피격2] 몸 50도 틀어졌다가 다시 플레이어 바라보기
    public float attacked2RotSpeed = 5;
    private void StateAttacked2()
    {
        //Quaternion currRot = transform.rotation;
        if (currentTime < attackedTime * 2)
        {
            //anim.SetTrigger("Attacked2");
            canAttack = false;
            currentTime += Time.fixedDeltaTime;
            transform.rotation = Quaternion.Lerp(LHE_EnemyFarHP.Instance.CurrRot, LHE_EnemyFarHP.Instance.CurrRot * Quaternion.Euler(0, 50, 0), currentTime);
        }
        else
        {
            canAttack = true;
            if (LHE_EnemyFarHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
            {
                state = State.AfterRunAttack;
            }
            else
            {
                state = State.Attack;
            }
            currentTime = 0;
        }
    }

    // [피격3] 정점과 끝점 찍는 식으로 뒤로 날라가기
    Vector3 CurrPos;
    Vector3 HighPos;
    Vector3 EndPos;
    private void StateAttacked3()
    {
        // ************dir = enemy위치- player위치 구하고 dir.y=0 한 방향으로 수정 // 0810
        // [피격 방향 구하기]
        Vector3 attackedDir = transform.position - player.transform.position;
        attackedDir.Normalize();
        attackedDir.y = 0;

        CurrPos = LHE_EnemyFarHP.Instance.CurrPos;
        HighPos = CurrPos + attackedDir * 2 + Vector3.up * 2;
        EndPos = CurrPos + attackedDir * 4;

        if (transform.position == CurrPos)
        {
            anim.SetTrigger("Attacked3Up");
            StartCoroutine("IeFly");
        }
    }

    IEnumerator IeFly() // 피격3
    {
        while (true)
        {
            if (currentTime < 0.5f)
            {
                currentTime += Time.deltaTime;
                //anim.SetTrigger("Attacked3Up");
                transform.position = Vector3.Lerp(transform.position, HighPos, currentTime * 2);
                yield return null;
            }
            else if (currentTime < 1)
            {
                currentTime += Time.deltaTime;
                anim.SetTrigger("Attacked3Down");
                transform.position = Vector3.Lerp(HighPos, EndPos, currentTime);
                //yield return null;
            }
            else
            {
                if (LHE_EnemyFarHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
                {
                    state = State.AfterRunAttack;
                }
                else
                {
                    state = State.Attack;
                }
                currentTime = 0;
                yield break;
            }
        }
    }

    // 위에서 쓰는 변수랑 다른 변수!!
    //public float attacked3Value = 1;
    // 피격3 베지어
    Vector3 Bezier(Vector3 CurrPos, Vector3 HighPos, Vector3 EndPos)
    {
        Vector3 p1 = Vector3.Lerp(CurrPos, HighPos, 1);
        Vector3 p2 = Vector3.Lerp(HighPos, EndPos, 1);
        return Vector3.Lerp(p1, p2, 0.5f);
    }



    [Header("[ Idle Settings ]")]
    public float idleSpeed = 1;
    int current = 0; // patrolPos 배열 indexw
    [Header("[ Move Settings ]")]
    public float startMoveDist = 25;
    private void StateIdle()
    {
        // [Patrol]
        // Idle 상태 진입시의 첫번째 위치를 기준으로 사방에 정점을 찍고, 그 위치들을 배회하도록
        // cc.SImpleMove 사용할 것(충돌처리 필요)
        //Vector3 originPos = transform.position; // 이렇게 하면 원점 계속 업데이트된다
        Vector3 pos1 = originPosition + 10 * Vector3.forward;
        Vector3 pos2 = originPosition + 10 * Vector3.right;
        Vector3 pos3 = originPosition - 10 * Vector3.forward;
        Vector3 pos4 = originPosition - 10 * Vector3.right;
        Vector3[] patrolPos = { pos1, pos2, pos3, pos4 };

        float distToPos = Vector3.Distance(transform.position, patrolPos[current]);
        if (distToPos > 0.1f) // 현재 목표로 하는 정점과의 거리가 0.1 초과면
        {
            // 이동
            transform.position = Vector3.MoveTowards(transform.position, patrolPos[current], idleSpeed * Time.deltaTime);

            // 몸체 회전
            Vector3 dir = patrolPos[current] - transform.position;
            dir.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);
        }
        else // 현재 목표로 하는 정점과의 거리가 0.1 이하면
        {
            transform.position = patrolPos[current];
            //current = (current + 1) % patrolPos.Length; // pos1 2 3 4 순서로 이동
            current = UnityEngine.Random.Range(0, 4); // 랜덤 정점 이동
        }

        // 플레이어와 거리 구해 특정 수치 미만이면 Move 시작
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (playerInSight && distance < startMoveDist)
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }
    }

    public float moveSpeed = 1;
    [Header("[ Dash Settings ]")]
    public float startDashDist = 20; // 5만큼 Move 5만큼 Dash
    private void StateMove()
    {
        // 일정거리만큼 걸어가다 (공격 전)남은거리만큼은 Dash로 빠르게 이동
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지
        //transform.forward = dir; // 몸체 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd 상태 대비해 Simplemove로 변경

        // 상태 전환
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startDashDist)
        {
            anim.SetTrigger("Dash");
            state = State.Dash;
        }
    }

    public float dashSpeed = 10;
    [Header("[ Attack Settings ]")]
    public float startAttackDist = 15; // *** 둘의 콜라이더 반지름 합친거 + 0.5f 정도로 수정
    private void StateDash()
    {
        // Dash로 플레이어에게 이동하다 거리 15되면 공격 시작

        // (1) 몸체 회전
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0821
        //transform.forward = dir; // 몸체 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정

        // Dash
        //transform.position = Vector3.Lerp(transform.position, player.transform.position, dashSpeed * Time.deltaTime);
        cc.SimpleMove(dir * dashSpeed);

        // 상태 전환
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startAttackDist)
        {
            state = State.Attack;
        }
    }

    public float attackCurrentTime = 0.5f;
    public float attackTime = 3;
    public GameObject arrowFactory;
    public GameObject arrowPosition;
    public GameObject arrowFireEffect;
    [Header("[ Run Settings ]")]
    public int runHPGap = 2;
    private void StateAttack()
    {
        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // 플레이어가 원거리 공격거리(15) 벗어나면 다시 플레이어에게로 Dash
            if (distance > startAttackDist)
            {
                anim.SetTrigger("Dash");
                state = State.Dash;
            }
            // 원거리 공격거리(15) 내부에 있다면 && 공격 가능 상태라면 공격
            else if (canAttack == true)
            {
                // *******************************
                // 여기에도 anim 하나 넣어줘야 할 것 같긴 한데...
                currentTime += Time.deltaTime;
                if(currentTime > 0.1f)
                {
                    anim.SetTrigger("AttackWait");
                    currentTime = 0;
                }


                // 플레이어 방향으로 enemy 몸체 지속 회전               
                //Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                //transform.rotation = rotation;
                // (1) 몸체 회전
                Vector3 dir = player.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0821
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
                // 회전하면서 바닥으로 파고드는 현상 해결
                // ********** 단순 숫자가 아닌 ray를 이용한 (바닥높이 + 1)로 수정!!!
                //transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                attackCurrentTime += Time.deltaTime;
                if (attackCurrentTime > attackTime)
                {
                    anim.SetTrigger("Attack");
                    attackCurrentTime = 0;

                    // 0817 이벤트함수로 이동
                    /*// attackTime이 되면 화살 생성
                    // ** arrow자체에서 AddDamage하는 방향으로 갈까..??
                    GameObject arrow = Instantiate(arrowFactory); 

                    // 화살 발사 위치의 forward를 player 향하게 회전
                    Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
                    arrowPosition.transform.forward = dir2;

                    // 화살/총구 위치로 변경
                    arrow.transform.position = arrowPosition.transform.position;
                    arrow.transform.forward = arrowPosition.transform.forward;*/

                    // 활/대포 쏠 때 enemy 살짝 밀려났다가 돌아오는 효과(반동)
                    //StartCoroutine("IeRecoil");

                    // *** 화살/대포에 맞는 순간 데미지 깎아야함 -> enemyArrow 자체에서 구현
                    //LHE_test_PlayerHP.Instance.AddDamage(1);
                }
            }
        }

        // 플레이어와 나의 체력을 비교해 내가 특정 수치 이상으로 적으면 도망
        if (LHE_EnemyFarHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap && LHE_EnemyFarHP.Instance.HP > 0)
        {
            anim.SetTrigger("Run");
            state = State.Run;
        }
    }

    // 0817 화살 발사 이벤트 함수
    internal void OnArrowAttack()
    {
        //print("arrowarrowarrowarrowarrowarrow");
        // attackTime이 되면 화살 생성
        // ** arrow자체에서 AddDamage하는 방향으로 갈까..??
        GameObject arrow = Instantiate(arrowFactory);
        arrowSound.Play(); //0821

        // 화살 발사 위치의 forward를 player 향하게 회전
        Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
        arrowPosition.transform.forward = dir2;

        // 화살/총구 위치로 변경
        arrow.transform.position = arrowPosition.transform.position;
        arrow.transform.forward = arrowPosition.transform.forward;

        // 발사 이펙트
        GameObject arrowEffect = Instantiate(arrowFireEffect);
        arrowEffect.transform.position = arrowPosition.transform.position;
        arrowEffect.transform.forward = arrowPosition.transform.forward;
        Destroy(arrowEffect, 10);

        StartCoroutine("IeRecoil");
    }

    // 총/대포 발사 후 enemy 몸체 반동 효과
    IEnumerator IeRecoil()
    {
        // 밀려남
        transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * 1f, 5 * Time.fixedDeltaTime);

        yield return new WaitForSeconds(0.1f);

        // 돌아옴
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 1f, 1 * Time.fixedDeltaTime);
    }
    

    public float runSpeed = 8;
    private void StateRun()
    {

        // 초기 위치로 도망
        // *** 추후 초기 위치 말고 스크린 상의 특정 위치로 수정??
        Vector3 dir = originPosition - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
        dir.Normalize();

        //transform.position = Vector3.Lerp(transform.position, originPosition, runSpeed * Time.deltaTime);
        cc.SimpleMove(dir * runSpeed); // 0806 RUN 방식 수정(transform -> simplemove)

        float distance = Vector3.Distance(transform.position, originPosition);
        //print("distance to originPos " + distance); // 0817 test
        if (distance < 1f && LHE_EnemyFarHP.Instance.HP > 0)
        {
            transform.position = originPosition;
            anim.SetTrigger("AfterRunIdle");
            state = State.AfterRunIdle;
        }
    }

    public float afterRunIdleTime = 30;
    private void StateAfterRunIdle()
    {
        //print("AfterRunIdleanim");

        // [Patrol]: 처음 Idle보다는 좁은 범위를
        // Idle 상태 진입시의 첫번째 위치를 기준으로 사방에 정점을 찍고, 그 위치들을 배회하도록
        // cc.SImpleMove 사용할 것(충돌처리 필요)
        //Vector3 originPos = transform.position; // 이렇게 하면 원점 계속 업데이트된다
        Vector3 pos1 = originPosition + 10 * Vector3.forward;
        Vector3 pos2 = originPosition + 10 * Vector3.right;
        Vector3 pos3 = originPosition - 10 * Vector3.forward;
        Vector3 pos4 = originPosition - 10 * Vector3.right;
        Vector3[] patrolPos = { pos1, pos2, pos3, pos4 };

        float distToPos = Vector3.Distance(transform.position, patrolPos[current]);
        if (distToPos > 0.1f) // 현재 목표로 하는 정점과의 거리가 0.1 초과면
        {
            // 이동
            transform.position = Vector3.MoveTowards(transform.position, patrolPos[current], idleSpeed * Time.deltaTime);

            // 몸체 회전
            Vector3 dir = patrolPos[current] - transform.position;
            dir.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);
        }
        else // 현재 목표로 하는 정점과의 거리가 0.1 이하면
        {
            transform.position = patrolPos[current];
            //current = (current + 1) % patrolPos.Length; // pos1 2 3 4 순서로 이동
            current = UnityEngine.Random.Range(0, 4); // 랜덤 정점 이동
        }

        // (Case 1) 일정 시간이 흐르기 전
        // 한 번 도망 후에는 플레이어가 공격거리 안으로 직접 왔을 때만 공격
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startAttackDist)
        {
            state = State.AfterRunAttack;
        }

        // (Case 2) 일정 시간이 흐른 뒤
        // 일정 시간이 흘러도 player가 반경 내 들어오지 않으면 player를 찾아 직접 Move
        currentTime += Time.deltaTime;
        if (currentTime > afterRunIdleTime)
        {
            //state = State.Move;// Move로 전환하면 왔다가 체력 비교하고 다시 도망간다,, 소용 X
            state = State.AfterRunMove;
            anim.SetTrigger("AfterRunMove");
            currentTime = 0;
        }
    }

    private void StateAfterRunMove()
    {

        // 일반 Move와 동일한데 남은거리 Dash만 제외(필요하면 넣지뭐..)
        // 일정거리만큼 걸어가다 (공격 전)남은거리만큼은 Dash로 빠르게 이동 // 일반Move
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0821
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd 상태 대비해 Simplemove로 변경

        // 상태 전환
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startAttackDist)
        {
            state = State.AfterRunAttack;
        }
    }

    public float afterRunAttackCurrentTime = 2.5f;
    private void StateAfterRunAttack()
    {

        // 한 번 도망 후 공격: 플레이어가 공격거리 안에 들어오면 공격, 나가면 Idle
        float distance = Vector3.Distance(transform.position, player.transform.position);

        currentTime += Time.deltaTime;
        if (currentTime > 0.1f)
        {
            anim.SetTrigger("AfterRunAttackWait");
            currentTime = 0;
        }

        if (distance < startAttackDist && canAttack == true)
        {
            // 플레이어 방향으로 enemy 몸체 지속 회전               
            //Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            //transform.rotation = rotation;
            Vector3 dir = player.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0821
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
            // 회전하면서 바닥으로 파고드는 현상 해결
            // ********** 단순 숫자가 아닌 ray를 이용한 (바닥높이 + 1)로 수정!!!
            //transform.position = new Vector3(transform.position.x, 1, transform.position.z);


            afterRunAttackCurrentTime += Time.deltaTime;
            if (afterRunAttackCurrentTime > attackTime)
            {
                anim.SetTrigger("AfterRunAttack");

                //LHE_test_PlayerHP.Instance.AddDamage(1); // -> enemyArrow 자체에서 구현

                afterRunAttackCurrentTime = 0;
            }
        }
        else
        {
            anim.SetTrigger("AfterRunMove");
            state = State.AfterRunMove;
        }
    }

    [Header("[ Wire Settings ]")]
    public float wireSpeed = 10;
    private void StateWire()
    {
        // NavMeshAgent 비활성화 -> 안해주면 Climb상태에서 벽을 못오르는듯
        //agent.enabled = false;

        StartCoroutine("IeWire");
    }

    public float wireWaitTime = 3f;
    IEnumerator IeWire()
    {
        // 플레이어의 hitinfo.point를 가져온다
        Vector3 playerHitPoint = YSMPlayerAttack.Instance.wireHitInfo.point;
        // 플레이어의 hitinfo.normal을 가져온다 // 0809 보편화
        Vector3 playerHitNormal = YSMPlayerAttack.Instance.wireHitInfo.normal;
        // normal 길이가1인 외적벡터를 구한다
        Vector3 playerHitCross = Vector3.Cross(playerHitNormal, Vector3.up).normalized;


        // 0806 Player Wire가 벽이 아닌 곳에도 작동하는 상황 대비해 if-else문 추가
        // ***** 추후 tag 등을 통해 ray 닿은곳이 Wall일 때만 player Wire 가능하게 수정 필요
        if (playerHitPoint.y >= 1)
        {
            // enemy의 Wire 도착지점 설정
            // = playerHitPoint에서 왼쪽으로 2만큼, 벽에서 1만큼 떨어진 지점
            // (근거리 enemy가 오른쪽 2지점에 오기 때문에 겹치지 않기 위해 왼쪽 2지점으로)
            Vector3 enemyWirePosition = playerHitPoint + (playerHitCross * -2f + playerHitNormal);

            // 일정시간 기다림
            yield return new WaitForSeconds(wireWaitTime);

            // 일정시간 기다린 후 실제 와이어액션 취하기 때문에 애니메이션 이 타이밍에 재생
            anim.SetTrigger("Wire");

            // 몸체 회전
            transform.rotation = Quaternion.LookRotation(-YSMPlayerAttack.Instance.wireHitInfo.normal);

            // 이동
            transform.position = Vector3.Lerp(transform.position, enemyWirePosition, wireSpeed * Time.fixedDeltaTime);

           
            
            // Climb 상태로 전환
            float distance = Vector3.Distance(transform.position, enemyWirePosition);
            //print("FARenemy-enemyWirePosition dist : " + distance);
            if (distance < 1f)
            {
                transform.position = enemyWirePosition;
                anim.SetTrigger("Climb");
                state = State.Climb;
            }
        }
    }

    [Header("[ Climb Settings ]")]
    public float wallHeight = 10; // ******* 추후 hitinfo에서 받아온 정보로 수정!!(모든 높이의 병게 대응할 수 있어야 함)
    public float climbSpeed = 0.5f;
    Vector3 walkEndPos;
    private void StateClimb()
    {
        // *********** 추후 애니메이션 타이밍 미세 조절

        // 0817 올라가는 단계를 한 번 더 나눠서 끝단에서 잡고 올라가는거 넣어줄까??
        // 캐릭터의 position.y값에 따라서 wallheight에 숫자 더해줘야할수도
        if (transform.position.y <= rayWallHeight * 2)
        {
            transform.position += transform.up * climbSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // 걷기 시작점과 끝점을 미리 구해두고
            walkEndPos = transform.position + transform.forward * 2f;

            // 상태 변경
            anim.SetTrigger("ClimbEndWalk");
            state = State.ClimbEndWalk;
        }
    }

/*    IEnumerator ClimbEndWalk() // 사용 안 함
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * 2f;

        while (true)
        {
            if (currentTime < 5)
            {
                currentTime += Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, currentTime);
                yield return null;
            }
            else
            {
                // 상태변경
                //state = State.Idle;

                // NVA 다시 활성화
                //agent.enabled = true;

                yield break;
            }
        }
    }*/

    private void StateClimbEndWalk()
    {
        // [climb을 끝낸 뒤 앞방향으로 2만큼 나아가고, 이후 상태를 AfterClimbIdle로 전환하고싶다]


        transform.position = Vector3.Lerp(transform.position, walkEndPos, 1);

        float distance = Vector3.Distance(transform.position, walkEndPos);
        if (distance < 0.1f)
        {
            transform.position = walkEndPos;

            anim.SetTrigger("AfterClimbIdle");
            state = State.AfterClimbIdle;
        }
    }

    public float afterClimbIdleTime = 15;
    private void StateAfterClimbIdle()
    {

        // (1) player가 벽 정상으로 올라온다면 시야각 체크 안하고 Move
        if ((player.transform.position.y > (transform.position.y - 1.2f)) && (player.transform.position.y < (transform.position.y + 1.2f)))
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }

        // (2) player가 올라오다가 바닥으로 간다면 다시 따라감
        // => 이것도 결국 일정시간동안 거리 안에 안들어도면 Move로 전환하면 될듯??
        currentTime += Time.deltaTime;
        if (currentTime > afterClimbIdleTime)
        {
            //state = State.Move;// Move로 전환하면 왔다가 체력 비교하고 다시 도망간다,, 소용 X
            anim.SetTrigger("AfterRunMove");
            state = State.AfterRunMove;
            currentTime = 0;
        }
    }

    [Header("[ Die Settings ]")]
    public float dieAnimLength = 10;
    private void StateDie()
    {
        //anim.SetTrigger("Die");

        cc.enabled = false;

        currentTime += Time.deltaTime;
        if(currentTime > dieAnimLength)
        {
            Destroy(gameObject);
        }
    }

    // Move, Dash, Attack Range Gizmo Draw
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, startMoveDist);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, startDashDist);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startAttackDist);
    }
}
