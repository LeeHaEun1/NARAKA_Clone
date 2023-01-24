using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [AI Enemy(2)] : ���Ÿ� ���� enemy
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
    bool playerInSight; // �þ߰� üũ
    bool canAttack; // ���� ���� ����
    bool isDead; // ��� ����

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
        // �¿� ���� 180��
        playerInSight = CheckPlayerAngle(player.transform.position);
        // Draw: ���� 20�� �¿� �þ߰� �Ѱ� ǥ��
        //Debug.DrawRay(transform.position, 20 * new Vector3(Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);
        //Debug.DrawRay(transform.position, 20 * new Vector3(-Mathf.Sin(sightDegree * Mathf.Deg2Rad), 0, Mathf.Cos(sightDegree * Mathf.Deg2Rad)), Color.yellow);


        // [Wire]
        // � �����̵� Player�� Wire �̵��ϸ� ��ó ������ ���� Wire Action
        // *** Run�� afterRun ���µ��� �����ұ�..??
        if (YSMPlayerAttack.Instance.enemyCanWire == true && YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.tag == "Wall")
        {
            rayWallHeight = YSMPlayerAttack.Instance.wireHitInfo.collider.gameObject.transform.position.y; // �θ��� y
            state = State.Wire;
        }


        // [Die]
        // � �����̵� ü�� 0�Ǹ� Die
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


    // [�ǰ�]
    // � �����̵� Player�� �����ϸ� �ڷ� �ణ �з����� �ǰ� ȿ��
    // *** ���� LHE_EnemyHP�� AddDamage�� ȣ��Ǵ� �������� ������ ���� ����??
    // *** ���� ���ϴ� ���ȵ� �Ÿ� ������ Attack ���¶� �з��� ���� �ٽ� Player�������� ���ƿ´�
    //     -> ü���� 3�̻� ���̳��� �Ǹ� �� ���ƿ�
    [Header("[ Attacked Settings ]")]
    public float attackedSpeed = 5;
    public float attackedTime = 5;
    public float attackedDist = 5;
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

    // [�ǰ�2] �� 50�� Ʋ�����ٰ� �ٽ� �÷��̾� �ٶ󺸱�
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

    // [�ǰ�3] ������ ���� ��� ������ �ڷ� ���󰡱�
    Vector3 CurrPos;
    Vector3 HighPos;
    Vector3 EndPos;
    private void StateAttacked3()
    {
        // ************dir = enemy��ġ- player��ġ ���ϰ� dir.y=0 �� �������� ���� // 0810
        // [�ǰ� ���� ���ϱ�]
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

    IEnumerator IeFly() // �ǰ�3
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

    // ������ ���� ������ �ٸ� ����!!
    //public float attacked3Value = 1;
    // �ǰ�3 ������
    Vector3 Bezier(Vector3 CurrPos, Vector3 HighPos, Vector3 EndPos)
    {
        Vector3 p1 = Vector3.Lerp(CurrPos, HighPos, 1);
        Vector3 p2 = Vector3.Lerp(HighPos, EndPos, 1);
        return Vector3.Lerp(p1, p2, 0.5f);
    }



    [Header("[ Idle Settings ]")]
    public float idleSpeed = 1;
    int current = 0; // patrolPos �迭 indexw
    [Header("[ Move Settings ]")]
    public float startMoveDist = 25;
    private void StateIdle()
    {
        // [Patrol]
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

        // �÷��̾�� �Ÿ� ���� Ư�� ��ġ �̸��̸� Move ����
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (playerInSight && distance < startMoveDist)
        {
            anim.SetTrigger("Move");
            state = State.Move;
        }
    }

    public float moveSpeed = 1;
    [Header("[ Dash Settings ]")]
    public float startDashDist = 20; // 5��ŭ Move 5��ŭ Dash
    private void StateMove()
    {
        // �����Ÿ���ŭ �ɾ�� (���� ��)�����Ÿ���ŭ�� Dash�� ������ �̵�
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ����
        //transform.forward = dir; // ��ü ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
        cc.SimpleMove(dir * moveSpeed); // 0807 ClimbEnd ���� ����� Simplemove�� ����

        // ���� ��ȯ
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < startDashDist)
        {
            anim.SetTrigger("Dash");
            state = State.Dash;
        }
    }

    public float dashSpeed = 10;
    [Header("[ Attack Settings ]")]
    public float startAttackDist = 15; // *** ���� �ݶ��̴� ������ ��ģ�� + 0.5f ������ ����
    private void StateDash()
    {
        // Dash�� �÷��̾�� �̵��ϴ� �Ÿ� 15�Ǹ� ���� ����

        // (1) ��ü ȸ��
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0821
        //transform.forward = dir; // ��ü ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����

        // Dash
        //transform.position = Vector3.Lerp(transform.position, player.transform.position, dashSpeed * Time.deltaTime);
        cc.SimpleMove(dir * dashSpeed);

        // ���� ��ȯ
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

            // �÷��̾ ���Ÿ� ���ݰŸ�(15) ����� �ٽ� �÷��̾�Է� Dash
            if (distance > startAttackDist)
            {
                anim.SetTrigger("Dash");
                state = State.Dash;
            }
            // ���Ÿ� ���ݰŸ�(15) ���ο� �ִٸ� && ���� ���� ���¶�� ����
            else if (canAttack == true)
            {
                // *******************************
                // ���⿡�� anim �ϳ� �־���� �� �� ���� �ѵ�...
                currentTime += Time.deltaTime;
                if(currentTime > 0.1f)
                {
                    anim.SetTrigger("AttackWait");
                    currentTime = 0;
                }


                // �÷��̾� �������� enemy ��ü ���� ȸ��               
                //Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                //transform.rotation = rotation;
                // (1) ��ü ȸ��
                Vector3 dir = player.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0821
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
                // ȸ���ϸ鼭 �ٴ����� �İ��� ���� �ذ�
                // ********** �ܼ� ���ڰ� �ƴ� ray�� �̿��� (�ٴڳ��� + 1)�� ����!!!
                //transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                attackCurrentTime += Time.deltaTime;
                if (attackCurrentTime > attackTime)
                {
                    anim.SetTrigger("Attack");
                    attackCurrentTime = 0;

                    // 0817 �̺�Ʈ�Լ��� �̵�
                    /*// attackTime�� �Ǹ� ȭ�� ����
                    // ** arrow��ü���� AddDamage�ϴ� �������� ����..??
                    GameObject arrow = Instantiate(arrowFactory); 

                    // ȭ�� �߻� ��ġ�� forward�� player ���ϰ� ȸ��
                    Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
                    arrowPosition.transform.forward = dir2;

                    // ȭ��/�ѱ� ��ġ�� ����
                    arrow.transform.position = arrowPosition.transform.position;
                    arrow.transform.forward = arrowPosition.transform.forward;*/

                    // Ȱ/���� �� �� enemy ��¦ �з����ٰ� ���ƿ��� ȿ��(�ݵ�)
                    //StartCoroutine("IeRecoil");

                    // *** ȭ��/������ �´� ���� ������ ��ƾ��� -> enemyArrow ��ü���� ����
                    //LHE_test_PlayerHP.Instance.AddDamage(1);
                }
            }
        }

        // �÷��̾�� ���� ü���� ���� ���� Ư�� ��ġ �̻����� ������ ����
        if (LHE_EnemyFarHP.Instance.HP < LHE_test_PlayerHP.Instance.HP - runHPGap && LHE_EnemyFarHP.Instance.HP > 0)
        {
            anim.SetTrigger("Run");
            state = State.Run;
        }
    }

    // 0817 ȭ�� �߻� �̺�Ʈ �Լ�
    internal void OnArrowAttack()
    {
        //print("arrowarrowarrowarrowarrowarrow");
        // attackTime�� �Ǹ� ȭ�� ����
        // ** arrow��ü���� AddDamage�ϴ� �������� ����..??
        GameObject arrow = Instantiate(arrowFactory);
        arrowSound.Play(); //0821

        // ȭ�� �߻� ��ġ�� forward�� player ���ϰ� ȸ��
        Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
        arrowPosition.transform.forward = dir2;

        // ȭ��/�ѱ� ��ġ�� ����
        arrow.transform.position = arrowPosition.transform.position;
        arrow.transform.forward = arrowPosition.transform.forward;

        // �߻� ����Ʈ
        GameObject arrowEffect = Instantiate(arrowFireEffect);
        arrowEffect.transform.position = arrowPosition.transform.position;
        arrowEffect.transform.forward = arrowPosition.transform.forward;
        Destroy(arrowEffect, 10);

        StartCoroutine("IeRecoil");
    }

    // ��/���� �߻� �� enemy ��ü �ݵ� ȿ��
    IEnumerator IeRecoil()
    {
        // �з���
        transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * 1f, 5 * Time.fixedDeltaTime);

        yield return new WaitForSeconds(0.1f);

        // ���ƿ�
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 1f, 1 * Time.fixedDeltaTime);
    }
    

    public float runSpeed = 8;
    private void StateRun()
    {

        // �ʱ� ��ġ�� ����
        // *** ���� �ʱ� ��ġ ���� ��ũ�� ���� Ư�� ��ġ�� ����??
        Vector3 dir = originPosition - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
        dir.Normalize();

        //transform.position = Vector3.Lerp(transform.position, originPosition, runSpeed * Time.deltaTime);
        cc.SimpleMove(dir * runSpeed); // 0806 RUN ��� ����(transform -> simplemove)

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
        if (currentTime > afterRunIdleTime)
        {
            //state = State.Move;// Move�� ��ȯ�ϸ� �Դٰ� ü�� ���ϰ� �ٽ� ��������,, �ҿ� X
            state = State.AfterRunMove;
            anim.SetTrigger("AfterRunMove");
            currentTime = 0;
        }
    }

    private void StateAfterRunMove()
    {

        // �Ϲ� Move�� �����ѵ� �����Ÿ� Dash�� ����(�ʿ��ϸ� ������..)
        // �����Ÿ���ŭ �ɾ�� (���� ��)�����Ÿ���ŭ�� Dash�� ������ �̵� // �Ϲ�Move
        Vector3 dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0821
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
        float distance = Vector3.Distance(transform.position, player.transform.position);

        currentTime += Time.deltaTime;
        if (currentTime > 0.1f)
        {
            anim.SetTrigger("AfterRunAttackWait");
            currentTime = 0;
        }

        if (distance < startAttackDist && canAttack == true)
        {
            // �÷��̾� �������� enemy ��ü ���� ȸ��               
            //Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            //transform.rotation = rotation;
            Vector3 dir = player.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0; // �÷��̾� ���� �� enemy �������� �ߴ� ���� ���� // 0821
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime); // 0806 ��ü ȸ�� ����
            // ȸ���ϸ鼭 �ٴ����� �İ��� ���� �ذ�
            // ********** �ܼ� ���ڰ� �ƴ� ray�� �̿��� (�ٴڳ��� + 1)�� ����!!!
            //transform.position = new Vector3(transform.position.x, 1, transform.position.z);


            afterRunAttackCurrentTime += Time.deltaTime;
            if (afterRunAttackCurrentTime > attackTime)
            {
                anim.SetTrigger("AfterRunAttack");

                //LHE_test_PlayerHP.Instance.AddDamage(1); // -> enemyArrow ��ü���� ����

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
        if (playerHitPoint.y >= 1)
        {
            // enemy�� Wire �������� ����
            // = playerHitPoint���� �������� 2��ŭ, ������ 1��ŭ ������ ����
            // (�ٰŸ� enemy�� ������ 2������ ���� ������ ��ġ�� �ʱ� ���� ���� 2��������)
            Vector3 enemyWirePosition = playerHitPoint + (playerHitCross * -2f + playerHitNormal);

            // �����ð� ��ٸ�
            yield return new WaitForSeconds(wireWaitTime);

            // �����ð� ��ٸ� �� ���� ���̾�׼� ���ϱ� ������ �ִϸ��̼� �� Ÿ�ֿ̹� ���
            anim.SetTrigger("Wire");

            // ��ü ȸ��
            transform.rotation = Quaternion.LookRotation(-YSMPlayerAttack.Instance.wireHitInfo.normal);

            // �̵�
            transform.position = Vector3.Lerp(transform.position, enemyWirePosition, wireSpeed * Time.fixedDeltaTime);

           
            
            // Climb ���·� ��ȯ
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
    public float wallHeight = 10; // ******* ���� hitinfo���� �޾ƿ� ������ ����!!(��� ������ ���� ������ �� �־�� ��)
    public float climbSpeed = 0.5f;
    Vector3 walkEndPos;
    private void StateClimb()
    {
        // *********** ���� �ִϸ��̼� Ÿ�̹� �̼� ����

        // 0817 �ö󰡴� �ܰ踦 �� �� �� ������ ���ܿ��� ��� �ö󰡴°� �־��ٱ�??
        // ĳ������ position.y���� ���� wallheight�� ���� ��������Ҽ���
        if (transform.position.y <= rayWallHeight * 2)
        {
            transform.position += transform.up * climbSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // �ȱ� �������� ������ �̸� ���صΰ�
            walkEndPos = transform.position + transform.forward * 2f;

            // ���� ����
            anim.SetTrigger("ClimbEndWalk");
            state = State.ClimbEndWalk;
        }
    }

/*    IEnumerator ClimbEndWalk() // ��� �� ��
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
                // ���º���
                //state = State.Idle;

                // NVA �ٽ� Ȱ��ȭ
                //agent.enabled = true;

                yield break;
            }
        }
    }*/

    private void StateClimbEndWalk()
    {
        // [climb�� ���� �� �չ������� 2��ŭ ���ư���, ���� ���¸� AfterClimbIdle�� ��ȯ�ϰ�ʹ�]


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

        // (1) player�� �� �������� �ö�´ٸ� �þ߰� üũ ���ϰ� Move
        if ((player.transform.position.y > (transform.position.y - 1.2f)) && (player.transform.position.y < (transform.position.y + 1.2f)))
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
