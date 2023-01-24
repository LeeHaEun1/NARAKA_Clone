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
        // ȿ����
        //arrowSound = GetComponent<AudioSource>();
        //arrowSound.Play();

        // �չ������� �̵�
        GetComponent<Rigidbody>().AddForce(transform.forward * arrowSpeed);
        // 5�� �Ŀ� �ڵ� �Ҹ�
        Destroy(gameObject, 5);
    }

    public GameObject arrowHitEffect;
    // �浹�� ����� Player��� Destroy & AddDamage(1)
    // �浹�� ����� Wall�̶�� Destroy
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LHE_test_PlayerHP.Instance.AddDamage(1);
            Destroy(gameObject);

            // ����Ʈ
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
