using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �����Ѵٸ� ���� Ÿ�� �ʹ�.
public class YSMCliming_LHEtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        YSMPlayerMovement_LHEtest player = other.GetComponent<YSMPlayerMovement_LHEtest>();
        // �ε��� ���� �÷��̾��� YSMPlayerMovement������Ʈ�� �����´�.
        // ���� ������ ������Ʈ�� �ִٸ�
        if (player)
        {
            // �÷��̾ Ŭ���̹� ���·� ��ȯ�Ѵ�.
            player.moveState = YSMPlayerMovement_LHEtest.MoveState.Climb;
        }

        YSMWire wire = other.GetComponent<YSMWire>();
        if(wire)
        {
            print("11");
            YSMPlayerAttack_LHEtest.Instance.StartCoroutine("IE_Wire");

        }
    }
}
