using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMPlayerAttack_LHEtest : MonoBehaviour
{
    // LHE 0803
    // LHE_EnemyMove���� ��� ���� public���� ����(����: private)
    public RaycastHit hitInpo;

    private float curTime = 0;
    private float wireSmoothness = 5;
    CharacterController cc;
    Animator anim;

    // �ʿ�Ӽ� : ȭ�����, ȭ�� Ǯ, ȭ�� ����
    public GameObject arrowFactory; // ȭ�� ����
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
        // enemy �ǰ�ȿ�� �׽�Ʈ������ �߰� -> �Ÿ���� �߰��ؾ���!!
        // ���Ÿ�/�ٰŸ� ���ݿ� ���� hit���γ�, ���ݰŸ� ���ο� �ִ��� ���� üũ�ϴ� �� �߰� �ʿ�
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

            // ���̾ ��� ���� �¾Ҵٸ�
            if (Physics.Raycast(ray, out hitInpo, 20))
            {
                // ���̾� ���� ���忡�� ���̾ ������ �� ��ġ�� ������ ���� �ʹ�.
                GameObject wire = Instantiate(wireFactory);
                wire.transform.position = transform.position;
                // �浹ü���� �̵� Wire�ڷ�ƾ ���
                transform.rotation = Quaternion.LookRotation(hitInpo.transform.position - transform.position);
                //StartCoroutine("IE_Wire");

                // LHE test 0804
                print("hitInpo.normal" + hitInpo.normal);
            }
        }
    }
    IEnumerator IE_Wire()
    {
        YSMPlayerMovement_LHEtest.Instance.canGravity = false; //�߷� ��Ȱ��
        while (true)
        {
            if (curTime < 1)
            {
                curTime += Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, hitInpo.point, Time.deltaTime * 100);

                // LHE 0803
                // �÷��̾� state�� wire�� �ӹ��������� enemy���µ� wire�� ���ϴ� ���� �߻�
                // -> �ذ� ���� �÷��̾ hitpoint �����ϸ� attackstate�������ֱ�
                float distance = Vector3.Distance(transform.position, hitInpo.point);
                if(distance < 0.1f)
                {
                    transform.position = hitInpo.point;
                    attackState = AttackState.arrow;
                }

                // ���̾� �� �������� ȸ���ϰ� �ʹ�.
                // transform.eulerAngles = new Vector3(0, 0, hitInpo.transform.position.z);
                yield return null;
            }
            else
            {
                YSMPlayerMovement_LHEtest.Instance.canGravity = true; // �߷� Ȱ��

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
                // �������� �̵� �ڷ�ƾ 
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
        // ���� ���� ��ư�� ������ ȭ���� ������ �ʹ�.
        // -> 
        // ���� ��ư�� ������ ī�޶� ������ �̵���Ű�� �ʹ�.

        if (Input.GetButtonDown("Fire1"))
        {
            // ī�޶� ������ �̵���Ű��
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance - 2;
            anim.SetTrigger("Bow");

        }
        // ���� ���� ��ư�� ���� ���� ��ġ�� ���ư��� �ʹ�
        // ȭ���� ������ �ϰ�ʹ�.
        if (Input.GetButtonUp("Fire1"))
        {
            // ī�޶� ���� ��ġ
            YSMCameraMovement.Instance.maxDistance = YSMCameraMovement.Instance.maxDistance + 2;

            // ȭ���� �����ؼ� �� ����ġ�� ����ʹ�.
            GameObject arrow = Instantiate(arrowFactory);
            arrow.transform.forward = Camera.main.transform.forward;
            arrow.transform.position = transform.position + new Vector3(0.5f,0,0);


        }
    }
}
