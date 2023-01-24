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

    // �ʿ�Ӽ� : ȭ�����, ȭ�� Ǯ, ȭ�� ����
    public GameObject arrowFactory; // ȭ�� ����
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

        wire // [LHE 0803] AI enemy���� player ���� ���� ���� �߰�
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
        // enemy �ǰ�ȿ�� �׽�Ʈ������ �߰� -> �Ÿ���� �߰��ؾ���!!
        // ���Ÿ�/�ٰŸ� ���ݿ� ���� hit���γ�, ���ݰŸ� ���ο� �ִ��� ���� üũ�ϴ� �� �߰� �ʿ�
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            LHE_EnemyHP.Instance.AddDamage3(1);

            //LHE_EnemyFarHP.Instance.AddDamage(1);
        }
    }

    Vector3 currentPos;
    void Wire()
    {
        // ���̾� qŬ���� ī�޶� Ȯ�� ���
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
            // AI enemy���� player ���� ���� ���� �߰�
            attackState = AttackState.wire;


            currentPos = transform.position;
            transform.rotation = Quaternion.LookRotation(-hitInpo.normal);

            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;
            //StartCoroutine("IE_CameraMoveDelay");

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // ���̾ ��� ���� �¾Ҵٸ�
            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                //���߿� ������
                if (currentPos.y >= 1)
                {
                    transform.position = currentPos;
                    // �׻��¸� �����ϰ� �߷��� ��Ȱ���ϰ�ʹ�.
                    YSMPlayerMovement.Instance.canGravity = false; //�߷� ��Ȱ��
                }
                else // ���� ������
                {

                }
                // ���̾� ���� ���忡�� ���̾ ������ �� ��ġ�� ������ ���� �ʹ�.
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position + offset;
                // �浹ü���� �̵� Wire�ڷ�ƾ ���
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
            YSMPlayerMovement.Instance.canGravity = false; //�߷� ��Ȱ��


            // [LHE 0806]
            // �÷��̾� state�� wire�� �ӹ��������� enemy���µ� �����ؼ� wire�� ���ϴ� ���� �߻�
            // -> �ذ� ���� wire trigger�Ǹ� attackstate �ٷ� �������ֱ�
            attackState = AttackState.basicAttack;


            if (curTime < 1)
            {
                curTime += Time.deltaTime;

                //transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * 10);
                cc.Move((hitInpo.point - transform.position) * 5 * Time.deltaTime);

                // *** 0806 ���� ���� ���� �پ ���º��� �ȵ�..
                // [LHE 0803]
                // �÷��̾� state�� wire�� �ӹ��������� enemy���µ� �����ؼ� wire�� ���ϴ� ���� �߻�
                // -> �ذ� ���� �÷��̾ hitpoint �����ϸ� attackstate�������ֱ�
                //float distance = Vector3.Distance(transform.position, hitInpo.point);
                //if (distance < 0.1f)
                //{
                //    transform.position = hitInpo.point;
                //    attackState = AttackState.basicAttack; // [LHE 0805] arrow -> basicAttack
                //}


                // ���̾� �� �������� ȸ���ϰ� �ʹ�.
                yield return null;
            }
            else
            {
                YSMPlayerMovement.Instance.canGravity = true; // �߷� Ȱ��

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
                // �������� �̵� �ڷ�ƾ 
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
                //������ �� slowCam = false ������ (�����ʿ�)
                YSMCameraMovement.Instance.slowCam = true;

                curTime = 0;

                yield break;
            }
        }
    }

    void Bow()
    {
        // ���� ���� ��ư�� ������ ȭ���� ������ �ʹ�.
        // -> 
        // ���� ��ư�� ������ ī�޶� ������ �̵���Ű�� �ʹ�.

        if (Input.GetButtonDown("Fire1"))
        {
            // ī�޶� ������ �̵���Ű��
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            bowAim.SetActive(true);
            basicAim.SetActive(false);

            //anim.SetTrigger("Bow");
        }
        if (Input.GetButton("Fire1"))
        {
            transform.forward = Camera.main.transform.forward;
        }
        // ���� ���� ��ư�� ���� ���� ��ġ�� ���ư��� �ʹ�
        // ȭ���� ������ �ϰ�ʹ�.
        if (Input.GetButtonUp("Fire1"))
        {
            bowAim.SetActive(false);
            basicAim.SetActive(true);
            // ī�޶� ���� ��ġ
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            /* // ȭ���� �����ؼ� �� ����ġ�� ����ʹ�.
             GameObject arrow = Instantiate(arrowFactory);
             arrow.transform.position = transform.position;*/

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                arrowImpact.position = hitInfo.point;
                // ������ Ƣ�� ������ �ε��� ������ ���ϴ� ����� ��ġ��������.
                arrowImpact.forward = hitInfo.normal;
                arrowPS.Stop();
                arrowPS.Play();
            }
        }
    }
}
