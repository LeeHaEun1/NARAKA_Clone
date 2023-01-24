using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_EnemyArrow : MonoBehaviour
{
    public float arrowSpeed = 50;

    //AudioSource arrowSound;

    // Start is called before the first frame update
    void Start()
    {
        // 효과음
        //arrowSound = GetComponent<AudioSource>();
        //arrowSound.Play();

        // 앞방향으로 이동
        GetComponent<Rigidbody>().AddForce(transform.forward * arrowSpeed);
        // 5초 후에 자동 소멸
        Destroy(gameObject, 5);
    }

    public GameObject arrowHitEffect;
    // 충돌한 대상이 Player라면 Destroy & AddDamage(1)
    // 충돌한 대상이 Wall이라면 Destroy
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LHE_test_PlayerHP.Instance.AddDamage(1);
            Destroy(gameObject);

            // 이펙트
            GameObject arrowEffect = Instantiate(arrowHitEffect);
            arrowEffect.transform.position = collision.contacts[0].point;
            arrowEffect.transform.forward = collision.contacts[0].normal;
        }
        else if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
