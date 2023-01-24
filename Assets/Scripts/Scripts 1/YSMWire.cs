using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YSMWire : MonoBehaviour
{
    Vector3 dir;
    public float speed = 10;
    float currentTime = 0;
    public GameObject wireImpactFactory;

    void Start()
    {
        dir = Camera.main.transform.forward;
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
       /* currentTime += Time.deltaTime;
        if (currentTime > 2)
        {
            Destroy(gameObject);
            currentTime = 0;
        }*/

    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject wireImpact = Instantiate(wireImpactFactory);
        wireImpact.transform.position = transform.position;
        //StartCoroutine(fixWire());
         YSMPlayerAttack.Instance.StartCoroutine("IE_Wire");
        Destroy(gameObject);
    }

    IEnumerator fixWire()
    {
        yield return null;

    }
}
