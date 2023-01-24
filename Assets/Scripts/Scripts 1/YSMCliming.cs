using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 만약 접촉한다면 벽을 타고 싶다.
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
        // 부딪힌 것이 플레이어라면 YSMPlayerMovement컴포넌트를 가져온다.
        // 만약 가져온 컴포넌트가 있다면
        if (player)
        {
            // 플레이어를 클라이밍 상태로 전환한다.
            player.moveState = YSMPlayerMovement.MoveState.Climb;

            // 플레이어가 사다리의 종점에 도달했는지를 확인하기 위한 종점의 높이 값을 전달한다.
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
        // 부딪힌 것이 플레이어라면 YSMPlayerMovement컴포넌트를 가져온다.
        // 만약 가져온 컴포넌트가 있다면
        if (player)
        {
            // 플레이어를 클라이밍 상태로 전환한다.
            player.moveState = YSMPlayerMovement.MoveState.Climb;

        }*/
    }


}
