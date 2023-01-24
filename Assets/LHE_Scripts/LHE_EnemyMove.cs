using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [AI Enemy(1)]
// 1. FSM
// �̵� �� �׻� �̵������ ���ʹ� �� ���� ��ġ�ϵ��� ��ü ȸ��
// *** �ܼ� �Ÿ��� �ƴ϶� ��ũ�� ���� ��ǥ�� �ϴ� �� ��������..?
// Idle
// �յ��¿� -> �̵��������� ��ü ȸ��
// �÷��̾ �����Ÿ� �ȿ� ������ �ڵ����� aim���� ����(��ü�� �׸�ŭ ȸ��)
// �뽬
// ����..(ü���� ���� ��ġ ���ϰ� �Ǹ� -> �÷��̾� ü�µ� ���(���� ��))
// (����)
// (���̾� �׼�)
// (������)

// ������ �߿��� ���� "�ǰݰ�"
// -> �÷��̾��� ���⺰�� �ٸ��� ����? damage ����??

// NavMeshAgent

// ���� �� Animation

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
        ClimbEndWalk, // 0811: �ڷ�ƾ���� ���·� ����
        AfterClimbIdle, // 0811: �ű� �߰�
        Attacked,
        Attacked2,
        Attacked3,
        Die
    }
    public State state;

    // Components
    CharacterController cc;
    Rigidbody rb; // ������
    NavMeshAgent agent; // ������
    public Animator anim;

    //[Effect]
    AudioSource swordSound;
    AudioSource attackedAudio1; // �ǰ�ȿ����
    AudioSource attackedAudio2; // �ǰ�ȿ����

    Vector3 originPosition;
    GameObject player;
    bool playerInSight; // �þ߰� üũ
    bool canAttack; // ���� ���� ����
    public bool isDead; // ��� ����

    // Start is called before the first frame update
    void Start()
    {
        // Character Controller
        cc = GetComponent<CharacterController>();
        // Rigidbody;
        //rb = GetComponent<Rigidbody>();
        // Nav Mesh Agent
        //agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false; // ������Ʈ������ ����������
        // Animator
        anim = GetComponentInChildren<Animator>();

        //[Effect]
        swordSound = GetComponent<AudioSource>();

        // �ʱ� ��ġ
        originPosition = transform.position;
        // �ʱ� ����
        state = State.Idle;

        // �÷��̾� ����, �÷��̾� HP
        player = GameObject.Find("Player");

        // �ʱ� �þ߰� = false
        playerInSight = false;
        // �ʱ� ���� ���� ���� = true;
        canAttack = true;
        // �ʱ� ��� ����
        isDead = false;

        // �ǰ� ȿ����
        attackedAudio1 = GameObject.Find("AttackedAudio1").GetComponent<AudioSource>();
        attackedAudio2 = GameObject.Find("AttackedAudio2").GetComponent<AudioSource>();
        //attacked1 = LHE_AttackedAudioClips.Instance.attackedAudioClips[1];
        //attacked2 = LHE_AttackedAudioClips.Instance.attackedAudioClips[2];
    }

    // ����: Attacked, StateAttack
    public float currentTime = 0;
    // Test �뵵: ���� player���� �Ÿ�
    public float currentDistance;
    // �÷��̾ RAY �� WALL�� ��������
    float rayWallHeight;
    // Update is called once per frame
    void Update()
    {
        // Test �뵵: ���� player���� �Ÿ�
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
        // �¿� ���� 180��
        playerInSight = CheckPlayerAngle(player.transform.position);
        // Draw: ���� 20�� �¿� �þ߰� �Ѱ� ǥ��
        //Debug.DrawRay(transform.position, transform.position + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, transform.position + new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * (transform.position + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, 0)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * (transform.position - new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, 0)), Color.yellow);

        //Debug.DrawLine(transform.position, transform.forward + new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), transform.position.y, transform.position.z), Color.yellow);
        //Debug.DrawLine(transform.position, transform.forward + new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), transform.position.y, transform.position.z), Color.yellow);


        // [Jumping]
        // ���� ������ player ���󰡴ٰ� player�� �����ϸ� enemy�� �̻��� �������� ��� Dash�ع����� ���� �߻�
        // �ٴ����� raycast �����Ͽ� ���� �߿��� ������ Jumping�̶�� ���·� ���� -> �ٴڿ� ���� �ٽ� Idle
        // **** HP �������� Idle vs AfterRunIdle ����?? (�������� ���󰬴ٴ� ���� ���ʿ� HP ���� �ȳ��� ��Ȳ�̴� Idle�θ� �ұ�?)
        // XXXXXXXX �̷��� �ϴϱ� ó������ Jumping�� Idle ������� & Wire �� Climb ���Ѵ�
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
        // � �����̵� Player�� Wire �̵��ϸ� ��ó ������ ���� Wire Action
        // *** Run�� afterRun ���µ��� �����ұ�..??
        // ******* player�� hitpoint�� wall�� Ʈ���� �ȿ� ���� ��
        //print("player wire "+ YSMPlayerAttack.Instance.enemyCanWire);
        //print("player wirehitinfo "+ YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag);
        if (YSMPlayerAttack.Instance.enemyCanWire == true && YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag == "Wall")
        {
            rayWallHeight = YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.transform.position.y;
            //print("rayWallHeight " + rayWallHeight);
            state = State.Wire;
        }


        // [Die]
        // � �����̵� ü�� 0�Ǹ� Die
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


    // [�ǰ�]
    // � �����̵� Player�� �����ϸ� �ǰ� ���·� ��ȯ
    // *** ���� LHE_EnemyHP�� AddDamage�� ȣ��Ǵ� �������� ������ ���� ����??
    // *** ���� ���ϴ� ���ȵ� �Ÿ� ������ Attack ���¶� �з��� ���� �ٽ� Player�������� ���ƿ´�
    [Header("[ Attacked Settings ]")]
    public float attackedSpeed = 5;
    public float attackedTime = 2;
    public float attackedDist = 3;
    // [�ǰ�1] �з����ٰ� �ǵ��ƿ���
    private void StateAttacked()
    {
        // ************dir = enemy��ġ- player��ġ ���ϰ� dir.y=0 �� �������� ���� // 0810
        // [�ǰ� ���� ���ϱ�]
        Vector3 attackedDir = transform.position - player.transform.position;
        attackedDir.Normalize();
        attackedDir.y = 0;

        //Vector3 attackedPosition = transform.position - transform.forward * attackedDist;
        // 0809 �ǰ� ���� ����
        //Vector3 attackedPosition = transform.position + player.transform.forward * attackedDist;
        Vector3 attackedPosition = transform.position + attackedDir * attackedDist;

        // 2�� ���� Lerp??
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

    // [�ǰ�2] �� 50�� Ʋ�����ٰ� �ٽ� �÷��̾� �ٶ󺸱�
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

    // [�ǰ�3] ������ �׸��� �ڷ� ���󰡱�
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


    // ������ � ����(velocity, theta, time, gravity)
    public static Vector3 Parabola(float v, float theta, float t, float g)
    {
        float x = v * MathF.Cos(theta) * t;
        float y = v * MathF.Sin(theta) - 0.5f * g * t * t;
        return new Vector3(0, y, -x);
    }

    // [�ǰ�3] ������ ���� ��� ������ �ڷ� ���󰡱�
    Vector3 HighPos;
    Vector3 EndPos;
    private void StateAttacked3()
    {
        // ************dir = enemy��ġ- player��ġ ���ϰ� dir.y=0 �� �������� ���� // 0810
        // [�ǰ� ���� ���ϱ�]
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
    IEnumerator IeFly() // �ǰ�3
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
    int current = 0; // patrolPos �迭 index
    [Header("[ Move Settings ]")]
    public float startMoveDist = 10;
    private void StateIdle()
    {
        // [�̶��� ��� NavMeshAgent ���ٱ�...?]
        // 0812 �̷��� ���ִϱ� ��ξƴѰ�, �� ���� ������ ���������,,, �׳� �� �Ȱ�ġ�� ���� ��ġ�� �ϴ°� ��������...
        //agent.enabled = true;

        // [Patrol]
        // Idle ���� ���Խ��� ù��° ��ġ�� �������� ��濡 ������ ���, �� ��ġ���� ��ȸ�ϵ���
        // cc.SImpleMove ����� ��(�浹ó�� �ʿ�)
        //Vector3 originPos = transform.position; // �̷��� �ϸ� ���� ��� ������Ʈ�ȴ�
        Vector3 pos1 = originPosition + 10*Vector3.forward;
        Vector3 pos2 = originPosition + 10*Vector3.right;
        Vector3 pos3 = originPosition - 10*Vector3.forward;
        Vector3 pos4 = originPosition - 10*Vector3.right;
        Vector3[] patrolPos = {pos1, pos2, pos3, pos4};

        float distToPos = Vector3.Distance(transform.position, patrolPos[current]);
        if(distToPos > 0.1f) // ���� ��ǥ�� �ϴ� �������� �Ÿ��� 0.1 �ʰ���
        {
            // �̵�
            transform.position = Vector3.MoveTowards(transform.position, patrolPos[current], idleSpeed * Time.deltaTime);

            // ��ü ȸ��
            Vector3 dir = patrolPos[current] - transform.position;
            dir.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);
        }
        else // ���� ��ǥ�� �ϴ� �������� �Ÿ��� 0.1 ���ϸ�
        {
            transform.position = patrolPos[current];
            //current = (current + 1) % patrolPos.Length; // pos1 2 3 4 ������ �̵�
            current = UnityEngine.Random.Range(0, 4); // ���� ���� �̵�
            //print("current position is patrolPos");
        }


        // �÷��̾�� �Ÿ� ���� Ư�� ��ġ �̸��̸� Move ����
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (playerInSight && distance < startMoveDist)
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }
    }

    public float moveSpeed = 5;
    [Header("[ Dash Settings ]")]
    public float startDashDist = 6; // 4��ŭ Move 6��ŭ Dash
    private void StateMove()
    {
        //anim.SetTrigger("Move");

        // ������ SimpleMove ����� ���̹Ƿ� Move �Ѿ���� agent���ش�
        // 0812 idle���� agent���°� ����..�ϸ� �ʿ��������
        //agent.enabled = false;

        // �����Ÿ���ŭ �ɾ�� (���� ��)�����Ÿ���ŭ�� Dash�� ������ �̵�
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0807 simplemove�� ����
        //transform.forward = dir; // ��ü ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd ���� ����� Simplemove�� ����

        // ���� ��ȯ
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startDashDist) // 
        {
            anim.SetTrigger("Dash");
            state = State.Dash;
        }
    }

    public float dashSpeed = 10;
    [Header("[ Attack Settings ]")]
    public float startAttackDist = 0.5f; // *** ���� �ݶ��̴� ������ ��ģ�� + 0.5f ������ ����
    private void StateDash()
    {
        //anim.SetTrigger("Dash");

        // [Dash�� �÷��̾�� �̵��ϴ� ���� ����]

        // (1) ��ü ȸ��
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0807
        //transform.forward = dir; // ��ü ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����

        // (2) Ray ���� �ٴ� �����̸� Dash, �׷��� ������ �ϴ� ���� �� ���� ����
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

        // ***********���� �ٲٱ� -> player�� wall���� ������ ����
        //if((transform.position.y - player.transform.position.y) > (wallHeight / 2))
        //{
        //    state = State.Jump;
        //}
        //else
        //{
        cc.SimpleMove(dir * dashSpeed); // 0807
                                        //print("************** wire dir" + dir);

        // ���� ��ȯ
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
    //    // ��ü ȸ��
    //    Vector3 dir = player.transform.position - transform.position;
    //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����

    //    // Jump
    //    if (transform.position.y > 1.5f)
    //    {
    //        cc.SimpleMove(transform.forward * moveSpeed);
    //    }

    //    // ���� ��ȯ
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
        // �÷��̾ ���ݰŸ� ����� �ٽ� �÷��̾�Է� Dash
        //if (player)
        //{
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > startAttackDist)
            {
                anim.SetTrigger("Dash");
                state = State.Dash;
            }
            else if (canAttack == true) // �ǰ� ���¿����� canAttack == false
            {
            // *******************************
            // ���⿡�� anim �ϳ� �־���� �� �� ���� �ѵ�...
                currentTime += Time.deltaTime;
                if (currentTime > 0.1f)
                {
                    anim.SetTrigger("AttackWait");
                    currentTime = 0;
                }

            // ********* ���Ÿ��� ��ü ȸ�� ����� �ٸ���,,, ���� �� �ֽ� �����ΰ���?? �ٰŸ� ���� ���� ���� ���� test �ʿ� ***********
            // ��ü ȸ��
                Vector3 dir = player.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0;
                //transform.rotation.x = Mathf.Clamp(transform.rotation.x, -60, 0);
                // 0813 �÷��̾� ���� �� �ʹ� ���̴� ��� �־� ������ �θ� ���ڴµ�..
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
                                                                                                                            //transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                attackCurrentTime += Time.deltaTime;
                if (attackCurrentTime > attackTime)
                {
                    int rand = UnityEngine.Random.Range(0, 3); //0823
                    anim.SetInteger("Attack", rand); // ��������
                    //anim.SetInteger("Attack", 0); // 0826 test
                    //anim.SetTrigger("Attack");
                    //swordSound.Play(); //0821
                    
                    // ����Ʈ!!!!
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
                    //LHE_test_PlayerHP.Instance.AddDamage(1); // ** ���� �� ��� ����
                    //print("attackanimtime " + anim.GetCurrentAnimatorStateInfo(0));
                    //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    //{
                    attackCurrentTime = 0;
                    //}
                }
            }
        //}

        // �÷��̾�� ���� ü���� ���� ���� Ư�� ��ġ �̻����� ������ ����
        if (LHE_EnemyHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap)
        {
            anim.SetTrigger("Run");
            state = State.Run;
            //print("runrunrunrururnrunrunrurnrunrurnrunrunrunru");
        }
    }

    // ���� ����Ʈ Ÿ�̹� �̺�Ʈ �Լ�
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

    // ������ ĸ�� �� ���� �Ǹ��� Į�� ����ߴ� �ڷ�ƾ
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

        // �ʱ� ��ġ�� ����
        // *** ���� �ʱ� ��ġ ���� ��ũ�� ���� Ư�� ��ġ�� ����??
        Vector3 dir = originPosition - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); //
                                                                                                                    // 0806 ��ü ȸ�� ����
        dir.Normalize();

        //transform.position = Vector3.Lerp(transform.position, originPosition, runSpeed * Time.deltaTime);
        cc.SimpleMove(dir * runSpeed); // 0806 RUN ��� ����(transform -> simplemove)

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

        // [Patrol]: ó�� Idle���ٴ� ���� ������
        // Idle ���� ���Խ��� ù��° ��ġ�� �������� ��濡 ������ ���, �� ��ġ���� ��ȸ�ϵ���
        // cc.SImpleMove ����� ��(�浹ó�� �ʿ�)
        //Vector3 originPos = transform.position; // �̷��� �ϸ� ���� ��� ������Ʈ�ȴ�
        Vector3 pos1 = originPosition + 10 * Vector3.forward;
        Vector3 pos2 = originPosition + 10 * Vector3.right;
        Vector3 pos3 = originPosition - 10 * Vector3.forward;
        Vector3 pos4 = originPosition - 10 * Vector3.right;
        Vector3[] patrolPos = { pos1, pos2, pos3, pos4 };

        float distToPos = Vector3.Distance(transform.position, patrolPos[current]);
        if (distToPos > 0.1f) // ���� ��ǥ�� �ϴ� �������� �Ÿ��� 0.1 �ʰ���
        {
            // �̵�
            transform.position = Vector3.MoveTowards(transform.position, patrolPos[current], idleSpeed * Time.deltaTime);

            // ��ü ȸ��
            Vector3 dir = patrolPos[current] - transform.position;
            dir.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);
        }
        else // ���� ��ǥ�� �ϴ� �������� �Ÿ��� 0.1 ���ϸ�
        {
            transform.position = patrolPos[current];
            //current = (current + 1) % patrolPos.Length; // pos1 2 3 4 ������ �̵�
            current = UnityEngine.Random.Range(0, 4); // ���� ���� �̵�
        }

        // (Case 1) ���� �ð��� �帣�� ��
        // �� �� ���� �Ŀ��� �÷��̾ ���ݰŸ� ������ ���� ���� ���� ����
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startAttackDist)
        {
            state = State.AfterRunAttack;
        }

        // (Case 2) ���� �ð��� �帥 ��
        // ���� �ð��� �귯�� player�� �ݰ� �� ������ ������ player�� ã�� ���� Move
        currentTime += Time.deltaTime;
        if(currentTime > afterRunIdleTime)
        {
            //state = State.Move;// Move�� ��ȯ�ϸ� �Դٰ� ü�� ���ϰ� �ٽ� ��������,, �ҿ� X
            anim.SetTrigger("AfterRunMove");
            state = State.AfterRunMove;
            currentTime = 0;
        }
    }

    private void StateAfterRunMove()
    {

        // �Ϲ� Move�� �����ѵ� �����Ÿ� Dash�� ����(�ʿ��ϸ� ������..)
        // �����Ÿ���ŭ �ɾ�� (���� ��)�����Ÿ���ŭ�� Dash�� ������ �̵� // �Ϲ�Move
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // 0821
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd ���� ����� Simplemove�� ����

        // ���� ��ȯ
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startAttackDist)
        {
            state = State.AfterRunAttack;
        }
    }

    public float afterRunAttackCurrentTime = 2.5f;
    private void StateAfterRunAttack()
    {
        // �� �� ���� �� ����: �÷��̾ ���ݰŸ� �ȿ� ������ ����, ������ Idle
        // ******* 0807 ���� (1) �þ߰� check �߰��ϰ� (2) �ǵ��ư���� �ڵ����ִٰ� �� �� ������ �ڵ��ƺ��°ŷ� ����
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
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����

            afterRunAttackCurrentTime += Time.deltaTime;
            if (afterRunAttackCurrentTime > attackTime)
            {
                int rand = UnityEngine.Random.Range(0, 3);
                anim.SetInteger("AfterRunAttack", rand);
                //swordSound.Play(); //0821


                //StartCoroutine("IeSword");
                //LHE_test_PlayerHP.Instance.AddDamage(1); // ** ���� �� ��� ����
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
        // NavMeshAgent ��Ȱ��ȭ -> �����ָ� Climb���¿��� ���� �������µ�
        //agent.enabled = false;

        StartCoroutine("IeWire");
    }

    public float wireWaitTime = 3f;
    IEnumerator IeWire()
    {
        // �÷��̾��� hitinfo.point�� �����´�
        Vector3 playerHitPoint = YSMPlayerAttack.Instance.wireHitInfo.point;
        // �÷��̾��� hitinfo.normal�� �����´� // 0809 ����ȭ
        Vector3 playerHitNormal = YSMPlayerAttack.Instance.wireHitInfo.normal;
        // normal ���̰�1�� �������͸� ���Ѵ�
        Vector3 playerHitCross = Vector3.Cross(playerHitNormal, Vector3.up).normalized;

        // 0806 Player Wire�� ���� �ƴ� ������ �۵��ϴ� ��Ȳ ����� if-else�� �߰�
        // ***** ���� tag ���� ���� ray �������� Wall�� ���� player Wire �����ϰ� ���� �ʿ�
        if (playerHitPoint.y >= 1) // 0811 �̰� ������ �����°ǰ�,, test�뵵�� ����
        {
            // enemy�� Wire �������� ����
            // = playerHitPoint���� ���������� 2��ŭ, ������ 1��ŭ ������ ����
            Vector3 enemyWirePosition = playerHitPoint + (playerHitCross * 2f + playerHitNormal);

            // �����ð� ��ٸ�
            yield return new WaitForSeconds(wireWaitTime);

            // �����ð� ��ٸ� �� ���� ���̾�׼� ���ϱ� ������ �ִϸ��̼� �� Ÿ�ֿ̹� ���
            anim.SetTrigger("Wire");

            // ��ü ȸ��
            transform.rotation = Quaternion.LookRotation(-YSMPlayerAttack.Instance.wireHitInfo.normal);

            // �̵�
            transform.position = Vector3.Lerp(transform.position, enemyWirePosition, wireSpeed * Time.fixedDeltaTime); // 0812 deltaTime���� �ٲٴ� ���� �� ���غ��δ�....

            // Climb ���·� ��ȯ
            float distance = Vector3.Distance(transform.position, enemyWirePosition);
            //print("NEARenemy-enemyWirePosition dist : " + distance);
            if (distance < 1f) // ******* 0812 �̸� ���� �� �÷��� ������... ��ü ȸ�������ΰ�??
            {
                transform.position = enemyWirePosition;
                anim.SetTrigger("Climb");
                state = State.Climb;
            }
        }
    }

    [Header("[ Climb Settings ]")]
    public float wallHeight = 10; // ****** ��� XXXXXX
    public float climbSpeed = 1;
    Vector3 walkEndPos;
    private void StateClimb()
    {

        // ĳ������ position.y���� ���� wallheight�� ���� ��������Ҽ���
        if (transform.position.y <= rayWallHeight * 2)
        {
            //transform.position += transform.up * moveSpeed * Time.fixedDeltaTime; // wire�߸����� �� enemy�� �̻��� �������� ������ �Ӹ��� �ö󰡴� ����
            transform.position += transform.up * climbSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // �ȱ� �������� ������ �̸� ���صΰ�
            //walkStartPos = transform.position;
            walkEndPos = transform.position + transform.forward * 2f;
            
            // ���� ����
            anim.SetTrigger("ClimbEndWalk");
            state = State.ClimbEndWalk;
            //StartCoroutine("ClimbEndWalk");
        }
    }

   /* //float climbEndRayDist;
    IEnumerator ClimbEndWalk() // ��� �� ��
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
                // ���º��� // 0811: Idle ->  Move�� ����(idle�ϸ� �þ߰� ������ �����ڸ��� ��������� ���� �߻�)
                // Move�� �ϴ� player�� �� �� ź ���¿����� �Ʒ��� ����������.. AfterClimbIdle ���¸� ���� �ȱ�??
                state = State.Idle;

                // NVA �ٽ� Ȱ��ȭ
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

        // [climb�� ���� �� �չ������� 2��ŭ ���ư���, ���� ���¸� AfterClimbIdle�� ��ȯ�ϰ�ʹ�]

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

        // (1) player�� �� �������� �ö�´ٸ� �þ߰� üũ ���ϰ� Move
        if ((player.transform.position.y > (transform.position.y - 0.1f)) && (player.transform.position.y < (transform.position.y + 2f)))
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }

        // (2) player�� �ö���ٰ� �ٴ����� ���ٸ� �ٽ� ����
        // => �̰͵� �ᱹ �����ð����� �Ÿ� �ȿ� �ȵ��� Move�� ��ȯ�ϸ� �ɵ�??
        currentTime += Time.deltaTime;
        if (currentTime > afterClimbIdleTime)
        {
            //state = State.Move;// Move�� ��ȯ�ϸ� �Դٰ� ü�� ���ϰ� �ٽ� ��������,, �ҿ� X
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
