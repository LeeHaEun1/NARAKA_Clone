using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [AI Enemy(1)]
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

public class LHE_EnemyMove : MonoBehaviour
{
    // Singleton
    public static LHE_EnemyMove Instance;
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
        AfterRunMove,
        AfterRunAttack,
        Wire,
        Climb,
        ClimbEndWalk, // 0811: 코루틴에서 상태로 변경
        AfterClimbIdle, // 0811: 신규 추가
        Attacked,
        Attacked2,
        Attacked3,
        Die
    }
    public State state;

    // Components
    CharacterController cc;
    Rigidbody rb; // 사용안함
    NavMeshAgent agent; // 사용안함
    public Animator anim;

    //[Effect]
    AudioSource swordSound;
    AudioSource attackedAudio1; // 피격효과음
    AudioSource attackedAudio2; // 피격효과음

    Vector3 originPosition;
    GameObject player;
    bool playerInSight; // 시야각 체크
    bool canAttack; // 공격 가능 여부
    public bool isDead; // 사망 여부

    // Start is called before the first frame update
    void Start()
    {
        // Character Controller
        cc = GetComponent<CharacterController>();
        // Rigidbody;
        //rb = GetComponent<Rigidbody>();
        // Nav Mesh Agent
        //agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false; // 컴포넌트에서도 꺼놓긴했음
        // Animator
        anim = GetComponentInChildren<Animator>();

        //[Effect]
        swordSound = GetComponent<AudioSource>();

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

        // 피격 효과음
        attackedAudio1 = GameObject.Find("AttackedAudio1").GetComponent<AudioSource>();
        attackedAudio2 = GameObject.Find("AttackedAudio2").GetComponent<AudioSource>();
        //attacked1 = LHE_AttackedAudioClips.Instance.attackedAudioClips[1];
        //attacked2 = LHE_AttackedAudioClips.Instance.attackedAudioClips[2];
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

        print("near enemy state : " + state);
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
                StateAfterRunMove();
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
                StateClimbEndWalk();
                break;  
            case State.AfterClimbIdle:
                StateAfterClimbIdle();
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
        //Debug.DrawRay(transform.position, transform.position + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, transform.position + new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * (transform.position + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, 0)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * (transform.position - new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, 0)), Color.yellow);

        //Debug.DrawLine(transform.position, transform.forward + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), transform.position.y, transform.position.z), Color.yellow);
        //Debug.DrawLine(transform.position, transform.forward + new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), transform.position.y, transform.position.z), Color.yellow);


        // [Jumping]
        // 절벽 위에서 player 따라가다가 player가 점프하면 enemy가 이상한 방향으로 계속 Dash해버리는 현상 발생
        // 바닥으로 raycast 지속하여 점프 중에는 별도의 Jumping이라는 상태로 변경 -> 바닥에 가서 다시 Idle
        // **** HP 기준으로 Idle vs AfterRunIdle 선택?? (절벽까지 따라갔다는 것은 애초에 HP 차이 안나는 상황이니 Idle로만 할까?)
        // XXXXXXXX 이렇게 하니까 처음부터 Jumping과 Idle 동시출력 & Wire 후 Climb 못한다
        //Ray floorRay = new Ray(transform.position, -Vector3.up);
        //RaycastHit floorRayHitinfo;
        //if(Physics.Raycast(floorRay, out floorRayHitinfo))
        //{
        //    float distance = Vector3.Distance(transform.position, floorRayHitinfo.point);
        //    print("distance to floor " + distance);

        //    if(distance > 1)
        //    {
        //        state = State.Jumping;
        //    }
        //}

        // [Wire]
        // 어떤 상태이든 Player가 Wire 이동하면 근처 벽으로 따라서 Wire Action
        // *** Run과 afterRun 상태들은 제외할까..??
        // ******* player의 hitpoint가 wall의 트리거 안에 있을 때
        //print("player wire "+ YSMPlayerAttack.Instance.enemyCanWire);
        //print("player wirehitinfo "+ YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag);
        if (YSMPlayerAttack.Instance.enemyCanWire == true && YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag == "Wall")
        {
            rayWallHeight = YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.transform.position.y;
            //print("rayWallHeight " + rayWallHeight);
            state = State.Wire;
        }


        // [Die]
        // 어떤 상태이든 체력 0되면 Die
        if (state != State.Die && LHE_EnemyHP.Instance.HP <= 0)
        {
            //if (isDead == true)
            //{
            //    anim.SetTrigger("Die");
            //    isDead = false; 
            //}
            StopAllCoroutines();
            state = State.Die;
            anim.SetTrigger("Die");
        }
    }


    [Header("[ Field of View Settings ]")]
    public float sightDegree = 60;
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
    // 어떤 상태이든 Player가 공격하면 피격 상태로 전환
    // *** 나의 LHE_EnemyHP의 AddDamage가 호출되는 순간으로 수정할 수는 없나??
    // *** 공격 당하는 동안도 거리 때문에 Attack 상태라서 밀려난 다음 다시 Player방향으로 돌아온다
    [Header("[ Attacked Settings ]")]
    public float attackedSpeed = 5;
    public float attackedTime = 2;
    public float attackedDist = 3;
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
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, attackedPosition, attackedSpeed * Time.deltaTime);
        }
        else
        {
            canAttack = true;
            if (LHE_EnemyHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
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
            canAttack = false;
            currentTime += Time.fixedDeltaTime;
            transform.rotation = Quaternion.Lerp(LHE_EnemyHP.Instance.CurrRot, LHE_EnemyHP.Instance.CurrRot * Quaternion.Euler(0, 50, 0), currentTime);
        }
        else
        {
            canAttack = true;
            if (LHE_EnemyHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
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

    // [피격3] 포물선 그리며 뒤로 날라가기
    //public float bounceForce = 400;
    //private void StateAttacked2()
    //{
    //    if (currentTime < 10)
    //    {
    //        currentTime += Time.fixedDeltaTime;
    //        rb.AddExplosionForce(bounceForce, transform.position, 500, 500);
    //        print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

    //    }
    //    else
    //    {
    //        state = State.Idle;
    //        currentTime = 0;
    //    }
    //}

    //private void StateAttacked3()
    //{
    //    if (currentTime < 3)
    //    {
    //        currentTime += Time.fixedDeltaTime;

    //        Vector3 currPos = transform.position + Parabola(1, 5, currentTime, -100f);
    //        transform.position = currPos;

    //        print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    //    }
    //    else
    //    {
    //        state = State.Attack;
    //        currentTime = 0;
    //    }
    //}


    // 포물선 운동 공식(velocity, theta, time, gravity)
    public static Vector3 Parabola(float v, float theta, float t, float g)
    {
        float x = v * MathF.Cos(theta) * t;
        float y = v * MathF.Sin(theta) - 0.5f * g * t * t;
        return new Vector3(0, y, -x);
    }

    // [피격3] 정점과 끝점 찍는 식으로 뒤로 날라가기
    Vector3 HighPos;
    Vector3 EndPos;
    private void StateAttacked3()
    {
        // ************dir = enemy위치- player위치 구하고 dir.y=0 한 방향으로 수정 // 0810
        // [피격 방향 구하기]
        Vector3 attackedDir = transform.position - player.transform.position;
        attackedDir.Normalize();
        attackedDir.y = 0;

        //HighPos = LHE_EnemyHP.Instance.CurrPos + player.transform.forward * 3 + Vector3.up * 3;
        //EndPos = LHE_EnemyHP.Instance.CurrPos + player.transform.forward * 6;
        HighPos = LHE_EnemyHP.Instance.CurrPos + attackedDir * 2 + Vector3.up * 2;
        EndPos = LHE_EnemyHP.Instance.CurrPos + attackedDir * 4;

        if (transform.position == LHE_EnemyHP.Instance.CurrPos)
        {
            anim.SetTrigger("Attacked3Up");
            StartCoroutine("IeFly");
        }
        //else if (transform.position == EndPos)
        //{w
        //    state = State.Attack;
        //}
        
    }

    //public float attacked3CurrTime = 0;
    IEnumerator IeFly() // 피격3
    {
        while (true)
        {
            if(currentTime < 0.5f)
            {
                currentTime += Time.deltaTime;
                //anim.SetTrigger("Attacked3Up");
                transform.position = Vector3.Lerp(transform.position, HighPos, currentTime * 2);
                yield return null;
            }
            else if(currentTime < 1)
            {
                currentTime += Time.deltaTime;
                anim.SetTrigger("Attacked3Down");
                transform.position = Vector3.Lerp(HighPos, EndPos, currentTime);
                //yield return null;
            }
            else
            {
                if (LHE_EnemyHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
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

/*
        //attacked3CurrTime += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, HighPos, 0.5f);
        yield return new WaitForSeconds(0.1f);
        transform.position = Vector3.Lerp(HighPos, EndPos, 0.5f);

        float distance = Vector3.Distance(transform.position, EndPos);
        if (distance < 0.3f)
        {
            attacked3CurrTime = 0;
            transform.position = EndPos;
            state = State.Attack;
        }*/
    }

    [Header("[ Idle Settings ]")]
    public float idleSpeed = 1;
    int current = 0; // patrolPos 배열 index
    [Header("[ Move Settings ]")]
    public float startMoveDist = 10;
    private void StateIdle()
    {
        // [이때만 잠시 NavMeshAgent 켜줄까...?]
        // 0812 이렇게 해주니까 경로아닌곳, 즉 벽에 닿으면 멈춰버리네,,, 그냥 별 안걸치는 곳에 배치를 하는게 나을수도...
        //agent.enabled = true;

        // [Patrol]
        // Idle 상태 진입시의 첫번째 위치를 기준으로 사방에 정점을 찍고, 그 위치들을 배회하도록
        // cc.SImpleMove 사용할 것(충돌처리 필요)
        //Vector3 originPos = transform.position; // 이렇게 하면 원점 계속 업데이트된다
        Vector3 pos1 = originPosition + 10*Vector3.forward;
        Vector3 pos2 = originPosition + 10*Vector3.right;
        Vector3 pos3 = originPosition - 10*Vector3.forward;
        Vector3 pos4 = originPosition - 10*Vector3.right;
        Vector3[] patrolPos = {pos1, pos2, pos3, pos4};

        float distToPos = Vector3.Distance(transform.position, patrolPos[current]);
        if(distToPos > 0.1f) // 현재 목표로 하는 정점과의 거리가 0.1 초과면
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
            //print("current position is patrolPos");
        }


        // 플레이어와 거리 구해 특정 수치 미만이면 Move 시작
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (playerInSight && distance < startMoveDist)
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }
    }

    public float moveSpeed = 5;
    [Header("[ Dash Settings ]")]
    public float startDashDist = 6; // 4만큼 Move 6만큼 Dash
    private void StateMove()
    {
        //anim.SetTrigger("Move");

        // 어차피 SimpleMove 사용할 것이므로 Move 넘어오면 agent꺼준다
        // 0812 idle에서 agent쓰는거 포기..하면 필요없어짐ㅂ
        //agent.enabled = false;

        // 일정거리만큼 걸어가다 (공격 전)남은거리만큼은 Dash로 빠르게 이동
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0807 simplemove로 변경
        //transform.forward = dir; // 몸체 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd 상태 대비해 Simplemove로 변경

        // 상태 전환
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startDashDist) // 
        {
            anim.SetTrigger("Dash");
            state = State.Dash;
        }
    }

    public float dashSpeed = 10;
    [Header("[ Attack Settings ]")]
    public float startAttackDist = 0.5f; // *** 둘의 콜라이더 반지름 합친거 + 0.5f 정도로 수정
    private void StateDash()
    {
        //anim.SetTrigger("Dash");

        // [Dash로 플레이어에게 이동하다 공격 시작]

        // (1) 몸체 회전
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 플레이어 점프 시 enemy 공중으로 뜨는 현상 방지 // 0807
        //transform.forward = dir; // 몸체 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정

        // (2) Ray 쏴서 바닥 상태이면 Dash, 그렇지 않으면 일단 점프 후 상태 변경
        //Ray floorRay = new Ray(transform.position, -Vector3.up);
        //RaycastHit floorRayHitinfo;
        //if (Physics.Raycast(floorRay, out floorRayHitinfo))
        //{
        //    print("Ray Hit");
        //    float rayDist = Vector3.Distance(transform.position, floorRayHitinfo.point);
        //    print("distance to floor " + rayDist);

        //    if (rayDist > wallHeight)
        //    {
        //        transform.position += -Vector3.up * dashSpeed * Time.fixedDeltaTime;
        //        //StartCoroutine("IeJump");
        //    }
        //    else
        //    {
        //        //Vector3 newDir = player.transform.position - transform.position;
        //        cc.SimpleMove(dir * dashSpeed); // 0807
        //    }
        //}

        // ***********조건 바꾸기 -> player가 wall에서 나갓을 때로
        //if((transform.position.y - player.transform.position.y) > (wallHeight / 2))
        //{
        //    state = State.Jump;
        //}
        //else
        //{
        cc.SimpleMove(dir * dashSpeed); // 0807
                                        //print("************** wire dir" + dir);

        // 상태 전환
        float distance = Vector3.Distance(transform.position, player.transform.position);
        //print("************** wire distance" + distance);

        if (distance < startAttackDist)
        {
            state = State.Attack;
        }
        //}


        // Dash
        //transform.position = Vector3.Lerp(transform.position, player.transform.position, dashSpeed * Time.deltaTime);
        //if (cc.isGrounded)
        //{
        //    cc.SimpleMove(dir * dashSpeed); // 0807
        //}
        //else
        //{
        //    transform.position += -Vector3.up * dashSpeed * Time.fixedDeltaTime;
        //}            

    }


    //private void StateJump()
    //{
    //    // 몸체 회전
    //    Vector3 dir = player.transform.position - transform.position;
    //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정

    //    // Jump
    //    if (transform.position.y > 1.5f)
    //    {
    //        cc.SimpleMove(transform.forward * moveSpeed);
    //    }

    //    // 상태 변환
    //    else
    //    {
    //        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    //        state = State.Move;
    //    }
    //}

    public float attackCurrentTime = 0.5f;
    public float attackTime = 1;
    //public GameObject swordEndPosition;
    public GameObject slashEffect0;
    public float slash0YPosition = 1;
    public GameObject slashEffect1_1;
    public GameObject slashEffect1_2;
    public GameObject slashEffect2;
    //public GameObject sword;
    [Header("[ Run Settings ]")]
    public int runHPGap = 2;
    private void StateAttack()
    {
        // 플레이어가 공격거리 벗어나면 다시 플레이어에게로 Dash
        //if (player)
        //{
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > startAttackDist)
            {
                anim.SetTrigger("Dash");
                state = State.Dash;
            }
            else if (canAttack == true) // 피격 상태에서는 canAttack == false
            {
            // *******************************
            // 여기에도 anim 하나 넣어줘야 할 것 같긴 한데...
                currentTime += Time.deltaTime;
                if (currentTime > 0.1f)
                {
                    anim.SetTrigger("AttackWait");
                    currentTime = 0;
                }

            // ********* 원거리랑 몸체 회전 방식이 다르다,,, 뭐가 더 최신 수정인거지?? 근거리 적에 대한 점프 공격 test 필요 ***********
            // 몸체 회전
                Vector3 dir = player.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0;
                //transform.rotation.x = Mathf.Clamp(transform.rotation.x, -60, 0);
                // 0813 플레이어 점프 시 너무 꺾이는 경우 있어 제한을 두면 좋겠는데..
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정
                                                                                                                            //transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                attackCurrentTime += Time.deltaTime;
                if (attackCurrentTime > attackTime)
                {
                    int rand = UnityEngine.Random.Range(0, 3); //0823
                    anim.SetInteger("Attack", rand); // 랜덤공격
                    //anim.SetInteger("Attack", 0); // 0826 test
                    //anim.SetTrigger("Attack");
                    //swordSound.Play(); //0821
                    
                    // 이펙트!!!!
                    //if(rand == 0)
                    //{
                        //GameObject slash0 = Instantiate(slashEffect0);
                        //slashEffect0.transform.position = transform.position;
                    //}
                    //else if(rand == 1)
                    //{

                    //}
                    //else if(rand == 2)
                    //{

                    //}


                    //StartCoroutine("IeSword");
                    //LHE_test_PlayerHP.Instance.AddDamage(1); // ** 병합 시 용어 수정
                    //print("attackanimtime " + anim.GetCurrentAnimatorStateInfo(0));
                    //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    //{
                    attackCurrentTime = 0;
                    //}
                }
            }
        //}

        // 플레이어와 나의 체력을 비교해 내가 특정 수치 이상으로 적으면 도망
        if (LHE_EnemyHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
        {
            anim.SetTrigger("Run");
            state = State.Run;
            //print("runrunrunrururnrunrunrurnrunrurnrunrunrunru");
        }
    }

    // 공격 이펙트 타이밍 이벤트 함수
    internal void OnAttack0()
    {
        swordSound.Play();
        GameObject slash0 = Instantiate(slashEffect0);
        slash0.transform.position = transform.position + Vector3.up * 1.7f;
        Destroy(slash0, 10);
    }

    internal void OnAttack1_1()
    {
        swordSound.Play();
        GameObject slash1_1 = Instantiate(slashEffect1_1);
        slash1_1.transform.position = transform.position;
        Destroy(slash1_1, 10);
    }

    internal void OnAttack1_2()
    {
        swordSound.Play();
        GameObject slash1_2 = Instantiate(slashEffect1_2);
        slash1_2.transform.position = transform.position;
        slash1_2.transform.right = transform.forward;
        Destroy(slash1_2, 10);
    }

    internal void OnAttack2()
    {
        swordSound.Play();
        GameObject slash2 = Instantiate(slashEffect2);
        slash2.transform.position = transform.position + transform.forward*2 + Vector3.up*1.36f;
        slash2.transform.forward = transform.forward;
        Destroy(slash2, 10);
    }

    // 프로토 캡슐 때 붙인 실린더 칼에 사용했던 코루틴
    /*    IEnumerator IeSword()
        {
            sword.transform.Rotate(60, 0, 0);
            yield return new WaitForSeconds(0.1f);
            sword.transform.Rotate(-60, 0, 0);
            print("near enemy sword");
        }*/

    public float runSpeed = 8;
    private void StateRun()
    {
        //anim.SetTrigger("Run");

        // 초기 위치로 도망
        // *** 추후 초기 위치 말고 스크린 상의 특정 위치로 수정??
        Vector3 dir = originPosition - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); //
                                                                                                                    // 0806 몸체 회전 수정
        dir.Normalize();

        //transform.position = Vector3.Lerp(transform.position, originPosition, runSpeed * Time.deltaTime);
        cc.SimpleMove(dir * runSpeed); // 0806 RUN 방식 수정(transform -> simplemove)

        float distance = Vector3.Distance(transform.position, originPosition);
        if (distance < 0.5f)
        {
            transform.position = originPosition;
            anim.SetTrigger("AfterRunIdle");
            state = State.AfterRunIdle;
        }
    }

    public float afterRunIdleTime = 30;
    private void StateAfterRunIdle()
    {

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
        if(currentTime > afterRunIdleTime)
        {
            //state = State.Move;// Move로 전환하면 왔다가 체력 비교하고 다시 도망간다,, 소용 X
            anim.SetTrigger("AfterRunMove");
            state = State.AfterRunMove;
            currentTime = 0;
        }
    }

    private void StateAfterRunMove()
    {

        // 일반 Move와 동일한데 남은거리 Dash만 제외(필요하면 넣지뭐..)
        // 일정거리만큼 걸어가다 (공격 전)남은거리만큼은 Dash로 빠르게 이동 // 일반Move
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 0821
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
        // ******* 0807 추후 (1) 시야각 check 추가하고 (2) 되돌아간대로 뒤돌아있다가 한 대 맞으면 뒤돌아보는거로 수정
        float distance = Vector3.Distance(transform.position, player.transform.position);

        currentTime += Time.deltaTime;
        if (currentTime > 0.1f)
        {
            anim.SetTrigger("AfterRunAttackWait");
            currentTime = 0;
        }

        if (distance < startAttackDist && canAttack == true)
        {
            Vector3 dir = player.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0; //0821
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 몸체 회전 수정

            afterRunAttackCurrentTime += Time.deltaTime;
            if (afterRunAttackCurrentTime > attackTime)
            {
                int rand = UnityEngine.Random.Range(0, 3);
                anim.SetInteger("AfterRunAttack", rand);
                //swordSound.Play(); //0821


                //StartCoroutine("IeSword");
                //LHE_test_PlayerHP.Instance.AddDamage(1); // ** 병합 시 용어 수정
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
        if (playerHitPoint.y >= 1) // 0811 이거 때문에 씹히는건가,, test용도로 해제
        {
            // enemy의 Wire 도착지점 설정
            // = playerHitPoint에서 오른쪽으로 2만큼, 벽에서 1만큼 떨어진 지점
            Vector3 enemyWirePosition = playerHitPoint + (playerHitCross * 2f + playerHitNormal);

            // 일정시간 기다림
            yield return new WaitForSeconds(wireWaitTime);

            // 일정시간 기다린 후 실제 와이어액션 취하기 때문에 애니메이션 이 타이밍에 재생
            anim.SetTrigger("Wire");

            // 몸체 회전
            transform.rotation = Quaternion.LookRotation(-YSMPlayerAttack.Instance.wireHitInfo.normal);

            // 이동
            transform.position = Vector3.Lerp(transform.position, enemyWirePosition, wireSpeed * Time.fixedDeltaTime); // 0812 deltaTime으로 바꾸니 떨림 더 심해보인다....

            // Climb 상태로 전환
            float distance = Vector3.Distance(transform.position, enemyWirePosition);
            //print("NEARenemy-enemyWirePosition dist : " + distance);
            if (distance < 1f) // ******* 0812 미만 판정 값 늘려도 떨린다... 몸체 회전문게인가??
            {
                transform.position = enemyWirePosition;
                anim.SetTrigger("Climb");
                state = State.Climb;
            }
        }
    }

    [Header("[ Climb Settings ]")]
    public float wallHeight = 10; // ****** 사용 XXXXXX
    public float climbSpeed = 1;
    Vector3 walkEndPos;
    private void StateClimb()
    {

        // 캐릭터의 position.y값에 따라서 wallheight에 숫자 더해줘야할수도
        if (transform.position.y <= rayWallHeight * 2)
        {
            //transform.position += transform.up * moveSpeed * Time.fixedDeltaTime; // wire잘못쐈을 때 enemy도 이상한 방향으로 무조건 머리로 올라가는 현상
            transform.position += transform.up * climbSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // 걷기 시작점과 끝점을 미리 구해두고
            //walkStartPos = transform.position;
            walkEndPos = transform.position + transform.forward * 2f;
            
            // 상태 변경
            anim.SetTrigger("ClimbEndWalk");
            state = State.ClimbEndWalk;
            //StartCoroutine("ClimbEndWalk");
        }
    }

   /* //float climbEndRayDist;
    IEnumerator ClimbEndWalk() // 사용 안 함
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
                // 상태변경 // 0811: Idle ->  Move로 변경(idle하면 시야각 문제로 가장자리에 멈춰버리는 현상 발생)
                // Move로 하니 player가 별 덜 탄 상태에서도 아래로 내려가버림.. AfterClimbIdle 상태를 따로 팔까??
                state = State.Idle;

                // NVA 다시 활성화
                //agent.enabled = true;

                // (Ray) ClimbEndFloorDist
                //Ray climbEndRay = new Ray(transform.position, -Vector3.up);
                //RaycastHit climbEndRayHitinfo;
                //if (Physics.Raycast(climbEndRay, out climbEndRayHitinfo))
                //{
                //    climbEndRayDist = Vector3.Distance(transform.position, climbEndRayHitinfo.point);
                //    print("climbEndRayDist " + climbEndRayDist);
                //}

                yield break;
            }
        }
    }*/

    private void StateClimbEndWalk()
    {

        // [climb을 끝낸 뒤 앞방향으로 2만큼 나아가고, 이후 상태를 AfterClimbIdle로 전환하고싶다]

        transform.position = Vector3.Lerp(transform.position, walkEndPos, 1);

        float distance = Vector3.Distance(transform.position, walkEndPos);
        if(distance < 0.1f)
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
        if ((player.transform.position.y > (transform.position.y - 0.1f)) && (player.transform.position.y < (transform.position.y + 2f)))
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
        cc.enabled = false;

        currentTime += Time.deltaTime;
        if (currentTime > dieAnimLength)
        {
            Destroy(gameObject);
        }
    }

    // Move, Dash, Attack Range Gizmo Draww
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
