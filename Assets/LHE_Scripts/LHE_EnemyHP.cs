using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // �Ӹ� ���� �����̵�� ����

// �÷��̾�� ���� ���ϸ� ü�� 1�� ����
public class LHE_EnemyHP : MonoBehaviour
{
    public int hp;
    public int maxHP = 10;
    public Slider sliderHP;

    public Animator anim;
    //bool isDead = false;

    AudioSource attackedAudio1; // �ǰ�ȿ����
    AudioSource attackedAudio2; // �ǰ�ȿ����

    // Singleton
    public static LHE_EnemyHP Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderHP.maxValue = maxHP;
        HP = maxHP;

        anim = GetComponentInChildren<Animator>();

        // �ǰ� ȿ����
        attackedAudio1 = GameObject.Find("AttackedAudio1").GetComponent<AudioSource>();
        attackedAudio2 = GameObject.Find("AttackedAudio2").GetComponent<AudioSource>();
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
                //anim.SetTrigger("Die");

                // ���¿��� ����ϸ� ��� �����,,
                //LHE_EnemyMove.Instance.anim.SetTrigger("Die");
            }
        }
    }

    // damage ũ�⸸ŭ ü�� ����
    public void AddDamage(int damage)
    {
        anim.SetTrigger("Attacked1");

        // HP ���ҽ�Ű��
        HP = HP - damage;

        // �ǰ�ȿ�� �ߵ�
        if(HP > 0)
        {
            LHE_EnemyMove.Instance.state = LHE_EnemyMove.State.Attacked;
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
        CurrRot = LHE_EnemyMove.Instance.transform.rotation;
        // �ǰ�ȿ�� �ߵ�
        if(HP > 0)
        {
            LHE_EnemyMove.Instance.state = LHE_EnemyMove.State.Attacked2;
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
        anim.SetTrigger("Attacked3Up");

        // HP ���ҽ�Ű��
        HP = HP - damage;

        // ���� ���ϴ� ������ position�� �����ϰ�
        CurrPos = LHE_EnemyMove.Instance.transform.position;
        // �ǰ�ȿ�� �ߵ�
        if(HP > 0)
        {
            LHE_EnemyMove.Instance.state = LHE_EnemyMove.State.Attacked3;
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
        //if (isDead == true)
        //{
        //    anim.SetTrigger("Die");
        //    isDead = false;
        //}
    }
}
