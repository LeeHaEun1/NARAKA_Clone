using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_EnemyNearEventClass : MonoBehaviour
{
    public LHE_EnemyMove enemyNear;

    public void OnAttack0()
    {
        enemyNear.OnAttack0();
    }

    public void OnAttack1_1()
    {
        enemyNear.OnAttack1_1();
    }

    public void OnAttack1_2()
    {
        enemyNear.OnAttack1_2();
    }

    public void OnAttack2()
    {
        enemyNear.OnAttack2();
    }
}
