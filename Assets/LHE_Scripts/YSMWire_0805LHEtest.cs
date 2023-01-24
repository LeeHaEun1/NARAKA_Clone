using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class YSMWire : MonoBehaviour
{
    Vector3 dir;
    public float speed = 10;

    void Start()
    {
        Transform hitInfoPos = YSMPlayerAttack.Instance.hitInpo.transform;
        dir = hitInfoPos.position - transform.position;
    }

    void Update()
    {
        transform.position += dir * 20 * Time.deltaTime;
    }
    // 부딪히면 사라지고 싶다.
    private void OnTriggerEnter(Collider other)
    {
        YSMPlayerAttack.Instance.StartCoroutine("IE_Wire");
        Destroy(gameObject);
    }

    
}*/
public class YSMWire_0805LHEtest : MonoBehaviour
{
    Vector3 dir;
    public float speed = 10;

    void Start()
    {
        dir = Camera.main.transform.forward;
    }

    void Update()
    {
        transform.position += dir * 20 * Time.deltaTime;
    }
    // ?ε????? ??????? ???.
    private void OnTriggerEnter(Collider other)
    {
        YSMPlayerAttack_0805LHEtest.Instance.StartCoroutine("IE_Wire");

        // [LHE 0806]
        print("player startcoroutine IE_Wire");

        Destroy(gameObject);
    }
}
