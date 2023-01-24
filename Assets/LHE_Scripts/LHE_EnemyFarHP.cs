using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // �Ӹ� ���� �����̵�� ����

// �÷��̾�� ���� ���ϸ� ü�� 1�� ����
public class LHE_EnemyFarHP : MonoBehaviour
{
    public int hp;
    public int maxHP = 10;
    public Slider sliderHP;

    public Animator anim;
    //bool isDead = false;

    AudioSource attackedAudio1; // �ǰ�ȿ����
    AudioSource attackedAudio2; // �ǰ�ȿ����

    // Singleton
    public static LHE_EnemyFarHP Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            sliderHP.value = value;

            //ü�� 0�Ǹ� Die �� ���¸ӽſ��� �ұ�.. ?
            // *** ���¸ӽſ��� anim ��
            if (hp <= 0)
            {
                //isDead = true;
                // ���� ��ȯ -> �� ��ü�� update���� ���� ��
                //LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Die;

                //anim.SetTrigger("Die");
                //isDead = true;

                //Destroy(gameObject, 3); // �̰͵� ���¿��� �ұ�,,
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderHP.maxValue = maxHP;
        HP = maxHP;

        anim = GetComponentInChildren<Animator>();
        
        //isDead = false;

        // �ǰ� ȿ����
        attackedAudio1 = GameObject.Find("AttackedAudio1").GetComponent<AudioSource>();
        attackedAudio2 = GameObject.Find("AttackedAudio2").GetComponent<AudioSource>();
    }

    // damage ũ�⸸ŭ ü�� ����
    public void AddDamage(int damage)
    {
        anim.SetTrigger("Attacked1");
        
        // HP ���ҽ�Ű��
        HP = HP - damage;

        // �ǰ�ȿ�� �ߵ� => �ǰ� ���·� ��ȯ��Ű�� ������ ����(frame ����)
        // ** -> ���� ������ ũ�⺰�� �ǰ�ȿ�� ���� ����
        if (HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked;
        }

        // ȿ����
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            attackedAudio1.Play();
        }
        else
        {
            attackedAudio2.Play();
        }
    }

    public Quaternion CurrRot;
    public void AddDamage2(int damage)
    {
        anim.SetTrigger("Attacked2");

        // HP ���ҽ�Ű��
        HP = HP - damage;

        // ���� ���ϴ� ������ rotation�� �����ϰ�
        CurrRot = LHE_EnemyFarMove.Instance.transform.rotation;
        // �ǰ�ȿ�� �ߵ�
        if (HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked2;
        }

        // ȿ����
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            attackedAudio1.Play();
        }
        else
        {
            attackedAudio2.Play();
        }
    }

    public Vector3 CurrPos;
    public void AddDamage3(int damage)
    {
        anim.SetTrigger("Attacked3");

        // HP ���ҽ�Ű��
        HP = HP - damage;

        // ���� ���ϴ� ������ position�� �����ϰ�
        CurrPos = LHE_EnemyFarMove.Instance.transform.position;
        // �ǰ�ȿ�� �ߵ�
        if(HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked3;
        }

        // ȿ����
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            attackedAudio1.Play();
        }
        else
        {
            attackedAudio2.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(isDead == true)
        //{
        //    //anim.SetTrigger("Die");
        //    //isDead = false;
        //}
    }
}
