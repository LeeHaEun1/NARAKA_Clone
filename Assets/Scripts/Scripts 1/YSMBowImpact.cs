using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMBowImpact : MonoBehaviour
{
    public GameObject bowAttackImpactFactory;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            GameObject bowImpact = Instantiate(bowAttackImpactFactory);
            bowImpact.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }
}
