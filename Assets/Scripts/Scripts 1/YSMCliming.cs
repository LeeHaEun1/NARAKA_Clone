using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �����Ѵٸ� ���� Ÿ�� �ʹ�.
public class YSMCliming : MonoBehaviour
{
    //public float exitHeight = 10;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /*private void OnTriggerEnter(Collider other)
    {
        YSMPlayerMovement player = other.GetComponent<YSMPlayerMovement>();
        // �ε��� ���� �÷��̾��� YSMPlayerMovement������Ʈ�� �����´�.
        // ���� ������ ������Ʈ�� �ִٸ�
        if (player)
        {
            // �÷��̾ Ŭ���̹� ���·� ��ȯ�Ѵ�.
            player.moveState = YSMPlayerMovement.MoveState.Climb;

            // �÷��̾ ��ٸ��� ������ �����ߴ����� Ȯ���ϱ� ���� ������ ���� ���� �����Ѵ�.
            //player.exHeight = exitHeight;
        }

        YSMWire wire = other.GetComponent<YSMWire>();
        if (wire)
        {
            print("11");
            YSMPlayerAttack.Instance.StartCoroutine("IE_Wire");

        }
    }*/

    private void OnCollisionStay(Collision collision)
    {
        /*YSMPlayerMovement player = collision.gameObject.GetComponent<YSMPlayerMovement>();
        // �ε��� ���� �÷��̾��� YSMPlayerMovement������Ʈ�� �����´�.
        // ���� ������ ������Ʈ�� �ִٸ�
        if (player)
        {
            // �÷��̾ Ŭ���̹� ���·� ��ȯ�Ѵ�.
            player.moveState = YSMPlayerMovement.MoveState.Climb;

        }*/
    }


}
