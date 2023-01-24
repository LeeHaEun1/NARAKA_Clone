using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_test_PlayerHP : MonoBehaviour
{
    public int hp;
    public int maxHP = 10;
    public Slider sliderHP;
    public Slider profileHP;

    Animator anim;


    // Singleton
    public static LHE_test_PlayerHP Instance;
    private void Awake()
    {
        if (Instance == null)
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
            profileHP.value = value;

            // ü�� 0�Ǹ� Die
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    bool check;
    // damage ũ�⸸ŭ ü�� ����
    public void AddDamage(int damage)
    {
        HP = HP - damage;
        check = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderHP.maxValue = maxHP;
        profileHP.maxValue = maxHP;
        HP = maxHP;

        anim = GetComponentInChildren<Animator>();

    }


    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            anim.SetTrigger("PlayerHit");

        }
        check = false;
    }
}
