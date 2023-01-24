using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI; // LHE 0828 : [���� �ϴ� ���� ���� UI]



public class YSMPlayerAttack : MonoBehaviour
{
    Rigidbody rd;
    
    List<GameObject> list = new List<GameObject>();
    private float currentHitDistance;

    private RaycastHit attackHitInfo;
    public RaycastHit wireHitInfo;
    public GameObject farEnemy;
    private RaycastHit ultimateHitInfo;

    public Vector3 offset;

    private float curTime = 0;
    // private float wireSmoothness = 5;

    private bool canWire = false;
    private bool firstQ = true;
    private bool secondQ = false;
    private bool swordControll = true;
    private bool bowControll = true;
    private bool ultimateState;

    public bool damageCont1 = false;
    public bool damageCont2 = false;
    public bool damageCont3 = false;


    private CharacterController cc;
    private Animator anim;
    private AudioSource bowSound;
    public Camera _camera;

    // �ʿ�Ӽ� : ȭ�����, ȭ�� Ǯ, ȭ�� ����
    public GameObject arrowFactory; // ȭ�� ����
    public GameObject wireFactory;

    //public Transform bowFirePos;
    public Transform arrowImpact;
    public GameObject JumpAttackImpactFactory;
    //public GameObject BowAttackImpactFactory;

    public GameObject attackImpactFactory1;
    public GameObject attackImpactFactory2;
    public GameObject attackImpactFactory3;
    public GameObject attackImpactFactory4; // ���� ��ư 0.5
    public GameObject attackImpactFactory5; // ���� ��ư 1

    public GameObject energyImpactFactory1;
    public GameObject energyImpactFactory2;
    public GameObject energyImpactFactory3;
    public GameObject energyImpactFactory4;

    public GameObject swordAttackImpactFactory;

    public GameObject UltSkillFactroy;

    //public GameObject bowEnergyImpactFactory;
    ParticleSystem arrowPS;


    public GameObject bowAim;
    public GameObject basicAim;

    public GameObject weaponBow;

    // LHE 0828 : [���� �ϴ� ���� ���� UI]
    public Image arrowUI;
    public Image swordUI;

    public GameObject ultSkillImpact;
    public AttackState attackState;
    public GameObject ultBG;
    public enum AttackState
    {
        basicAttack,
        ultimate,
        bow,
        sword,
    }

    public static YSMPlayerAttack Instance;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();

        attackState = AttackState.basicAttack;
        anim = GetComponentInChildren<Animator>();

        arrowPS = arrowImpact.GetComponent<ParticleSystem>();

        bowSound = GetComponent<AudioSource>();

        rd = GetComponent<Rigidbody>();

