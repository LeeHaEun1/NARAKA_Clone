using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 머리 위에 슬라이드바 띄우기

// 플레이어에게 공격 당하면 체력 1씩 감소
public class LHE_EnemyFarHP : MonoBehaviour
{
    public int hp;
    public int maxHP = 10;
    public Slider sliderHP;

    public Animator anim;
    //bool isDead = false;

    AudioSource attackedAudio1; // 피격효과음
    AudioSource attackedAudio2; // 피격효과음

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

            //체력 0되면 Die → 상태머신에서 할까.. ?
            // *** 상태머신에서 anim 등
            if (hp <= 0)
            {
                //isDead = true;
                // 상태 전환 -> 적 자체의 update에서 실행 중
                //LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Die;

                //anim.SetTrigger("Die");
                //isDead = true;

                //Destroy(gameObject, 3); // 이것도 상태에서 할까,,
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

        // 피격 효과음
        attackedAudio1 = GameObject.Find("AttackedAudio1").GetComponent<AudioSource>();
        attackedAudio2 = GameObject.Find("AttackedAudio2").GetComponent<AudioSource>();
    }

    // damage 크기만큼 체력 감소
    public void AddDamage(int damage)
    {
        anim.SetTrigger("Attacked1");
        
        // HP 감소시키고
        HP = HP - damage;

        // 피격효과 발동 => 피격 상태로 전환시키는 것으로 수정(frame 문제)
        // ** -> 추후 데미지 크기별로 피격효과 정도 조절
        if (HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked;
        }

        // 효과음
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

        // HP 감소시키고
        HP = HP - damage;

        // 공격 당하는 순간의 rotation값 저장하고
        CurrRot = LHE_EnemyFarMove.Instance.transform.rotation;
        // 피격효과 발동
        if (HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked2;
        }

        // 효과음
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

        // HP 감소시키고
        HP = HP - damage;

        // 공격 당하는 순간의 position값 저장하고
        CurrPos = LHE_EnemyFarMove.Instance.transform.position;
        // 피격효과 발동
        if(HP > 0)
        {
            LHE_EnemyFarMove.Instance.state = LHE_EnemyFarMove.State.Attacked3;
        }

        // 효과음
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
