using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMLeftSkill : MonoBehaviour
{
    List<GameObject> list = new List<GameObject>();
    float radius = 2;
    RaycastHit attackHitInfo;
    float attackDistance = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int enemyNear = 1 << LayerMask.NameToLayer("EnemyNear");
        int enemyFar = 1 << LayerMask.NameToLayer("EnemyFar");
        int enemy = enemyFar + enemyNear;
        print(enemy);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, Vector3.up, 0, enemy);

        foreach (RaycastHit hit in hits)
        {
            print("1111");
            LHE_EnemyHP enemyNearHp = hit.transform.GetComponent<LHE_EnemyHP>();

            if (enemyNearHp)
            {
                if (list.Contains(hit.transform.gameObject))
                {
                    break;
                }

                enemyNearHp.AddDamage(1);
                print("2222");
                print(enemyNearHp.HP);
                list.Add(hit.transform.gameObject);
            }

            LHE_EnemyFarHP enemyFarHp = hit.transform.GetComponent<LHE_EnemyFarHP>();

            if (enemyFarHp)
            {
                if (list.Contains(hit.transform.gameObject))
                {
                    break;
                }

                enemyFarHp.AddDamage(1);
                print("2222");
                print(enemyFarHp.HP);
                list.Add(hit.transform.gameObject);
            }
        }
    }
    void OnDrawGizmos()
    {
        // Physics.SphereCast (레이저를 발사할 위치, 구의 반경, 발사 방향, 충돌 결과, 최대 거리)
        bool isHit = Physics.SphereCast(transform.position, radius, Vector3.up, out attackHitInfo, 3, LayerMask.GetMask("Enemy"));

        Gizmos.color = Color.red;

        if (isHit)
        {
            Gizmos.DrawWireSphere(transform.position, radius);

        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, radius);

        }
    }
}