        // LHE 0828 : [���� �ϴ� ���� ���� UI]
        arrowUI.color = new Color(1, 1, 1, 0.2f);
        swordUI.color = new Color(1, 1, 1, 0.2f);
    }

    void Update()
    {
        print("ultimateState : " + ultimateState);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            attackState = AttackState.basicAttack;
            print("basicAttack");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            attackState = AttackState.bow;

            // LHE 0828 : [���� �ϴ� ���� ���� UI]
            arrowUI.color = new Color(1, 1, 1, 1f);
            swordUI.color = new Color(1, 1, 1, 0.2f);

            DrawBow();
            print("arrow");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            attackState = AttackState.sword;
            DrawSword();
            print("sword");


            // LHE 0828 : [���� �ϴ� ���� ���� UI]
            arrowUI.color = new Color(1, 1, 1, 0.2f);
            swordUI.color = new Color(1, 1, 1, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {

            attackState = AttackState.ultimate;
            if(!cc.isGrounded)
            {
                StartCoroutine(IE_UltimateGravity());
            }
            anim.SetTrigger("ChangeUltimate");
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
                Sword();

                break;

            case AttackState.ultimate:
                UltimateSkill();
                UltimateImpact();
                Sword();

                break;
        }

        Wire();

        originPos = Camera.main.transform.position;
        direction = Camera.main.transform.forward;
        radius = cc.height * 0.5f;

        if(Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(aaaa());
        }
    }

    IEnumerator IE_UltimateGravity()
    {
        YSMPlayerMovement.Instance.canGravity = false;
        YSMPlayerMovement.Instance.yVelocity = 0;

        yield return new WaitForSeconds(1f);

        YSMPlayerMovement.Instance.canGravity = true;
    }

    GameObject sword;
    MeshRenderer swordMesh;
    void DrawBow()
    {
        if (swordMesh.enabled == true)
            swordMesh.enabled = false;
        weaponBow.gameObject.SetActive(true);

        anim.SetTrigger("DrawBow");
    }

    void DrawSword()
    {
        //GameObject sword = transform.Find("GreatSword_01").gameObject;
        sword = GameObject.FindWithTag("Sword");
        swordMesh = sword.GetComponent<MeshRenderer>();
        swordMesh.enabled = true;
        // Ȱ ��Ȱ��ȭ.
        weaponBow.gameObject.SetActive(false);

        anim.SetTrigger("DrawSword");
    }

    float radius;

    Vector3 originPos;
    Vector3 direction;
    //RaycastHit sphereHit;
    public bool enemyCanWire = false;

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
            secondQ = true;
            swordControll = false;
            bowControll = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && secondQ == true)
        {
            // �¿츦 ���� �ʵ����ϱ� ���� bool����
            YSMPlayerMovement.Instance.attackMoveControll = true;

            print("JoomOut");

            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;
            canWire = false;
            secondQ = false;
            firstQ = true;
            swordControll = true;
            bowControll = true;

        }

        if (firstQ == false)
        {
            // �¿츦 ���� �ʵ����ϱ� ���� bool����
            YSMPlayerMovement.Instance.attackMoveControll = false;

            transform.forward = Camera.main.transform.forward;
        }
        if (Input.GetButtonUp("Fire1") && canWire == true)
        {
            //anim.SetTrigger("WireTurn");
            YSMCameraMovement.Instance.slowCam = false; // ī�޶� ������

            // �¿츦 ���� �ʵ����ϱ� ���� bool����
            YSMPlayerMovement.Instance.attackMoveControll = true;

            swordControll = true;
            bowControll = true;

            enemyCanWire = true;
            canWire = false;

            YSMPlayerMovement.Instance.jumpCount = -1;
            print("WireJumpCount : " + YSMPlayerMovement.Instance.jumpCount);

            firstQ = true;

            currentPos = transform.position;
            //transform.rotation = Quaternion.LookRotation(-wireHitInfo.normal);

            //StartCoroutine("IE_CameraMoveDelay");

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // ���̾ ��� ���� �¾Ҵٸ�
            if (Physics.Raycast(ray, out wireHitInfo, 30))
            {
                // ����ü ����
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position + offset;
                // �浹ü���� �̵� Wire�ڷ�ƾ ���

                // �÷��̾� ȸ�� �ڷ�ƾ
                StartCoroutine("IE_WireRot");
                // ī�޶� ��� ���߰� �ڷ� �ణ �ű��� ���󰡰� �Ѵ�.
                StartCoroutine("IE_CameraMoveDelay");

                transform.rotation = Quaternion.LookRotation(-wireHitInfo.normal);

                YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

                // �������� : �������� ���̶�� ���ǳֱ�
                LHE_EnemyHP enemyHP = wireHitInfo.transform.GetComponent<LHE_EnemyHP>();

                if (enemyHP)
                {
                    LHE_EnemyHP.Instance.AddDamage2(1);
                }
                LHE_EnemyFarHP enemyFarHP = wireHitInfo.transform.GetComponent<LHE_EnemyFarHP>();
                if (enemyFarHP)
                {
                    LHE_EnemyFarHP.Instance.AddDamage2(1);
                }

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

            }
            else
            {
                YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            }
            // ī�޶� ���� ������� �ӵ��� ����
            YSMCameraMovement.Instance.followSpeed = YSMCameraMovement.Instance.followSpeed * 2;
        }

    }

    IEnumerator IE_CameraMoveDelay()
    {
        YSMCameraMovement.Instance.canCmaFollow = false;
        yield return new WaitForSeconds(0.3f);
        // ī�޶� ������� �ӵ� ����
        YSMCameraMovement.Instance.followSpeed = YSMCameraMovement.Instance.followSpeed * 0.5f;
        YSMCameraMovement.Instance.canCmaFollow = true;

    }

    IEnumerator IE_Wire()
    {

        while (true)
        {
            YSMPlayerMovement.Instance.canGravity = false; //�߷� ��Ȱ��

            yield return null;

            enemyCanWire = false;
            if (Input.GetButtonDown("Jump"))
            {
                YSMPlayerMovement.Instance.canGravity = true; // �߷� Ȱ��



                curTime = 0;

                yield break;
            }

            if (curTime < 1)
            {
                curTime += Time.deltaTime;

                //transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * 10);
                cc.Move((wireHitInfo.point - transform.position) * 5 * Time.deltaTime);

                if (Input.GetButtonDown("Jump"))
                {
                    YSMPlayerMovement.Instance.canGravity = true; // �߷� Ȱ��
                    YSMCameraMovement.Instance.slowCam = true;

                    curTime = 0;

                    yield break;
                }

                // ���̾� �� �������� ȸ���ϰ� �ʹ�.
                yield return null;
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    YSMPlayerMovement.Instance.canGravity = true; // �߷� Ȱ��
                    YSMCameraMovement.Instance.slowCam = true;


                    curTime = 0;

                    yield break;
                }

                YSMPlayerMovement.Instance.canGravity = true; // �߷� Ȱ��
                YSMCameraMovement.Instance.slowCam = true;
                curTime = 0;

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

                if (Input.GetButtonDown("Jump"))
                {
                    curTime = 0;
                }

                yield return null;
            }
            else
            {
                //StopCoroutine(IE_WireRot());
                //StartCoroutine(aaaa());

                print("hitInfo : " + wireHitInfo.transform.transform);

                curTime = 0;

                yield break;
            }
        }
    }

    IEnumerator aaaa()
    {

        /*Quaternion targetRotation = Quaternion.LookRotation(transform.up);
        transform.rotation = targetRotation;
        print("targetRotation : " + targetRotation);*/

        while (true)
        {
            if (currentTime < 0.5f)
            {
                //transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(transform.up), Quaternion.LookRotation(wireHitInfo.transform.position - transform.position), Time.deltaTime);
                transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(transform.up), Quaternion.LookRotation(transform.forward), Time.deltaTime);

                yield return null;
            }
            else
            {

                //anim.SetTrigger("WireTurn");

                //transform.rotation = Quaternion.LookRotation(wireHitInfo.transform.position - transform.position);

                //transform.up = new Vector3(0, 1, 0);

                currentTime = 0;

                yield break;
            }
        }
    }

    bool ultimateImpact = true;
    public GameObject ultimateFactory;
    public GameObject ultimateFactory1;
    public GameObject ultimateFactory2;
    GameObject ultimate;
    GameObject ultimate1;
    GameObject ultimate2;

    private int ultimateSkillCount = 0;

    void UltimateImpact()
    {
        if (ultimateImpact == true)
        {
            ultimate = Instantiate(ultimateFactory);
            ultimate1 = Instantiate(ultimateFactory1);
            ultimate2 = Instantiate(ultimateFactory2);

            ultimate1.transform.position = transform.position;
            ultimate.transform.position = transform.position;

            ultimateImpact = false;
        }
        if (ultimateSkillCount <= 4)
        {
            ultimate2.transform.position = transform.position;
        }
    }
    GameObject ultSkill;

    void UltimateSkill()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            sword = GameObject.FindWithTag("Sword");
            swordMesh = sword.GetComponent<MeshRenderer>();
            swordMesh.enabled = false;
            transform.forward = Camera.main.transform.forward;

            YSMPlayerMovement.Instance.canGravity = false;
            YSMPlayerMovement.Instance.yVelocity = 0;
            YSMPlayerMovement.Instance.dir = new Vector3(0, 0, 0);
            YSMPlayerMovement.Instance.canMove = false;

            StartCoroutine("IE_UltimateSkill");

            /*if (Physics.Raycast(ray, out ultimateHitInfo, 20))
            {
                // �������� �̵� �ڷ�ƾ 
                YSMPlayerMovement.Instance.canGravity = false;
                StartCoroutine("IE_UltimateSkill");

            }
            else
            {
                YSMPlayerMovement.Instance.canGravity = false;

                StartCoroutine("IE_UltimateSkill");
            }*/
        }
    }
    bool detectEnemy;
    //public GameObject farEnemy;
    //float ultSmoothness = 10;
    IEnumerator IE_UltimateSkill()
    {
        ultSkillImpact.SetActive(true);

        print(ultBG.name);
        ultBG.SetActive(true);

        detectEnemy = true;
        ultimateState = true;
        // �浹�Ǳ� ������ ������ ũ��
        cc.center = new Vector3(0, 1, 0);
        cc.radius = 2;

        anim.SetTrigger("Ultimate");
        yield return new WaitForSeconds(0.7f);

        while (true)
        {
            if (curTime < 0.4f)
            {
                curTime += Time.deltaTime;
                cc.Move(transform.forward * 50 * Time.deltaTime);

                yield return null;
            }
            else
            {
                ultBG.SetActive(false);

                cc.radius = 0.5f;
                cc.center = new Vector3(0, 0, 0);

                yield return new WaitForSeconds(0.1f);
                ultimateState = false;
                YSMPlayerMovement.Instance.canMove = true;
                YSMPlayerMovement.Instance.canGravity = true;

                curTime = 0;
                swordMesh.enabled = true;

                ultSkillImpact.SetActive(false);

                yield break;
            }
        }
    }
    public GameObject realEnergyImpactFactory;

    void EnergyFactory()
    {
        GameObject realEnergy = Instantiate(realEnergyImpactFactory);
        /*GameObject energy1 = Instantiate(energyImpactFactory1);
        GameObject energy2 = Instantiate(energyImpactFactory2);
        GameObject energy3 = Instantiate(energyImpactFactory3);
        GameObject energy4 = Instantiate(energyImpactFactory4);
        
        energy1.transform.position = transform.position;
        energy2.transform.position = transform.position;
        energy3.transform.position = transform.position;
        energy4.transform.position = transform.position;*/
        realEnergy.transform.position = transform.position;
        energyTime = 0;
    }

    public GameObject bowImapctUI;

    void Bow()
    {
        // ���� ���� ��ư�� ������ ȭ���� ������ �ʹ�.
        // -> 
        // ���� ��ư�� ������ ī�޶� ������ �̵���Ű�� �ʹ�.
        /*if (Input.GetButtonDown("Fire1"))
        {
            attackok = true;
        }

        if (Input.GetButton("Fire1") && bowControll)
        {
            //attackok = false;
            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;
            print("currentTime : " + currentTime);
            print("energyTime : " + energyTime);
            //EnergyFactory();

            if (energyTime > 1f)
            {
                EnergyFactory();
            }
        }*/

        if (Input.GetButtonDown("Fire1") && bowControll)
        {
            //anim.SetTrigger("BowAttack");
            anim.Play("StartBowAttack");
            anim.SetTrigger("BowAttack");

            attackok = true;

            // ī�޶� ������ �̵���Ű��
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            bowAim.SetActive(true);
            basicAim.SetActive(false);

            //anim.SetTrigger("Bow");
        }
        if (Input.GetButton("Fire1") && bowControll)
        {
            // ȭ�� ���ݽ� �¿츦 ���� �ʵ����ϱ� ���� bool��������
            YSMPlayerMovement.Instance.attackMoveControll = false;
            transform.forward = Camera.main.transform.forward;

            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;
            print("currentTime : " + currentTime);
            print("energyTime : " + energyTime);
            //EnergyFactory();

            if (energyTime > 1f)
            {
                // UI�������� ����
                bowImapctUI.SetActive(true);
            }
            // �������� : playerMove�� dir.y =0 ���� ��������.
            // �������� : ȭ�� ���ư����� �ϱ�. -> ���忡�� �����
            //GameObject bowImpact = Instantiate(BowAttackImpactFactory);
        }
        // ���� ���� ��ư�� ���� ���� ��ġ�� ���ư��� �ʹ�
        // ȭ���� ������ �ϰ�ʹ�.
        if (Input.GetButtonUp("Fire1") && bowControll)
        {
            bowImapctUI.SetActive(false);

            anim.Play("EndBowAttack");
            anim.SetTrigger("EndBowAttack");
            bowSound.Stop();
            bowSound.Play();

            // ȭ�� ���ݽ� �¿츦 ���� �ʵ����ϱ� ���� bool��������
            YSMPlayerMovement.Instance.attackMoveControll = true;

            /*anim.SetTrigger("BowAttack");
            anim.SetTrigger("BowMove");*/

            currentTime = 0;
            energyTime = 0;
            attackok = false;

            bowAim.SetActive(false);
            basicAim.SetActive(true);
            // ī�޶� ���� ��ġ
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            /* // ȭ���� �����ؼ� �� ����ġ�� ����ʹ�.
             GameObject arrow = Instantiate(arrowFactory);
             arrow.transform.position = transform.position;*/

            int layer = 1 << gameObject.layer;

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 30, ~layer))
            {
                LHE_EnemyFarHP enemyFarHP = hitInfo.transform.GetComponent<LHE_EnemyFarHP>();
                if (enemyFarHP)
                {
                    LHE_EnemyFarHP.Instance.AddDamage2(1);
                }
                LHE_EnemyHP enemyHP = hitInfo.transform.GetComponent<LHE_EnemyHP>();
                if (enemyHP)
                {
                    LHE_EnemyHP.Instance.AddDamage2(1);
                }
                //LHE_EnemyHP.Instance.AddDamage(1);
                print(LHE_EnemyHP.Instance.hp);

                arrowImpact.position = hitInfo.point;
                // ������ Ƣ�� ������ �ε��� ������ ���ϴ� ����� ��ġ��������.
                arrowImpact.forward = hitInfo.normal;
                arrowPS.Stop();
                arrowPS.Play();
            }
        }
    }

    public GameObject enemy;
    public float attackDistance = 5;
    public Vector3 myPos;
    float attackTime = 0;
    bool attack1 = true;
    bool attack2 = false;
    bool attackok = true;
    bool aaa = false;
    bool bbb = false;
    bool energyAnim;
    bool energyAnim1;
    float currentTime = 0;
    public float energyTime = 0.5f;

    RaycastHit hit;
    float ChargingTime;
    void Sword()
    {
        // ���� ���콺 ��ư�� �����µ� ���� �����Ÿ� ���� ���� �ִٸ� �� �ٶ󺸰� �����ϰ� �ʹ�.
        // 1. ����ڰ� ���콺 ���� ��ư�� �������ϱ�
        ChargingTime += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            ChargingTime = 0;
            bbb = true;
            attackok = true;
            energyAnim = true;
            //currnetTime11 = Time.time;

        }
        if (ChargingTime > 0.25f && bbb == true)
        {
            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;
            if (energyAnim == true)
            {
                anim.SetTrigger("LeftCharging");
            }
            energyAnim = false;
            if (energyTime > 1f)
            {
                EnergyFactory();
                print("Fire1111");
            }
        }
        // ������� ����Ʈ
        /*if (Input.GetButton("Fire1") && swordControll)
        {
            //attackok = false;
            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;

            anim.SetTrigger("LeftCharging");
            print("!!!!!!!!!!!!!!!!!");

            if (energyTime > 1f)
            {
                EnergyFactory();
            }
        }*/

        if (Input.GetButtonUp("Fire1"))
        {
            print("crrentTime : " + currentTime);
            ChargingTime = 0;
            bbb = false;
            if (swordControll)
            {
                energyTime = 0;
                // 1�� �������

                if (currentTime > 2)
                {
                    currentTime = 0; // ��¡�ð� �ʱ�ȭ
                    //YSMAttackImpact.Instance.damageCheck = true;
                    
                    attackok = false;

                    anim.SetTrigger("LeftChargingAttack");

                    /*GameObject attackImpact = Instantiate(attackImpactFactory4);
                    attackImpact.transform.position = transform.position;
                    attackImpact.transform.forward = Camera.main.transform.forward;*/

                    // attackImpact.transform.position = transform.position + transform.up;
                    // attackImpact.transform.forward = -transform.forward;
                    // attackImpact.transform.rotation = Quaternion.AngleAxis(90, transform.forward);
                    //attackImpact.transform.rotation = Quaternion.AngleAxis(30, -transform.right);

                    YSMAttackImpact ImpactCheck = GameObject.FindWithTag("Wave").GetComponent<YSMAttackImpact>();
                    ImpactCheck.damageCheck = true;

                    print("ũ�� �������");
                }
                else if (currentTime > 1f) // 0.5�� �������
                {
                    currentTime = 0; // ��¡�ð� �ʱ�ȭ
                    
                    //YSMAttackImpact.Instance.damageCheck = true;

                    attackok = false;

                    anim.SetTrigger("LeftChargingAttack1");

                    //attackImpact.transform.forward = Camera.main.transform.forward;

                    transform.forward = Camera.main.transform.forward;

                    /*GameObject attackImpact = Instantiate(attackImpactFactory5);
                    attackImpact.transform.position = transform.position + transform.up *2;
                    attackImpact.transform.forward = transform.forward;
                    */
                    //attackImpact.transform.forward = Camera.main.transform.forward;
                    //attackImpact.transform.eulerAngles = new Vector3(0, 0, 90);
                    //attackImpact.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                    //attackImpact.transform.rotation = Quaternion.AngleAxis(90, Camera.main.transform.forward);

                    //attackImpact.transform.forward = transform.forward;
                    //attackImpact.transform.position = transform.position;

                    YSMAttackImpact ImpactCheck = GameObject.FindWithTag("Wave").GetComponent<YSMAttackImpact>();
                    ImpactCheck.damageCheck = true;

                    print("�۰� �������");
                }
                else if (attackok && currentTime <= 1)
                {
                    //StartCoroutine("IE_RightMouse1");

                    attackTime = 0;
                    // Ŭ���ϰ� �����ð� ���� �� Ŭ���Ѵٸ� ���� ������ �ϰ�ʹ�,.
                    // �׷��� �����ð� ���� ������ ���� �ʴ´ٸ� ù��° �������� ���ư� �� �ֵ��� �ϰ� �ʹ�.

                    // ���� ���� �� ������ �������

                    if (attack1 && cc.isGrounded)
                    {
                        anim.SetTrigger("OnLeftAttackCombo");

                        print("attack1");
                        float nearEnemyDis;
                        float farEnemyDis;
                        if (GameObject.FindWithTag("EnemyNear"))
                        {
                            nearEnemyDis = Vector3.Distance(transform.position, enemy.transform.position);
                            print("nearEnemyDis : " + nearEnemyDis);
                        }
                        else
                        {
                            nearEnemyDis = 0;
                        }

                        if (GameObject.FindWithTag("EnemyFar"))
                        {
                            farEnemyDis = Vector3.Distance(transform.position, farEnemy.transform.position);
                            print("farEnemyDis : " + farEnemyDis);
                        }
                        else
                        {
                            farEnemyDis = 0;
                        }

                        if (nearEnemyDis < farEnemyDis)
                        {
                            if(GameObject.FindWithTag("EnemyNear"))
                            {
                                if(Vector3.Distance(enemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = enemy.transform.position - transform.position;
                                    print("EnemyNearAttack");
                                }
                            }
                            
                            // RayCastall ���
                            /*RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, Vector3.up, 1.5f, LayerMask.GetMask("Enemy"));

                            foreach (RaycastHit hit in hits)
                            {
                                print("Attack1 Near Ray");

                                LHE_EnemyHP enemyNearHp = hit.transform.GetComponent<LHE_EnemyHP>();

                                if (enemyNearHp)
                                {
                                    enemyNearHp.AddDamage(1);
                                    print("enemyNearHp : " + enemyNearHp.HP);
                                    list.Add(hit.transform.gameObject);
                                }

                                LHE_EnemyFarHP enemyFarHp = hit.transform.GetComponent<LHE_EnemyFarHP>();

                                if (enemyFarHp)
                                {
                                    enemyFarHp.AddDamage(1);
                                    print("enemyFarHp : " + enemyFarHp.HP);
                                    list.Add(hit.transform.gameObject);
                                }
                            }*/
                            // �������� : Enemy�Լ����� ȿ�� �����
                            /*GameObject attackImapact = Instantiate(attackImpactFactory1);
                            attackImapact.transform.position = hit.point;*/
                        }
                        else
                        {
                            if (GameObject.FindWithTag("EnemyFar"))
                            {
                                if (Vector3.Distance(farEnemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = farEnemy.transform.position - transform.position;
                                    print("EnemyFarAttack");

                                }
                            }

                            /*RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, Vector3.up, 1.5f, LayerMask.GetMask("Enemy"));

                            foreach (RaycastHit hit in hits)
                            {
                                LHE_EnemyHP enemyNearHp = hit.transform.GetComponent<LHE_EnemyHP>();

                                if (enemyNearHp)
                                {
                                    enemyNearHp.AddDamage(1);
                                    print(enemyNearHp.HP);
                                    list.Add(hit.transform.gameObject);
                                }

                                LHE_EnemyFarHP enemyFarHp = hit.transform.GetComponent<LHE_EnemyFarHP>();

                                if (enemyFarHp)
                                {
                                    enemyFarHp.AddDamage(1);
                                    print(enemyFarHp.HP);
                                    list.Add(hit.transform.gameObject);
                                }
                            }*/
                        }

                        attack1 = false;
                        attack2 = true;
                    }
                    else if (attack2 && cc.isGrounded)
                    {
                        anim.SetTrigger("OnLeftAttackCombo");

                        float nearEnemyDis;
                        float farEnemyDis;
                        if (GameObject.FindWithTag("EnemyNear"))
                        {
                            nearEnemyDis = Vector3.Distance(transform.position, enemy.transform.position);
                        }
                        else
                        {
                            nearEnemyDis = 0;
                        }

                        if (GameObject.FindWithTag("EnemyFar"))
                        {
                            farEnemyDis = Vector3.Distance(transform.position, farEnemy.transform.position);
                        }
                        else
                        {
                            farEnemyDis = 0;
                        }

                        if (nearEnemyDis < farEnemyDis)
                        {
                            if (GameObject.FindWithTag("EnemyNear"))
                            {
                                if (Vector3.Distance(enemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = enemy.transform.position - transform.position;
                                    print("EnemyNearAttack");
                                }
                            }
                            // �������� : Enemy�Լ����� ȿ�� �����
                            /*GameObject attackImapact = Instantiate(attackImpactFactory1);
                            attackImapact.transform.position = hit.point;*/
                        }
                        else
                        {
                            if (GameObject.FindWithTag("EnemyFar"))
                            {
                                if (Vector3.Distance(farEnemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = farEnemy.transform.position - transform.position;
                                    print("EnemyFarAttack");
                                }
                            }
                        }

                        attack2 = false;
                        attack1 = true;
                    }

                    // ���� ���� �ϰ� �ʹ�.
                    if (!cc.isGrounded)
                    {
                        anim.SetTrigger("SwordJumpAttack");

                        print("JumpAttack");

                        float nearEnemyDis;
                        float farEnemyDis;
                        if (GameObject.FindWithTag("EnemyNear"))
                        {
                            nearEnemyDis = Vector3.Distance(transform.position, enemy.transform.position);
                        }
                        else
                        {
                            nearEnemyDis = 0;
                        }

                        if (GameObject.FindWithTag("EnemyFar"))
                        {
                            farEnemyDis = Vector3.Distance(transform.position, farEnemy.transform.position);
                        }
                        else
                        {
                            farEnemyDis = 0;
                        }

                        if (nearEnemyDis < farEnemyDis)
                        {
                            if (GameObject.FindWithTag("EnemyNear"))
                            {
                                if (Vector3.Distance(enemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = enemy.transform.position - transform.position;
                                    print("EnemyNearAttack");
                                }
                            }
                            // �������� : Enemy�Լ����� ȿ�� �����
                            /*GameObject attackImapact = Instantiate(attackImpactFactory1);
                            attackImapact.transform.position = hit.point;*/
                        }
                        else
                        {
                            if (GameObject.FindWithTag("EnemyFar"))
                            {
                                if (Vector3.Distance(farEnemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = farEnemy.transform.position - transform.position;
                                    print("EnemyFarAttack");
                                }
                            }
                        }
                        // ���� �Ÿ� ���� �����Ѵٸ�
                        /*if (Vector3.Distance(transform.position, enemy.transform.position) < attackDistance)
                        {
                            // ���� �ٶ󺸰�
                            transform.forward = enemy.transform.position - transform.position;
                            // ���̸� �i�µ� �����Ѵٸ�
                            bool isCast = Physics.SphereCast(transform.position, 1, transform.forward, out attackHitInfo, attackDistance);

                            if (isCast)
                            {

                                LHE_EnemyHP enemyHP = attackHitInfo.transform.GetComponent<LHE_EnemyHP>();
                                if (enemyHP)
                                {
                                    enemyHP.AddDamage(1);
                                }

                                // ���۵���
                                //StartCoroutine("IE_WireRot");
                                *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                                jumpAttackImpact.transform.position = attackHitInfo.point;*//*
                            }
                        }
                        else
                        {
                            // ���۵���
                            //StartCoroutine("IE_WireRot");
                            *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                            jumpAttackImpact.transform.position = transform.position + transform.forward;*//*
                        }

                        if (Vector3.Distance(transform.position, farEnemy.transform.position) < attackDistance)
                        {
                            // ���� �ٶ󺸰�
                            transform.forward = farEnemy.transform.position - transform.position;
                            // ���̸� �i�µ� �����Ѵٸ�
                            bool isCast = Physics.SphereCast(transform.position, 1, transform.forward, out attackHitInfo, attackDistance);

                            if (isCast)
                            {
                                LHE_EnemyFarHP enemyFarHP = attackHitInfo.transform.GetComponent<LHE_EnemyFarHP>();
                                if (enemyFarHP)
                                {
                                    enemyFarHP.AddDamage(1);
                                }


                                // ���۵���
                                //StartCoroutine("IE_WireRot");
                                *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                                jumpAttackImpact.transform.position = attackHitInfo.point;*//*
                            }
                        }
                        else
                        {
                            // ���۵���
                            //StartCoroutine("IE_WireRot");
                            *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                            jumpAttackImpact.transform.position = transform.position + transform.forward;*//*
                        }*/
                    }
                }

                attackTime += Time.deltaTime;
                // 0.1�ʾȿ� ���� ���ϸ� ù��° �������� ���ƿ���
                if (attackTime > 1f)
                {
                    attack1 = true;
                    attack2 = false;
                }
            }
        }


        // ---------------------------- ���콺 ��Ŭ��--------------------------
        if (Input.GetButtonDown("Fire2"))
        {
            ChargingTime = 0;
            aaa = true;
            attackok = true;
            energyAnim1 = true;
            //currnetTime11 = Time.time;

        }
        //ChargingTime += Time.deltaTime;
        if (ChargingTime > 0.25f && aaa == true)
        {
            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;
            if (energyAnim1 == true)
            {
                anim.SetTrigger("RightCharging");
            }
            energyAnim1 = false;
            if (energyTime > 1f)
            {
                EnergyFactory();
                print("Fire2222");
            }
        }
        // ������� ����Ʈ
        /*if (Input.GetButton("Fire1") && swordControll)
        {
            //attackok = false;
            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;

            anim.SetTrigger("LeftCharging");
            print("!!!!!!!!!!!!!!!!!");

            if (energyTime > 1f)
            {
                EnergyFactory();
            }
        }*/

        if (Input.GetButtonUp("Fire2"))
        {
            ChargingTime = 0;
            aaa = false;
            if (swordControll)
            {
                energyTime = 0;
                // 1�� �������
                /*if(currentTime > 2.5f)
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack3");
                    // ������ �̵��ϴ� �Լ� �ֱ�
                    //StartCoroutine(IE_ChargingRightAttackMove1());
                    //rd.AddForce(transform.forward * 500, ForceMode.Impulse);

                    // �չ��� ī�޶� �չ���� ��ġ
                    transform.forward = Camera.main.transform.forward;
                }*/
                if (currentTime > 2f)
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack");
                    // ������ �̵��ϴ� �Լ� �ֱ�
                    StartCoroutine(IE_ChargingRightAttackMove());
                    // �չ��� ī�޶� �չ���� ��ġ
                    transform.forward = Camera.main.transform.forward;

                }
                else if (currentTime > 1f) // 0.5�� �������
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack2");
                    // ������ �̵��ϴ� �Լ� �ֱ�
                    StartCoroutine(IE_ChargingRightAttackMove2());
                    // �չ��� ī�޶� �չ���� ��ġ
                    transform.forward = Camera.main.transform.forward;

                }
                else if (attackok && currentTime <= 1)
                {
                    //StartCoroutine("IE_RightMouse1");

                    attackTime = 0;
                    // Ŭ���ϰ� �����ð� ���� �� Ŭ���Ѵٸ� ���� ������ �ϰ�ʹ�,.
                    // �׷��� �����ð� ���� ������ ���� �ʴ´ٸ� ù��° �������� ���ư� �� �ֵ��� �ϰ� �ʹ�.

                    // ���� ���� �� ������ �������

                    if (attack1 && cc.isGrounded)
                    {
                        anim.SetTrigger("OnRightAttackCombo");

                        //damageCont2 = true;

                        print("attack1");
                        float nearEnemyDis;
                        float farEnemyDis;
                        if (GameObject.FindWithTag("EnemyNear"))
                        {
                            nearEnemyDis = Vector3.Distance(transform.position, enemy.transform.position);
                            print("nearEnemyDis : " + nearEnemyDis);
                        }
                        else
                        {
                            nearEnemyDis = 0;
                        }

                        if (GameObject.FindWithTag("EnemyFar"))
                        {
                            farEnemyDis = Vector3.Distance(transform.position, farEnemy.transform.position);
                            print("farEnemyDis : " + farEnemyDis);
                        }
                        else
                        {
                            farEnemyDis = 0;
                        }

                        if (nearEnemyDis < farEnemyDis)
                        {
                            if (GameObject.FindWithTag("EnemyNear"))
                            {
                                if (Vector3.Distance(enemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = enemy.transform.position - transform.position;
                                    print("EnemyNearAttack");
                                }
                            }

                        }
                        else
                        {
                            if (GameObject.FindWithTag("EnemyFar"))
                            {
                                if (Vector3.Distance(farEnemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = farEnemy.transform.position - transform.position;
                                    print("EnemyFarAttack");

                                }
                            }
                        }

                        attack1 = false;
                        attack2 = true;
                    }
                    else if (attack2 && cc.isGrounded)
                    {
                        anim.SetTrigger("OnRightAttackCombo");

                        //damageCont1 = true;

                        float nearEnemyDis;
                        float farEnemyDis;
                        if (GameObject.FindWithTag("EnemyNear"))
                        {
                            nearEnemyDis = Vector3.Distance(transform.position, enemy.transform.position);
                            print("nearEnemyDis : " + nearEnemyDis);
                        }
                        else
                        {
                            nearEnemyDis = 0;
                        }

                        if (GameObject.FindWithTag("EnemyFar"))
                        {
                            farEnemyDis = Vector3.Distance(transform.position, farEnemy.transform.position);
                            print("farEnemyDis : " + farEnemyDis);
                        }
                        else
                        {
                            farEnemyDis = 0;
                        }

                        if (nearEnemyDis < farEnemyDis)
                        {
                            if (GameObject.FindWithTag("EnemyNear"))
                            {
                                if (Vector3.Distance(enemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = enemy.transform.position - transform.position;
                                    print("EnemyNearAttack");
                                }
                            }
                        }
                        else
                        {
                            if (GameObject.FindWithTag("EnemyFar"))
                            {
                                if (Vector3.Distance(farEnemy.transform.position, transform.position) < 2)
                                {
                                    transform.forward = farEnemy.transform.position - transform.position;
                                    print("EnemyFarAttack");
                                }
                            }
                        }

                        attack2 = false;
                        attack1 = true;
                    }
                }
            }
        }
    }

    IEnumerator IE_RightMouse1()
    {
        while (true)
        {
            if (currentTime < 1)
            {
                currentTime += Time.deltaTime;

                transform.position = Vector3.Slerp(transform.position, transform.position + transform.forward, 0.5f);

                yield return null;
            }
            else
            {
                currentTime = 0;

                yield break;
            }
        }
    }

    IEnumerator AttackSphereRayDelay()
    {
        yield return new WaitForSeconds(0.1f);
        print("11111");
        Ray ray = new Ray(transform.position, transform.forward);
        bool isCast = Physics.SphereCast(ray, radius, out attackHitInfo);
        print(attackHitInfo.transform.name);

        print("isCast : " + isCast);
        if (isCast)
        {
            print("2222");
            // -> ���ʹ� HP�� �����ϴ� ������ �Լ��� ȣ���ϰ� �ʹ�.
            YSMCameraShake.Instance.Shake();
            LHE_EnemyFarHP enemyFarHP = attackHitInfo.transform.GetComponent<LHE_EnemyFarHP>();
            if (enemyFarHP)
            {
                LHE_EnemyFarHP.Instance.AddDamage(1);
            }
            LHE_EnemyHP enemyHP = attackHitInfo.transform.GetComponent<LHE_EnemyHP>();
            if (enemyHP)
            {
                LHE_EnemyHP.Instance.AddDamage(1);
            }
            print("enemyHP : " + LHE_EnemyHP.Instance.hp);
        }
    }

    void OnDrawGizmos()
    {
        // Physics.SphereCast (�������� �߻��� ��ġ, ���� �ݰ�, �߻� ����, �浹 ���, �ִ� �Ÿ�)
        //bool isHit = Physics.SphereCast(transform.position, radius, transform.forward, out attackHitInfo, attackDistance);
        //bool isHit
        // Physics.SphereCastAll(transform.position, 3, Vector3.up);

        Gizmos.color = Color.red;
        if (currentHitDistance <= 3)
        {
            //print("1111111111");
            Gizmos.DrawWireSphere(transform.position, 3);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 3);

        }

        /*if (isHit)
        {
            Gizmos.DrawRay(transform.position, transform.forward * attackHitInfo.distance);
            Gizmos.DrawWireSphere(transform.position + transform.forward * attackHitInfo.distance, radius);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * attackDistance);
        }*/
    }
    float rightAttackTime = 1.25f;
    IEnumerator IE_ChargingRightAttackMove()
    {
        while(true)
        {
            if(currentTime < rightAttackTime)
            {
                currentTime += Time.deltaTime;

                cc.Move(Camera.main.transform.forward * 5 * Time.deltaTime);

                yield return null;
            }
            else
            {
                currentTime = 0;

                yield break;
            }
        }
    }
    float attackTime111;
    IEnumerator IE_ChargingRightAttackMove1()
    {
        while(true)
        {
            if(currentTime < rightAttackTime)
            {
                currentTime += Time.deltaTime;
                //attackTime111 = 5;
                //attackTime111 -= Time.deltaTime *10;
                cc.Move(Camera.main.transform.forward * 3 * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, attackTime111 * 0.1f);

                yield return null;
            }
            else
            {
                currentTime = 0;

                yield break;
            }
        }
    }
    IEnumerator IE_ChargingRightAttackMove2()
    {
        while(true)
        {
            if(currentTime < 1)
            {
                currentTime += Time.deltaTime;

                cc.Move(Camera.main.transform.forward * 5 * Time.deltaTime);

                yield return null;
            }
            else
            {
                currentTime = 0;

                yield break;
            }
        }
    }
    // �ʿ�Ӽ� : �ñر� ���¸� ǥ���ϴ� bool����
    // �÷��̾ �ñر⸦ Ű�� ��ų�� ����� �� ���� �ε����ٸ�
    private void OnCollisionStay(Collision collision)
    {
        if (ultimateState == true)
        {
            // �ε��� ���� ���ʹ�HP��ũ��Ʈ�� �����ͼ�
            //if(collision.gameObject.name.Contains("Enemy"))
            
            LHE_EnemyHP enemyNearHP = collision.gameObject.GetComponentInChildren<LHE_EnemyHP>();
            LHE_EnemyFarHP enemyFarHP = collision.gameObject.GetComponentInChildren<LHE_EnemyFarHP>();

            // �������Լ��� ȣ���ϰ� �ʹ�.
            if(enemyNearHP)
            {
                enemyNearHP.AddDamage(1);
                print("ultimateDamage");
            }

            if(enemyFarHP)
            {
                enemyFarHP.AddDamage(1);
                print("ultimateDamage");
            }
        }
    }
}
