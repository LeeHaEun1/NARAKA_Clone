using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMAttackImpact : MonoBehaviour
{
    Vector3 dir;
    public float speed = 50;
    float currentTime = 0;
    GameObject player;
    public bool damageCheck = false;

    public static YSMAttackImpact Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        //dir = player.transform.forward;
        dir = player.transform.forward;
        dir.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        /*currentTime += Time.deltaTime;
        if (currentTime > 2)
        {
            Destroy(gameObject);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(damageCheck == true)
        {
            if (other.gameObject.tag == "EnemyNear")
            {
                LHE_EnemyHP.Instance.AddDamage(1);
            }
            if (other.gameObject.tag == "EnemyFar")
            {
                LHE_EnemyHP.Instance.AddDamage(1);
            }
        }
        damageCheck = false;
    }
}
