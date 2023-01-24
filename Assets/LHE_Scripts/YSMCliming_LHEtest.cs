using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 만약 접촉한다면 벽을 타고 싶다.
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
        // 부딪힌 것이 플레이어라면 YSMPlayerMovement컴포넌트를 가져온다.
        // 만약 가져온 컴포넌트가 있다면
        if (player)
        {
            // 플레이어를 클라이밍 상태로 전환한다.
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
