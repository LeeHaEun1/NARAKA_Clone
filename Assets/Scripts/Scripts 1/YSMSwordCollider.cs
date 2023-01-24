using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMSwordCollider : MonoBehaviour
{
    AudioSource swordSound;
    // Start is called before the first frame update
    void Start()
    {
        swordSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 무기 콜라이더를 감지했을 때 enemyHP스크립트를 가져와서 확인 후 
    // 만약 enemyHP를 가지고 있다면 HP 를 감소시키고 싶다.
    private void OnCollisionEnter(Collision collision)
    {
        print("collision name : " + collision.gameObject.name);
        if(collision.gameObject.name.Contains("Enemy"))
        {
            LHE_EnemyHP enemyNearHP = collision.gameObject.GetComponent<LHE_EnemyHP>();
            if(enemyNearHP)
            {
                if(YSMDamage.Instance.damageCont1 == true)
                {
                    enemyNearHP.AddDamage(1);
                }
                else if (YSMDamage.Instance.damageCont2 == true)
                {
                    enemyNearHP.AddDamage2(1);
                }
                else if (YSMDamage.Instance.damageCont3 == true)
                {
                    enemyNearHP.AddDamage3(1);
                }

                

                swordSound.Stop();
                swordSound.Play();
            }
            print("11111111111111111111111");

            LHE_EnemyFarHP enemyFarHP = collision.gameObject.GetComponent<LHE_EnemyFarHP>();
            if(enemyFarHP)
            {
                if (YSMDamage.Instance.damageCont1 == true)
                {
                    enemyFarHP.AddDamage(1);
                }
                else if (YSMDamage.Instance.damageCont2 == true)
                {
                    enemyFarHP.AddDamage2(1);
                }
                else if (YSMDamage.Instance.damageCont3 == true)
                {
                    enemyFarHP.AddDamage3(1);
                }
                swordSound.Stop();
                swordSound.Play();
            }

            YSMDamage.Instance.damageCont1 = false;
            YSMDamage.Instance.damageCont2 = false;
            YSMDamage.Instance.damageCont3 = false;
            print("2222222222222222");
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        LHE_EnemyHP enemyNearHP = other.gameObject.GetComponentInChildren<LHE_EnemyHP>();
        enemyNearHP.AddDamage(1);
        print("3333333333333333333");

        LHE_EnemyFarHP enemyFarHP = other.gameObject.GetComponentInChildren<LHE_EnemyFarHP>();
        enemyFarHP.AddDamage(1);
        print("4444444444444444444");
    }*/
}
