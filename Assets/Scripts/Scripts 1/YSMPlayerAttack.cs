using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI; // LHE 0828 : [우측 하단 현재 무기 UI]



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

    // 필요속성 : 화살공장, 화살 풀, 화살 갯수
    public GameObject arrowFactory; // 화살 공장
    public GameObject wireFactory;

    //public Transform bowFirePos;
    public Transform arrowImpact;
    public GameObject JumpAttackImpactFactory;
    //public GameObject BowAttackImpactFactory;

    public GameObject attackImpactFactory1;
    public GameObject attackImpactFactory2;
    public GameObject attackImpactFactory3;
    public GameObject attackImpactFactory4; // 왼쪽 버튼 0.5
    public GameObject attackImpactFactory5; // 왼쪽 버튼 1

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

    // LHE 0828 : [우측 하단 현재 무기 UI]
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

        // LHE 0828 : [우측 하단 현재 무기 UI]
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

            // LHE 0828 : [우측 하단 현재 무기 UI]
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


            // LHE 0828 : [우측 하단 현재 무기 UI]
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
        // 활 비활성화.
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
        // 와이어 q클릭시 카메라 확대 축소
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
            // 좌우를 보지 않도록하기 위한 bool변수
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
            // 좌우를 보지 않도록하기 위한 bool변수
            YSMPlayerMovement.Instance.attackMoveControll = false;

            transform.forward = Camera.main.transform.forward;
        }
        if (Input.GetButtonUp("Fire1") && canWire == true)
        {
            //anim.SetTrigger("WireTurn");
            YSMCameraMovement.Instance.slowCam = false; // 카메라 느리게

            // 좌우를 보지 않도록하기 위한 bool변수
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

            // 와이어를 쏘고 만약 맞았다면
            if (Physics.Raycast(ray, out wireHitInfo, 30))
            {
                // 투사체 생성
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position + offset;
                // 충돌체에서 이동 Wire코루틴 재생

                // 플레이어 회전 코루틴
                StartCoroutine("IE_WireRot");
                // 카메라를 잠시 멈추고 뒤로 약간 옮긴후 따라가게 한다.
                StartCoroutine("IE_CameraMoveDelay");

                transform.rotation = Quaternion.LookRotation(-wireHitInfo.normal);

                YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

                // 수정사항 : 맞은것이 적이라면 조건넣기
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

            }
            else
            {
                YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            }
            // 카메라 원래 따라오는 속도로 조정
            YSMCameraMovement.Instance.followSpeed = YSMCameraMovement.Instance.followSpeed * 2;
        }

    }

    IEnumerator IE_CameraMoveDelay()
    {
        YSMCameraMovement.Instance.canCmaFollow = false;
        yield return new WaitForSeconds(0.3f);
        // 카메라 따라오는 속도 조정
        YSMCameraMovement.Instance.followSpeed = YSMCameraMovement.Instance.followSpeed * 0.5f;
        YSMCameraMovement.Instance.canCmaFollow = true;

    }

    IEnumerator IE_Wire()
    {

        while (true)
        {
            YSMPlayerMovement.Instance.canGravity = false; //중력 비활성

            yield return null;

            enemyCanWire = false;
            if (Input.GetButtonDown("Jump"))
            {
                YSMPlayerMovement.Instance.canGravity = true; // 중력 활성



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
                    YSMPlayerMovement.Instance.canGravity = true; // 중력 활성
                    YSMCameraMovement.Instance.slowCam = true;

                    curTime = 0;

                    yield break;
                }

                // 와이어 쏜 방향으로 회전하고 싶다.
                yield return null;
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    YSMPlayerMovement.Instance.canGravity = true; // 중력 활성
                    YSMCameraMovement.Instance.slowCam = true;


                    curTime = 0;

                    yield break;
                }

                YSMPlayerMovement.Instance.canGravity = true; // 중력 활성
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
                // 지점까지 이동 코루틴 
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
        // 충돌되기 쉽도록 반지름 크게
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
        // 만약 왼쪽 버튼을 누르면 화살을 날리고 싶다.
        // -> 
        // 왼쪽 버튼을 누르면 카메라를 앞으로 이동시키고 싶다.
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

            // 카메라 앞으로 이동시키기
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            bowAim.SetActive(true);
            basicAim.SetActive(false);

            //anim.SetTrigger("Bow");
        }
        if (Input.GetButton("Fire1") && bowControll)
        {
            // 화살 공격시 좌우를 보지 않도록하기 위한 bool변수제어
            YSMPlayerMovement.Instance.attackMoveControll = false;
            transform.forward = Camera.main.transform.forward;

            currentTime += Time.deltaTime;
            energyTime += Time.deltaTime;
            print("currentTime : " + currentTime);
            print("energyTime : " + energyTime);
            //EnergyFactory();

            if (energyTime > 1f)
            {
                // UI기모으기로 변경
                bowImapctUI.SetActive(true);
            }
            // 수정사항 : playerMove에 dir.y =0 으로 조정하자.
            // 수정사항 : 화살 날아가도록 하기. -> 공장에서 만들기
            //GameObject bowImpact = Instantiate(BowAttackImpactFactory);
        }
        // 만약 왼쪽 버튼을 떼면 원래 위치로 돌아가고 싶다
        // 화살이 나가게 하고싶다.
        if (Input.GetButtonUp("Fire1") && bowControll)
        {
            bowImapctUI.SetActive(false);

            anim.Play("EndBowAttack");
            anim.SetTrigger("EndBowAttack");
            bowSound.Stop();
            bowSound.Play();

            // 화살 공격시 좌우를 보지 않도록하기 위한 bool변수제어
            YSMPlayerMovement.Instance.attackMoveControll = true;

            /*anim.SetTrigger("BowAttack");
            anim.SetTrigger("BowMove");*/

            currentTime = 0;
            energyTime = 0;
            attackok = false;

            bowAim.SetActive(false);
            basicAim.SetActive(true);
            // 카메라 원래 위치
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            /* // 화살을 생성해서 내 앞위치에 놓고싶다.
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
                // 파편이 튀는 방향을 부딪힌 지점이 향하는 방향과 일치시켜주자.
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
        // 왼쪽 마우스 버튼을 눌렀는데 만약 일정거리 내에 적이 있다면 적 바라보고 공격하고 싶다.
        // 1. 사용자가 마우스 왼쪽 버튼을 눌렀으니까
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
        // 기모으기 임팩트
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
                // 1초 기모으기

                if (currentTime > 2)
                {
                    currentTime = 0; // 차징시간 초기화
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

                    print("크게 기모으기");
                }
                else if (currentTime > 1f) // 0.5초 기모으기
                {
                    currentTime = 0; // 차징시간 초기화
                    
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

                    print("작게 기모으기");
                }
                else if (attackok && currentTime <= 1)
                {
                    //StartCoroutine("IE_RightMouse1");

                    attackTime = 0;
                    // 클릭하고 일정시간 내에 또 클릭한다면 다음 공격을 하고싶다,.
                    // 그러나 일정시간 내에 공격을 하지 않는다면 첫번째 공격으로 돌아갈 수 있도록 하고 싶다.

                    // 왼쪽 공격 꾹 누르면 기모으기

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
                            
                            // RayCastall 사용
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
                            // 수정사항 : Enemy함수에서 효과 만들기
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
                            // 수정사항 : Enemy함수에서 효과 만들기
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

                    // 점프 공격 하고 싶다.
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
                            // 수정사항 : Enemy함수에서 효과 만들기
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
                        // 공격 거리 내에 존재한다면
                        /*if (Vector3.Distance(transform.position, enemy.transform.position) < attackDistance)
                        {
                            // 적을 바라보고
                            transform.forward = enemy.transform.position - transform.position;
                            // 레이를 쐇는데 존재한다면
                            bool isCast = Physics.SphereCast(transform.position, 1, transform.forward, out attackHitInfo, attackDistance);

                            if (isCast)
                            {

                                LHE_EnemyHP enemyHP = attackHitInfo.transform.GetComponent<LHE_EnemyHP>();
                                if (enemyHP)
                                {
                                    enemyHP.AddDamage(1);
                                }

                                // 빙글돌기
                                //StartCoroutine("IE_WireRot");
                                *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                                jumpAttackImpact.transform.position = attackHitInfo.point;*//*
                            }
                        }
                        else
                        {
                            // 빙글돌기
                            //StartCoroutine("IE_WireRot");
                            *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                            jumpAttackImpact.transform.position = transform.position + transform.forward;*//*
                        }

                        if (Vector3.Distance(transform.position, farEnemy.transform.position) < attackDistance)
                        {
                            // 적을 바라보고
                            transform.forward = farEnemy.transform.position - transform.position;
                            // 레이를 쐇는데 존재한다면
                            bool isCast = Physics.SphereCast(transform.position, 1, transform.forward, out attackHitInfo, attackDistance);

                            if (isCast)
                            {
                                LHE_EnemyFarHP enemyFarHP = attackHitInfo.transform.GetComponent<LHE_EnemyFarHP>();
                                if (enemyFarHP)
                                {
                                    enemyFarHP.AddDamage(1);
                                }


                                // 빙글돌기
                                //StartCoroutine("IE_WireRot");
                                *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                                jumpAttackImpact.transform.position = attackHitInfo.point;*//*
                            }
                        }
                        else
                        {
                            // 빙글돌기
                            //StartCoroutine("IE_WireRot");
                            *//*GameObject jumpAttackImpact = Instantiate(JumpAttackImpactFactory);
                            jumpAttackImpact.transform.position = transform.position + transform.forward;*//*
                        }*/
                    }
                }

                attackTime += Time.deltaTime;
                // 0.1초안에 공격 안하면 첫번째 공격으로 돌아오기
                if (attackTime > 1f)
                {
                    attack1 = true;
                    attack2 = false;
                }
            }
        }


        // ---------------------------- 마우스 우클릭--------------------------
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
        // 기모으기 임팩트
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
                // 1초 기모으기
                /*if(currentTime > 2.5f)
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack3");
                    // 앞으로 이동하는 함수 넣기
                    //StartCoroutine(IE_ChargingRightAttackMove1());
                    //rd.AddForce(transform.forward * 500, ForceMode.Impulse);

                    // 앞방향 카메라 앞방향과 일치
                    transform.forward = Camera.main.transform.forward;
                }*/
                if (currentTime > 2f)
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack");
                    // 앞으로 이동하는 함수 넣기
                    StartCoroutine(IE_ChargingRightAttackMove());
                    // 앞방향 카메라 앞방향과 일치
                    transform.forward = Camera.main.transform.forward;

                }
                else if (currentTime > 1f) // 0.5초 기모으기
                {
                    currentTime = 0;

                    attackok = false;

                    anim.SetTrigger("RightChargingAttack2");
                    // 앞으로 이동하는 함수 넣기
                    StartCoroutine(IE_ChargingRightAttackMove2());
                    // 앞방향 카메라 앞방향과 일치
                    transform.forward = Camera.main.transform.forward;

                }
                else if (attackok && currentTime <= 1)
                {
                    //StartCoroutine("IE_RightMouse1");

                    attackTime = 0;
                    // 클릭하고 일정시간 내에 또 클릭한다면 다음 공격을 하고싶다,.
                    // 그러나 일정시간 내에 공격을 하지 않는다면 첫번째 공격으로 돌아갈 수 있도록 하고 싶다.

                    // 왼쪽 공격 꾹 누르면 기모으기

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
            // -> 에너미 HP에 존재하는 데미지 함수를 호출하고 싶다.
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
        // Physics.SphereCast (레이저를 발사할 위치, 구의 반경, 발사 방향, 충돌 결과, 최대 거리)
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
    // 필요속성 : 궁극기 상태를 표시하는 bool변수
    // 플레이어가 궁극기를 키고 스킬을 사용할 때 적과 부딪혔다면
    private void OnCollisionStay(Collision collision)
    {
        if (ultimateState == true)
        {
            // 부딪힌 적의 에너미HP스크립트를 가져와서
            //if(collision.gameObject.name.Contains("Enemy"))
            
            LHE_EnemyHP enemyNearHP = collision.gameObject.GetComponentInChildren<LHE_EnemyHP>();
            LHE_EnemyFarHP enemyFarHP = collision.gameObject.GetComponentInChildren<LHE_EnemyFarHP>();

            // 데미지함수를 호출하고 싶다.
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
