using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMDamage : MonoBehaviour
{

    public static YSMDamage Instance;
    private void Awake()
    {
        Instance = this;
    }

    public bool damageCont1 = false;
    public bool damageCont2 = false;
    public bool damageCont3 = false;

    public void DamageCont1()
    {
        damageCont1 = true;
    }
    public void DamageCont2()
    {
        damageCont2 = true;
    }
    public void DamageCont3()
    {
        damageCont3 = true;
    }
}
