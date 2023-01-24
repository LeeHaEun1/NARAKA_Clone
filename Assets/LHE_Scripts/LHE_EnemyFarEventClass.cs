using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_EnemyFarEventClass : MonoBehaviour
{
    public LHE_EnemyFarMove enemyFar;

    //public GameObject arrowFactory;
    //public GameObject arrowPosition;

    //GameObject player;

    void Start()
    {
        //player = GameObject.Find("Player");
    }

    public void OnArrowAttack()
    {
        /*// attackTime이 되면 화살 생성
        // ** arrow자체에서 AddDamage하는 방향으로 갈까..??
        GameObject arrow = Instantiate(arrowFactory);

        // 화살 발사 위치의 forward를 player 향하게 회전
        Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
        arrowPosition.transform.forward = dir2;

        // 화살/총구 위치로 변경
        arrow.transform.position = arrowPosition.transform.position;
        arrow.transform.forward = arrowPosition.transform.forward;*/
        enemyFar.OnArrowAttack();
    }
}
