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
        /*// attackTime�� �Ǹ� ȭ�� ����
        // ** arrow��ü���� AddDamage�ϴ� �������� ����..??
        GameObject arrow = Instantiate(arrowFactory);

        // ȭ�� �߻� ��ġ�� forward�� player ���ϰ� ȸ��
        Vector3 dir2 = player.transform.position - arrowPosition.transform.position;
        arrowPosition.transform.forward = dir2;

        // ȭ��/�ѱ� ��ġ�� ����
        arrow.transform.position = arrowPosition.transform.position;
        arrow.transform.forward = arrowPosition.transform.forward;*/
        enemyFar.OnArrowAttack();
    }
}
