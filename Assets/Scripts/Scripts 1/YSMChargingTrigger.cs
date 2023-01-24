using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMChargingTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Enemy"))
        {
            LHE_EnemyHP enemyNearHP = collision.gameObject.GetComponent<LHE_EnemyHP>();
            if (enemyNearHP)
            {
                enemyNearHP.AddDamage2(1);
                print("11111111");
            }

            LHE_EnemyFarHP enemyFarHP = collision.gameObject.GetComponent<LHE_EnemyFarHP>();
            if (enemyFarHP)
            {
                enemyFarHP.AddDamage2(1);
                print("22222222");

            }
        }
    }
}
