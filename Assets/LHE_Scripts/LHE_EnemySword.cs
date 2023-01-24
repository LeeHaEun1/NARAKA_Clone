using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_EnemySword : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            print("player-sword collide");

            LHE_test_PlayerHP.Instance.AddDamage(1);
        }
    }
}
